using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class Relic: Rune
    {
        public string Flavor = "";
        public int Defense;
        public int HitPoints;
        public int Size;

        public List<int> DescriptionAbilities = new List<int>();
        public List<string> DescriptionConditions = new List<string>();

        public override bool GetIntFromDataPath(DataPath dp, out int result)
        {
            if(!base.GetIntFromDataPath(dp, out result))
            {
                if (dp == DataPath.Defense)
                    result = Defense;
                else if (dp == DataPath.HitPoints)
                    result = HitPoints;
                else if (dp == DataPath.Size)
                    result = Size;
                else
                    return false;
            }

            return true;
        }

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
    }
}
