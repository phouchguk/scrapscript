namespace Scrapscript.Parse
{
    public class Assert : Object
    {
        public Assert(Object value, Object cond)
        {
            Value = value;
            Cond = cond;
        }

        public Object Value { get; }
        public Object Cond { get; }
    }
}
