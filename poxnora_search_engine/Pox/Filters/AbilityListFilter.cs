using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox.Filters
{
    public enum AbilityListFilterType { CONTAINS, CONTAINS_SIMILAR }
    public class AbilityListFilter: DataFilter
    {
        public AbilityListFilterType FilterType { get; set; }
        public int RefValue { get; set; }

        public override bool Satisfies(DataElement obj)
        {
            bool result;

            List<Ability> val;
            bool applicable = obj.GetAbilityListFromDataPath(dpath, out val);
            if (!applicable)
            {
                result = false;
            }
            else
            {
                switch (FilterType)
                {
                    case AbilityListFilterType.CONTAINS:
                        {
                            result = false;
                            foreach (Ability e in val)
                            {
                                if (e.ID == RefValue)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case AbilityListFilterType.CONTAINS_SIMILAR:
                        {
                            result = false;
                            foreach(int ability_id in Program.database.Abilities_similar[Program.database.Abilities[RefValue].Name])
                            {
                                foreach(Ability e in val)
                                {
                                    if(e.ID == ability_id)
                                    {
                                        result = true;
                                        break;
                                    }
                                }
                                if (result)
                                    break;
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
            string base_str = (NegateResult ? "Negated: " : "") + dpath.ToString() + " ";
            if (FilterType == AbilityListFilterType.CONTAINS)
                return base_str + "contains " + Program.database.Abilities[RefValue].ToString();
            if (FilterType == AbilityListFilterType.CONTAINS_SIMILAR)
                return base_str + "contains similar to " + Program.database.Abilities[RefValue].ToString();
            return base_str + "UNKNOWN_OPERATOR" + Program.database.Abilities[RefValue].ToString();
        }
    }
}
