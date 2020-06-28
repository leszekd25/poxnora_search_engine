using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum BooleanFilterType { EQUAL = 0 }

    public class BooleanFilter : DataFilter
    {
        public BooleanFilterType FilterType { get; set; }
        public bool RefValue { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            bool val;
            bool applicable = obj.GetBoolFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case BooleanFilterType.EQUAL:
                        result = val == RefValue;
                        break;
                    default:
                        result = false;
                        break;
                }
            }

            return result ^ NegateResult;
        }

        public override string ToString()
        {
            string base_str = (NegateResult ? "Negated: " : "") + dpath.ToString() + " ";
            if (FilterType == BooleanFilterType.EQUAL)
                return base_str + "is equal to " + RefValue.ToString();
            return base_str + "UNKNOWN_OPERATOR" + RefValue.ToString();
        }
    }
}
