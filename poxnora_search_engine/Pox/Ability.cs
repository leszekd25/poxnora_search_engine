using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class Ability: DataElement, IEquatable<Ability>, IComparable<Ability>
    {
        public int APCost;
        public int ActivationType;
        public int Level;
        public int Cooldown;
        public string IconName = "";

        public int AssetID;
        public int PrerequisiteID;
        public int Revision;
        public bool Activated;
        public bool Resettable;
        public bool Ranked;
        public string Comments = "";
        public string ClassName = "";

        public int UseCount;

        public override bool GetIntFromDataPath(DataPath dp, out int result)
        {
            if (!base.GetIntFromDataPath(dp, out result))
            {
                if (dp == DataPath.APCost)
                    result = APCost;
                else if (dp == DataPath.Level)
                    result = Level;
                else if (dp == DataPath.Cooldown)
                    result = Cooldown;
                else if (dp == DataPath.ActivationType)
                    result = ActivationType;
                else if (dp == DataPath.AssetID)
                    result = AssetID;
                else if (dp == DataPath.PrerequisiteID)
                    result = PrerequisiteID;
                else if (dp == DataPath.Revision)
                    result = Revision;
                else if (dp == DataPath.UseCount)
                    result = UseCount;
                else
                    return false;
            }

            return true;
        }

        public override bool GetBoolFromDataPath(DataPath dp, out bool result)
        {
            if(!base.GetBoolFromDataPath(dp, out result))
            {
                if (dp == DataPath.Activated)
                    result = Activated;
                else if (dp == DataPath.Resettable)
                    result = Resettable;
                else if (dp == DataPath.Ranked)
                    result = Ranked;
                else
                    return false;
            }

            return true;
        }

        public override bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if (!base.GetStringFromDataPath(dp, out result))
            {
                if (dp == DataPath.Comments)
                    result = Comments;
                else if (dp == DataPath.ClassName)
                    result = ClassName;
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

        public override string ToString()
        {
            if (Level > 0)
                return Name + " " + Level.ToString();
            else
                return Name;
        }

        public override bool Equals(object o)
        {
            if (!(o is Ability))
                return false;

            Ability a = (Ability)o;

            if (APCost != a.APCost)
                return false;
            if (ActivationType != a.ActivationType)
                return false;
            if (Level != a.Level)
                return false;
            if (Cooldown != a.Cooldown)
                return false;
            if (IconName != a.IconName)
                return false;

            return base.Equals((DataElement)o);
        }

        public bool Equals(Ability other)
        {
            if (APCost != other.APCost)
                return false;
            if (ActivationType != other.ActivationType)
                return false;
            if (Level != other.Level)
                return false;
            if (Cooldown != other.Cooldown)
                return false;
            if (IconName != other.IconName)
                return false;

            return base.Equals((DataElement)other);
        }

        // for sorting
        public int CompareTo(Ability comparePart)
        {
            return ToString().CompareTo(comparePart.ToString());
        }
    }
}
