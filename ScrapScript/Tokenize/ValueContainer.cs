namespace Scrapscript.Tokenize
{
    public abstract class ValueContainer<T> : Token
    {
        public T Value { get; set; }

        public ValueContainer(T value)
        {
            this.Value = value;
        }
    }
}
