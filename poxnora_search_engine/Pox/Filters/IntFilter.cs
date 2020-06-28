using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum IntFilterType { EQUAL = 0, GREATER_THAN, LESS_THAN }

    public class IntFilter: DataFilter
    {
        public IntFilterType FilterType { get; set; }
        public int RefValue { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            int val;
            bool applicable = obj.GetIntFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case IntFilterType.EQUAL:
                        result = val == RefValue;
                        break;
                    case IntFilterType.GREATER_THAN:
                        result = val > RefValue;
                        break;
                    case IntFilterType.LESS_THAN:
                        result = val < RefValue;
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
            string base_str = (NegateResult ? "Negated: " : "") + dpath.ToString()+" ";
            if (FilterType == IntFilterType.EQUAL)
                return base_str + "is equal to " + RefValue.ToString();
            if (FilterType == IntFilterType.GREATER_THAN)
                return base_str + "is greater than " + RefValue.ToString();
            if (FilterType == IntFilterType.LESS_THAN)
                return base_str + "is less than " + RefValue.ToString();
            return base_str + "UNKNOWN_OPERATOR" + RefValue.ToString();
        }
    }
}
