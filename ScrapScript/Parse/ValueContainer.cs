namespace Scrapscript.Parse
{
    public class ValueContainer<T> : Object
    {
        public ValueContainer(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
