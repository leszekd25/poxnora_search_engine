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
        public int BaseNoraCost;
        public int PrognosedBaseNoraCost;
        public int PrognosedBaseNoraCostDifference;

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
                else if (dp == DataPath.BaseNoraCost)
                    result = BaseNoraCost;
                else if (dp == DataPath.PrognosedBaseNoraCost)
                    result = PrognosedBaseNoraCost;
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

        private int CalculateHPDefenseNoraCost()
        {
            int eff_hp = (int)(HitPoints * (10f / (10 - Defense)));
            return eff_hp / 2;
        }

        private static int[] MaxRangeNoraCostTable = new int[]
        {
            0, 2, 4, 6, 8, 10, 14, 20, 28, 38, 50, 64
        };

        private static float[] RangeDifferenceNoraCostCoefficientTable = new float[]
        {
            0f, 1f, 2.5f, 4.5f, 7f, 9f, 11.5f
        };

        private int CalculateRangeNoraCost()
        {
            int rngdiff_bonus = (int)(RangeDifferenceNoraCostCoefficientTable[MaxRNG - MinRNG] * MinRNG);

            return 0                         // min range cost
                 + MaxRangeNoraCostTable[MaxRNG-1]                // max range cost
                                                                //+ (MaxRNG >= 5 ? (MaxRNG - 5) * 2 : 0)                     // bonus if range exceeds 5
                 + rngdiff_bonus;     // range difference cost
                 //+ (((MaxRNG > 1) && (MinRNG == 1)) ? 2 : 0);               // bonus if both ranged and melee
        }

        private static int[] DamageNoraCostTable = new int[]
        {
            -15, -11, -8, -6,   // 3
            -4, -3, -2, -1,
            0, 1, 2, 4,       // 11
            6, 8, 10, 12,
            14, 16, 18, 20,   // 19
            22, 24, 26, 28    // 23
        };

        private int CalculateDamageNoraCost()
        {
            float coeff = Speed / 4f;
            return (int)(DamageNoraCostTable[Damage] * coeff);
        }

        private static int[] SpeedNoraCostTable = new int[]
        {
            0,           // 0
            0,
            0,
            2,
            6,
            10,
            14,
            19,
            25,           // 8
            32
        };

        private int CalculateSpeedNoraCost()
        {
            return SpeedNoraCostTable[Speed];
        }

        private int CalculateSizeNoraCost()
        {
            return (Size == 1 ? 0 : -5);
        }

        private int CalculateAbilityNoraCostModifiers()
        {
            int result = 0;

            
            return result;
        }

        // this is called after ability references are set up
        public void CalculatePrognosedBaseNoraCost()
        {
            int result = 0;

            result += CalculateHPDefenseNoraCost();
            result += CalculateSpeedNoraCost();
            result += CalculateSizeNoraCost();
            result += CalculateRangeNoraCost();
            result += CalculateDamageNoraCost();
            result += CalculateAbilityNoraCostModifiers();

            PrognosedBaseNoraCost = result;
        }
    }
}
