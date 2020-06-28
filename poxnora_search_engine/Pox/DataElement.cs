using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class DataElement
    {
        public int ID;
        public string Name = "";
        public string Description = "";
        public int NoraCost;         // base nora cost for the champion, without upgrades

        public override string ToString()
        {
            return Name;
        }

        public virtual bool GetIntFromDataPath(DataPath dp, out int result)
        {
            if (dp == DataPath.ID)
                result = ID;
            else if (dp == DataPath.NoraCost)
                result = NoraCost;
            else
            {
                result = 0;
                return false;
            }
            return true;
        }

        public virtual bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if (dp == DataPath.Name)
                result = Name;
            else if (dp == DataPath.Description)
                result = Description;
            else
            {
                result = "";
                return false;
            }
            return true;
        }

        public virtual bool GetEnumFromDataPath(DataPath dp, out string result)
        {
            result = "";
            return false;
        }

        public virtual bool GetBoolFromDataPath(DataPath dp, out bool result)
        {
            result = false;
            return false;
        }

        public virtual bool GetEnumListFromDataPath(DataPath dp, out List<string> result)
        { 
            result = null;
            return false;
        }

        public virtual bool GetAbilityListFromDataPath(DataPath dp, out List<Ability> result)
        {
            result = null;
            return false;
        }
    }
}
