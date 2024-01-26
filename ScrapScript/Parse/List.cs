using System.Collections.Generic;

namespace Scrapscript.Parse
{
    public class List : Object
    {
        public List(IEnumerable<Object> items)
        {
            Items = items;
        }

        public IEnumerable<Object> Items { get; }
    }
}
