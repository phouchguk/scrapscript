namespace ScrapScript.Parse
{
    public class Assign : Object
    {
        public Assign(Var name, Object value)
        {
            Name = name;
            Value = value;
        }

        public Var Name { get; }
        public Object Value { get; }
    }
}
