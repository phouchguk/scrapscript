namespace Scrapscript.Parse
{
    public class Apply : Object
    {
        public Apply(Object func, Object arg)
        {
            Func = func;
            Arg = arg;
        }

        public Object Func { get; }
        public Object Arg { get; }
    }
}
