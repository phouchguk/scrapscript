using System.Collections.Generic;

namespace Scrapscript.Parse
{
    public class MatchFunction : Object
    {
        public MatchFunction(IEnumerable<MatchCase> cases)
        {
            Cases = cases;
        }

        public IEnumerable<MatchCase> Cases { get; }
    }
}
