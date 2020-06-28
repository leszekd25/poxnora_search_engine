using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum StringFilterType { EQUAL = 0, CONTAINS }
    public class StringFilter: DataFilter
    {
        public StringFilterType FilterType { get; set; }

        private string refvalue;
        public string RefValue { get { return refvalue; } set { refvalue = value; RefValueLowerCase = value.ToLower(); } }
        public string RefValueLowerCase;

        public bool IgnoreCase { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            string val;
            bool applicable = obj.GetStringFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case StringFilterType.EQUAL:
                        {
                            if (IgnoreCase)
                                result = val.ToLower() == RefValueLowerCase;
                            else
                                result = val == RefValue;
                        }
                        break;
                    case StringFilterType.CONTAINS:
                        {
                            if (IgnoreCase)
                                result = (val.ToLower()).Contains(RefValueLowerCase);
                            else
                                result = val.Contains(RefValue);
                        }
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
            if (FilterType == StringFilterType.EQUAL)
                return base_str + "is " + RefValue.ToString();
            if (FilterType == StringFilterType.CONTAINS)
                return base_str + "contains " + RefValue.ToString();
            return base_str + "UNKNOWN_OPERATOR" + RefValue.ToString();
        }
    }
}
