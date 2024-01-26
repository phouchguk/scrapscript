namespace Scrapscript.Parse
{
    public class Access : Object
    {
        public Access(Object obj, Object at)
        {
            Obj = obj;
            At = at;
        }

        public Object Obj { get; }
        public Object At { get; }
    }
}
