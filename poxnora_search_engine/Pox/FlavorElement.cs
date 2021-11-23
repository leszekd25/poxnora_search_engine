using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace poxnora_search_engine.Pox
{
    public class FlavorElement: DataElement
    {
        public string Key = "";

        public List<int> DescriptionAbilities = new List<int>();
        public List<string> DescriptionConditions = new List<string>();
        public List<Ability> DescriptionAbilities_refs = new List<Ability>();

        public void LoadFromJSON(JToken token)
        {
            if (token.SelectToken("key") != null)
                Key = token.SelectToken("key").ToObject<string>();

            if (token.SelectToken("name") != null)
                Name = token.SelectToken("name").ToObject<string>();

            if (token.SelectToken("description") != null)
                Description = token.SelectToken("description").ToObject<string>();
        }


        public override bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if (dp == DataPath.Key)
                result = Key;
            else
            {
                result = "";
                return false;
            }
            return true;
        }

        public override bool Equals(object o)
        {
            if (!(o is FlavorElement))
                return false;

            FlavorElement d = (FlavorElement)o;
            if (Key != d.Key)
                return false;

            return base.Equals((DataElement)o);
        }
    }
}
