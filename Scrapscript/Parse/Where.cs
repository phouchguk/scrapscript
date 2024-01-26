namespace Scrapscript.Parse
{
    public class Where : Object
    {
        public Where(Object body, Object binding)
        {
            Body = body;
            Binding = binding;
        }

        public Object Body { get; }
        public Object Binding { get; }
    }
}
