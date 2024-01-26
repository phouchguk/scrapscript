using ScrapScript.Scrap;

namespace ScrapScript.Tokenize
{
    public abstract class Token
    {
        public int LineNo { get; set; }

        public static Queue<Token> Tokenize(Dictionary<string, Prec> precs, HashSet<char> operChars, string str)
        {
            var lexer = new Lexer(precs, operChars, str);
            var tokens = new Queue<Token>();
            Token token = null;

            while (!((token = lexer.ReadOne()) is Eof)) tokens.Enqueue(token);

            return tokens;
        }
    }
}
