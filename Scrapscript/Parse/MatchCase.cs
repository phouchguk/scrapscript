namespace Scrapscript.Parse
{
    public class MatchCase : Object
    {
        public MatchCase(Object pattern, Object body)
        {
            Pattern = pattern;
            Body = body;
        }

        public Object Pattern { get; }
        public Object Body { get; }
    }
}
