using System;
using System.Collections.Generic;

using Scrapscript.Scrap;
using Scrapscript.Tokenize;

namespace Scrapscript
{
    internal class Program
    {
        public static Dictionary<string, Prec> ps = new Dictionary<string, Prec>();
        public static HashSet<char> operChars = new HashSet<char>();
        public static double highestPrec = 0.0;

        static void Init()
        {
            ps["::"] = Prec.Lp(2000);
            ps["@"] = Prec.Rp(1001);
            ps[string.Empty] = Prec.Rp(1000);
            ps[">>"] = Prec.Lp(14);
            ps["<<"] = Prec.Lp(14);
            ps["^"] = Prec.Rp(13);
            ps["*"] = Prec.Lp(12);
            ps["/"] = Prec.Lp(12);
            ps["//"] = Prec.Lp(12);
            ps["%"] = Prec.Lp(12);
            ps["+"] = Prec.Lp(11);
            ps["-"] = Prec.Lp(11);
            ps[">*"] = Prec.Rp(10);
            ps["++"] = Prec.Rp(10);
            ps["=="] = Prec.Np(9);
            ps["/="] = Prec.Np(9);
            ps["<"] = Prec.Np(9);
            ps[">"] = Prec.Np(9);
            ps["<="] = Prec.Np(9);
            ps[">="] = Prec.Np(9);
            ps["&&"] = Prec.Rp(8);
            ps["||"] = Prec.Rp(7);
            ps["|>"] = Prec.Rp(6);
            ps["<|"] = Prec.Lp(6);
            ps["->"] = Prec.Lp(5);
            ps["|"] = Prec.Rp(4.5);
            ps[":"] = Prec.Lp(4.5);
            ps["="] = Prec.Rp(4);
            ps["!"] = Prec.Lp(3);
            ps["."] = Prec.Rp(3);
            ps["?"] = Prec.Rp(3);
            ps[","] = Prec.Xp(1);
            ps["..."] = Prec.Xp(0);

            foreach (var prec in ps.Values)
            {
                if (prec.Pl > highestPrec) highestPrec = prec.Pl;
                if (prec.Pr > highestPrec) highestPrec = prec.Pr;
            }

            foreach (string key in ps.Keys) foreach (char c in key) operChars.Add(c);
        }

        static Lexer Lex(string str)
        {
            return new Lexer(ps, operChars, str);
        }

        static void Main(string[] args)
        {
            Init();

            var l = Lex("   42");
            var token = l.ReadOne();

            Console.WriteLine(((Int)token).Value);

            l = Lex("42.2   ");
            token = l.ReadOne();

            Console.WriteLine(((Float)token).Value);

            l = Lex("   \"Hello, World!\"");
            token = l.ReadOne();

            Console.WriteLine(((Tokenize.Text)token).Value);

            l = Lex("   |> ");
            token = l.ReadOne();

            Console.WriteLine(((Operator)token).Value);

            l = Lex("   $hello ");
            token = l.ReadOne();

            Console.WriteLine(((Name)token).Value);

            l = Lex("   #smooth ");
            token = l.ReadOne();
            Console.WriteLine(((Symbol)token).Value);

            var obj = new Parse.Parser(ps, highestPrec, Token.Tokenize(ps, operChars, "x + y + z . z = x + y . x = 1 . y = 2")).Parse(0);
        }
    }
}
