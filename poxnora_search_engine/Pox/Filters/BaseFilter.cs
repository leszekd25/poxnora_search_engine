using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public class BaseFilter
    {
        public string Name = "Base filter";
        public bool NegateResult { get; set; }
        public virtual bool Satisfies(DataElement obj)
        {
            return !NegateResult;
        }

        public void Apply(ref List<DataElement> input, ref List<DataElement> output)
        {
            foreach (var d in input)
                if (Satisfies(d))
                    output.Add(d);
        }

        public override string ToString()
        {
            return (NegateResult ? "Negated: " : "") + "Everything";
        }
    }
}
