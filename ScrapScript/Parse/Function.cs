namespace ScrapScript.Parse
{
    public class Function : Object
    {
        public Function(Object arg, Object body)
        {
            Arg = arg;
            Body = body;
        }

        public Object Arg { get; }
        public Object Body { get; }
    }
}
