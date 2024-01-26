namespace Scrapscript.Tokenize
{
    public class Eof : Token
    {
        private Eof()
        {
        }

        public static Eof Instance = new Eof();
    }
}
