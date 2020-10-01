using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class Equipment: Rune
    {
        public string Flavor;

        public List<int> DescriptionAbilities = new List<int>();
        public List<string> DescriptionConditions = new List<string>();
        public List<Ability> DescriptionAbilities_refs = new List<Ability>();

        public override bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if (!base.GetStringFromDataPath(dp, out result))
            {
                if (dp == DataPath.FlavorText)
                    result = Flavor;
                else
                    return false;
            }

            return true;
        }

        public override bool Equals(object o)
        {
            if (!(o is Equipment))
                return false;

            Equipment e = (Equipment)o;

            if (Flavor != e.Flavor)
                return false;

            return base.Equals((Rune)o);
        }
    }
}
