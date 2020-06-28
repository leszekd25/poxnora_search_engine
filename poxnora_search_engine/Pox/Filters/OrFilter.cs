using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public class OrFilter: BaseFilter
    {
        public List<BaseFilter> Filters { get; } = new List<BaseFilter>();

        public override bool Satisfies(DataElement obj)
        {
            bool result = false;
            foreach (BaseFilter f in Filters)
            {
                if (f.Satisfies(obj))
                {
                    result = true;
                    break;
                }
            }
            return result ^ NegateResult;
        }

        public override string ToString()
        {
            return (NegateResult ? "Negated: " : "") + "Match any filter";
        }
    }
}
