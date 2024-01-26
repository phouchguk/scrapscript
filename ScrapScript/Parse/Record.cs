using System.Collections.Generic;

namespace Scrapscript.Parse
{
    public class Record : Object
    {
        public Dictionary<string, Object> Data { get; set; }

        public Record()
        {
            this.Data = new Dictionary<string, Object>();
        }
    }
}
