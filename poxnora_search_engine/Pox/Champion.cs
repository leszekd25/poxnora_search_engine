using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{

    public class Champion: Rune
    {
        public int MaxRNG;
        public int MinRNG;
        public int Defense;
        public int Speed;
        public int Damage;
        public int HitPoints;
        public int Size;
        public List<string> Class = new List<string>();
        public List<string> Race = new List<string>();
        public List<int> Abilities = new List<int>();
        public List<int> Upgrade1 = new List<int>();
        public int DefaultUpgrade1Index;
        public List<int> Upgrade2 = new List<int>();
        public int DefaultUpgrade2Index;
        public int MinNoraCost;
        public int DefaultNoraCost;
        public int MaxNoraCost;

        public List<Ability> AllAbilities_refs = new List<Ability>();
        public List<Ability> BaseAbilities_refs = new List<Ability>();
        public List<Ability> AllUpgradeAbilities_refs = new List<Ability>();
        public List<Ability> UpgradeAbilities1_refs = new List<Ability>();
        public List<Ability> UpgradeAbilities2_refs = new List<Ability>();


        public override bool GetIntFromDataPath(DataPath dp, out int result)
        {
            if(!base.GetIntFromDataPath(dp, out result))
            {
                if (dp == DataPath.MaxRNG)
                    result = MaxRNG;
                else if (dp == DataPath.MinRNG)
                    result = MinRNG;
                else if (dp == DataPath.Defense)
                    result = Defense;
                else if (dp == DataPath.Speed)
                    result = Speed;
                else if (dp == DataPath.Damage)
                    result = Damage;
                else if (dp == DataPath.HitPoints)
                    result = HitPoints;
                else if (dp == DataPath.Size)
                    result = Size;
                else if (dp == DataPath.DefaultNoraCost)
                    result = DefaultNoraCost;
                else if (dp == DataPath.MinimumNoraCost)
                    result = MinNoraCost;
                else if (dp == DataPath.MaximumNoraCost)
                    result = MaxNoraCost;
                else
                    return false;
            }

            return true;
        }

        public override bool GetEnumListFromDataPath(DataPath dp, out List<string> result)
        {
            if (!base.GetEnumListFromDataPath(dp, out result))
            {
                if (dp == DataPath.Class)
                    result = Class;
                else if (dp == DataPath.Race)
                    result = Race;
                else
                    return false;
            }

            return true;
        }

        public override bool GetAbilityListFromDataPath(DataPath dp, out List<Ability> result)
        {
            if (dp == DataPath.AllAbilities)
                result = AllAbilities_refs;
            else if (dp == DataPath.BaseAbilities)
                result = BaseAbilities_refs;
            else if (dp == DataPath.UpgradeAbilities)
                result = AllUpgradeAbilities_refs;
            else
            {
                result = null;
                return false;
            }

            return true;
        }
    }
}
