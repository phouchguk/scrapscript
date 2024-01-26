using System.Text;

using ScrapScript.Scrap;

namespace ScrapScript.Tokenize
{
    public class Lexer
    {
        private readonly Dictionary<string, Prec> precs;
        private readonly HashSet<char> operChars;
        private readonly string text;
        private int idx;
        private int lineNo;
        private int colNo;
        private string line;

        public Lexer(Dictionary<string, Prec> precs, HashSet<char> operChars, string text)
        {
            this.precs = precs;
            this.operChars = operChars;
            this.text = text;
            this.idx = 0;
            this.lineNo = 1;
            this.colNo = 1;
            this.line = string.Empty;
        }

        bool HasInput()
        {
            return this.idx < this.text.Length;
        }

        char PeekChar()
        {
            if (!this.HasInput())
            {
                throw new Exception("while reading token");
            }

            return this.text[this.idx];
        }

        char ReadChar()
        {
            char c = this.PeekChar();

            if (c == '\n')
            {
                this.lineNo += 1;
                this.colNo = 1;
                this.line = string.Empty;
            }
            else
            {
                this.line += c;
                this.colNo = 1;
            }

            this.idx++;

            return c;
        }

        bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '$' || c == '\'' || c == '_';
        }

        public Token ReadOne()
        {
            char c = '\0';

            while (this.HasInput())
            {
                c = this.ReadChar();
                if (!char.IsWhiteSpace(c)) break;
            }

            if (c == '\0') return new Eof();

            if (c == '"') return this.ReadString();

            if (c == '-')
            {
                if (this.HasInput() && this.PeekChar() == '-')
                    this.ReadComment();
                return this.ReadOne();
            }

            if (c == '#')
            {
                Token value = this.ReadOne();

                if (value is Eof) throw new Exception("eof while reading symbol");
                if (!(value is Name)) throw new Exception($"expected name after #, got {value}");

                return new Symbol(((Name)value).Value) { LineNo = this.lineNo };
            }

            if (char.IsDigit(c)) return this.ReadNumber(c);

            if (c == '(') return new LeftParen() { LineNo = this.lineNo };
            if (c == ')') return new RightParen() { LineNo = this.lineNo };
            if (c == '[') return new LeftBracket() { LineNo = this.lineNo };
            if (c == ']') return new RightBracket() { LineNo = this.lineNo };
            if (c == '{') return new LeftBrace() { LineNo = this.lineNo };
            if (c == '}') return new RightBrace() { LineNo = this.lineNo };

            if (this.operChars.Contains(c)) return this.ReadOp(c);

            if (this.IsIdentifierChar(c)) return this.ReadVar(c);

            throw new Exception($"unexpected token {c} on line {this.lineNo}, col {this.colNo}: {this.line}");
        }

        Token ReadString()
        {
            var buf = new StringBuilder();

            while (true)
            {
                if (!this.HasInput()) throw new Exception("eof while reading string");
                char c = this.ReadChar();
                if (c == '"') break;
                buf.Append(c);
            }

            return new String(buf.ToString()) { LineNo = this.lineNo  };
        }

        void ReadComment()
        {
            while (this.HasInput() && this.ReadChar() != '\n') ;
        }

        Token ReadNumber(char firstDigit)
        {
            var buf = new StringBuilder();
            buf.Append(firstDigit);

            bool hasDecimal = false;

            while (this.HasInput())
            {
                char c = this.PeekChar();

                if (c == '.')
                {
                    if (hasDecimal)
                    {
                        throw new Exception($"unexpected token '{c}'");
                    }

                    hasDecimal = true;
                }
                else if (!char.IsDigit(c))
                {
                    break;
                }

                this.ReadChar();

                buf.Append(c);
            }

            if (hasDecimal)
            {
                return new Float(double.Parse(buf.ToString())) { LineNo = this.lineNo };
            }

            return new Int(int.Parse(buf.ToString())) { LineNo = this.lineNo };
        }

        bool StartsOperator(string s)
        {
            return this.precs.Keys.Any(k => k.StartsWith(s));
        }

        Token ReadOp(char c)
        {
            var buf = new StringBuilder(3);
            buf.Append(c);

            while (this.HasInput())
            {
                c = this.PeekChar();
                if (!this.StartsOperator(buf.ToString() + c)) break;
                this.ReadChar();
                buf.Append(c);
            }

            if (this.precs.ContainsKey(buf.ToString())) return new Operator(buf.ToString()) { LineNo = this.lineNo };

            throw new Exception($"unexpected token {buf}");
        }

        Token ReadVar(char firstChar)
        {
            var buf = new StringBuilder();
            buf.Append(firstChar);

            while (this.HasInput() && this.IsIdentifierChar(this.PeekChar())) buf.Append(this.ReadChar());

            return new Name(buf.ToString()) { LineNo = this.lineNo };
        }
    }
}
