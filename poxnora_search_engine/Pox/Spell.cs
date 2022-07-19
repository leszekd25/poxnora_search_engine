using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class Spell: Rune
    {
        public string Flavor = "";

        public override bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if(!base.GetStringFromDataPath(dp, out result))
            {
                if (dp == DataPath.FlavorText)
                    result = Flavor;
                else
                    return false;
            }

            return true;
        }
        public override bool GetEnumListFromDataPath(DataPath dp, out List<string> result)
        {
            if (!base.GetEnumListFromDataPath(dp, out result))
            {
                if (dp == DataPath.Keyword)
                    result = DescriptionKeywords;
                else
                    return false;
            }

            return true;
        }

        public override bool Equals(object o)
        {
            if (!(o is Spell))
                return false;

            Spell s = (Spell)o;

            if (Flavor != s.Flavor)
                return false;

            return base.Equals((Rune)o);
        }
    }
}
