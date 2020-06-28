using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum EnumListFilterType { CONTAINS = 0 }
    public class EnumListFilter : DataFilter
    {
        public EnumListFilterType FilterType { get; set; }
        public StringLibrary Options_ref;
        public string RefValue { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            List<string> val;
            bool applicable = obj.GetEnumListFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case EnumListFilterType.CONTAINS:
                        {
                            result = false;
                            foreach (string e in val)
                            {
                                if (e == RefValue)
                                {
                                    result = true;
                                    break;
                                }
                            }
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
            string base_str = (NegateResult ? "Negated: " : "") + "Faction ";
            if (FilterType == EnumListFilterType.CONTAINS)
                return base_str + "contains " + RefValue.ToString();
            return base_str + "UNKNOWN_OPERATOR" + RefValue.ToString();
        }
    }
}
