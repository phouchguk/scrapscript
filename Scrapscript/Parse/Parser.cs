namespace Scrapscript.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    using Scrapscript.Scrap;
    using Scrapscript.Tokenize;

    public class Parser
    {
        private readonly Dictionary<string, Prec> ps;
        private readonly double highestPrec;
        private readonly Queue<Tokenize.Token> tokens;

        public Parser(Dictionary<string, Prec> ps, double highestPrec, Queue<Tokenize.Token> tokens)
        {
            this.ps = ps;
            this.highestPrec = highestPrec;
            this.tokens = tokens;
        }

        private Assign ParseAssign(double p = 0.0)
        {
            Object assign = this.Parse(p);

            if (assign is Spread)
            {
                return new Assign(new Var("..."), assign);
            }
            else if (!(assign is Assign))
            {
                throw new Exception("failed to parse variable assignment in record constructor");
            }

            return (Assign)assign;
        }

        public Object Parse(double p = 0.0)
        {
            if (!this.tokens.Any())
            {
                throw new Exception("unexpected end of input");
            }

            var token = this.tokens.Dequeue();
            Object? l = null;

            if (token is Tokenize.Int)
            {
                l = new Int(((Tokenize.Int)token).Value);
            }
            else if (token is Tokenize.Float)
            {
                l = new Float(((Tokenize.Float)token).Value);
            }
            else if (token is Tokenize.Name)
            {
                l = new Var(((Tokenize.Name)token).Value);
            }
            else if (token is Tokenize.Symbol)
            {
                l = new Symbol(((Tokenize.Symbol)token).Value);
            }
            else if (token is Tokenize.Text)
            {
                l = new Text(((Tokenize.Text)token).Value);
            }
            else if (token is Tokenize.Operator)
            {
                Tokenize.Operator op = (Tokenize.Operator)token;

                if (op.Value == "...")
                {
                    if (tokens.Any() && tokens.Peek() is Tokenize.Name)
                    {
                        return new Spread(((Tokenize.Name)tokens.Dequeue()).Value);
                    }
                    else
                    {
                        return new Spread(null);
                    }
                }
                else if (op.Value == "|")
                {
                    Object expr = this.Parse(this.ps["|"].Pr);

                    if (!(expr is Function))
                    {
                        throw new Exception($"expected function in match expression {expr}");
                    }

                    Function f = (Function)expr;

                    var cases = new List<MatchCase>();
                    cases.Add(new MatchCase(f.Arg, f.Body));

                    while (tokens.Any() && tokens.Peek() is Operator && ((Operator)tokens.Peek()).Value == "|")
                    {
                        tokens.Dequeue();

                        expr = this.Parse(this.ps["|"].Pr);

                        if (!(expr is Function))
                        {
                            throw new Exception($"expected function in match expression {expr}");
                        }

                        f = (Function)expr;
                        cases.Add(new MatchCase(f.Arg, f.Body));
                    }

                    l = new MatchFunction(cases);
                } else if (op.Value == "-")
                {
                    Object r = this.Parse(this.highestPrec + 1);
                    l = new Binop(BinopKind.Sub, new Int(0), r);
                }
            }
            else if (token is LeftParen)
            {
                if (tokens.Peek() is RightParen)
                {
                    l = Hole.Instance;
                }
                else
                {
                    l = this.Parse();
                }

                tokens.Dequeue(); // pop closing )
            }
            else if (token is LeftBracket)
            {
                token = tokens.Peek();

                if (token is RightBracket)
                {
                    tokens.Dequeue();
                    l = new Parse.List(Enumerable.Empty<Object>());
                }
                else
                {
                    var items = new List<Object>();

                    items.Add(this.Parse(2));

                    while (!(tokens.Dequeue() is RightBracket)) // pop ,
                    {
                        if (items[items.Count() - 1] is Spread)
                        {
                            throw new Exception("spread must come at end of list match");
                        }

                        items.Add(this.Parse(2));
                    }

                    l = new Parse.List(items);
                }
            }
            else if (token is LeftBrace)
            {
                var r = new Record();

                token = tokens.Peek();

                if (token is RightBracket)
                {
                    tokens.Dequeue();
                }
                else
                {
                    var assign = this.ParseAssign(2);
                    r.Data[assign.Name.Name] = assign.Value;

                    while (!(tokens.Dequeue() is RightBrace)) // pop ,
                    {
                        if (assign.Value is Spread)
                        {
                            throw new Exception("spread must come at end of record match");
                        }

                        assign = this.ParseAssign(2);
                        r.Data[assign.Name.Name] = assign.Value;
                    }
                }

                l = r;
            }
            else
            {
                throw new Exception($"unexpected token {token}");
            }

            if (l == null)
            {
                throw new Exception("unreachable");
            }

            while (true)
            {
                if (!tokens.Any())
                {
                    break;
                }

                Token tok = tokens.Peek();

                if (tok is RightParen || tok is RightBracket || tok is RightBrace)
                {
                    break;
                }

                if (!(tok is Operator))
                {
                    Prec nonOpPrec = this.ps[string.Empty];

                    if (nonOpPrec.Pl < p)
                    {
                        break;
                    }

                    l = new Apply(l, this.Parse(nonOpPrec.Pr));
                    continue;
                }

                Operator op = (Operator)tok;

                Prec prec = this.ps[op.Value];

                if (prec.Pl < p)
                {
                    break;
                }

                tokens.Dequeue();

                if (op.Value == "=")
                {
                    if (!(l is Var))
                    {
                        throw new Exception($"expected variable in assignment {l}");
                    }

                    l = new Assign((Var)l, this.Parse(prec.Pr));
                }
                else if (op.Value == "->")
                {
                    l = new Function(l, this.Parse(prec.Pr));
                }
                else if (op.Value == "|>")
                {
                    l = new Apply(this.Parse(prec.Pr), l);
                }
                else if (op.Value == "<|")
                {
                    l = new Apply(l, this.Parse(prec.Pr));
                }
                else if (op.Value == ">>")
                {
                    l = new Compose(l, this.Parse(prec.Pr));
                }
                else if (op.Value == "<<")
                {
                    l = new Compose(this.Parse(prec.Pr), l);
                }
                else if (op.Value == ".")
                {
                    l = new Where(l, this.Parse(prec.Pr));
                }
                else if (op.Value == "?")
                {
                    l = new Assert(l, this.Parse(prec.Pr));
                }
                else if (op.Value == "@")
                {
                    l = new Access(l, this.Parse(prec.Pr));
                }
                else
                {
                    l = new Binop(this.BinopKindFromStr(op.Value), l, this.Parse(prec.Pr));
                }
            }

            return l;
        }

        private BinopKind BinopKindFromStr(string s)
        {
            switch (s)
            {
                case "+": return BinopKind.Add;
                case "-": return BinopKind.Sub;
                case "*": return BinopKind.Mul;
                case "/": return BinopKind.Div;
                case "//": return BinopKind.FloorDiv;
                case "^": return BinopKind.Exp;
                case "%": return BinopKind.Mod;
                case "==": return BinopKind.Equal;
                case "/=": return BinopKind.NotEqual;
                case "<": return BinopKind.Less;
                case ">": return BinopKind.Greater;
                case "<=": return BinopKind.LessEqual;
                case ">=": return BinopKind.GreaterEqual;
                case "&&": return BinopKind.BoolAnd;
                case "||": return BinopKind.BoolOr;
                case "++": return BinopKind.StringConcat;
                case ">+": return BinopKind.ListCons;
                case "+<": return BinopKind.ListAppend;
                case "!": return BinopKind.RightEval;
                case ":": return BinopKind.HasType;
                case "|>": return BinopKind.Pipe;
                case "<|": return BinopKind.ReversePipe;
                default: throw new Exception($"unrecognised Binop: {s}");
            }
        }
    }
}
