using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum EnumFilterType { EQUAL = 0 }
    public class EnumFilter: DataFilter
    {
        public EnumFilterType FilterType { get; set; }
        public StringLibrary Options_ref;
        public string RefValue { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            string val;
            bool applicable = obj.GetEnumFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case EnumFilterType.EQUAL:
                        result = val.Equals(RefValue);
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
            if (FilterType == EnumFilterType.EQUAL)
                return base_str + "is " + RefValue.ToString();
            return base_str + "UNKNOWN_OPERATOR" + RefValue.ToString();
        }
    }
}
