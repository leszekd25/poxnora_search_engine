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
            if (!(o is Relic))
                return false;

            Relic r = (Relic)o;

            if (Flavor != r.Flavor)
                return false;
            if (Defense != r.Defense)
                return false;
            if (HitPoints != r.HitPoints)
                return false;
            if (Size != r.Size)
                return false;

            return base.Equals((Rune)o);
        }
    }
}
