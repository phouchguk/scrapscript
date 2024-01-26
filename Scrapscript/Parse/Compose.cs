namespace Scrapscript.Parse
{
    public class Compose : Object
    {
        public Compose(Object inner, Object outer)
        {
            Inner = inner;
            Outer = outer;
        }

        public Object Inner { get; }
        public Object Outer { get; }
    }
}
