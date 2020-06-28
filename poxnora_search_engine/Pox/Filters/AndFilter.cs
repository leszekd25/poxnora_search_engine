using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public class AndFilter: BaseFilter
    {
        public List<BaseFilter> Filters { get; } = new List<BaseFilter>();

        public override bool Satisfies(DataElement obj)
        {
            bool result = true;
            foreach(BaseFilter f in Filters)
            {
                if(!f.Satisfies(obj))
                {
                    result = false;
                    break;
                }
            }
            return result ^ NegateResult;
        }

        public override string ToString()
        {
            return (NegateResult ? "Negated: " : "") + "Match all filters";
        }
    }
}
