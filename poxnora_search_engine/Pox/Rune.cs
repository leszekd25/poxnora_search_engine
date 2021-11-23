using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace poxnora_search_engine.Pox
{
    public class Rune: DataElement
    {
        public string Rarity = "";
        public string Hash = "";
        public string Artist = "";
        public List<string> Faction = new List<string>();
        public string Expansion = "";
        public bool ForSale;
        public bool Tradeable;
        public bool AllowRanked;
        public int DeckLimit;
        public int Cooldown;


        public void LoadFromJSON(JToken runedata)
        {
            if (runedata.SelectToken("id") != null)
                ID = runedata.SelectToken("id").ToObject<int>();

            if (runedata.SelectToken("name") != null)
                Name = runedata.SelectToken("name").ToObject<string>();

            if (runedata.SelectToken("description") != null)
                Description = runedata.SelectToken("description").ToObject<string>();

            if (runedata.SelectToken("rarity") != null)
            { 
                Rarity = runedata.SelectToken("rarity").ToObject<string>();
                Program.database.Rarities.AllowedStrings.Add(Rarity);
            }

            if (runedata.SelectToken("noraCost") != null)
                NoraCost = runedata.SelectToken("noraCost").ToObject<int>();

            if (runedata.SelectToken("hash") != null)
                Hash = runedata.SelectToken("hash").ToObject<string>();

            if (runedata.SelectToken("artist") != null)
                Artist = runedata.SelectToken("artist").ToObject<string>();

            if (runedata.SelectToken("factions") != null)
            {
                foreach (JToken f in runedata.SelectToken("factions"))
                {
                    Faction.Add(f.ToObject<string>());
                    Program.database.Factions.AllowedStrings.Add(f.ToObject<string>());
                }
            }

            if (runedata.SelectToken("runeSet") != null)
            {
                Expansion = runedata.SelectToken("runeSet").ToObject<string>();
                Program.database.Expansions.AllowedStrings.Add(Expansion);
            }

            if (runedata.SelectToken("forSale") != null)
                ForSale = runedata.SelectToken("forSale").ToObject<bool>();

            if (runedata.SelectToken("tradeable") != null)
                Tradeable = runedata.SelectToken("tradeable").ToObject<bool>();

            if (runedata.SelectToken("allowRanked") != null)
                AllowRanked = runedata.SelectToken("allowRanked").ToObject<bool>();

            if (runedata.SelectToken("deckLimit") != null)
                DeckLimit = runedata.SelectToken("deckLimit").ToObject<int>();

            if (runedata.SelectToken("cooldown") != null)
                Cooldown = runedata.SelectToken("cooldown").ToObject<int>();
        }

        public override bool GetIntFromDataPath(DataPath dp, out int result)
        {
            if(!base.GetIntFromDataPath(dp, out result))
            {
                if (dp == DataPath.DeckLimit)
                    result = DeckLimit;
                else if (dp == DataPath.Cooldown)
                    result = Cooldown;
                else if (dp == DataPath.ID)
                    result = ID;
                else if (dp == DataPath.NoraCost)
                    result = NoraCost;
                else
                    return false;
            }

            return true;
        }

        public override bool GetStringFromDataPath(DataPath dp, out string result)
        {
            if(!base.GetStringFromDataPath(dp, out result))
            {
                if (dp == DataPath.Artist)
                    result = Artist;
                else
                    return false;
            }

            return true;
        }

        public override bool GetEnumFromDataPath(DataPath dp, out string result)
        {
            if(!base.GetEnumFromDataPath(dp, out result))
            {
                if (dp == DataPath.Rarity)
                    result = Rarity;
                else if (dp == DataPath.Expansion)
                    result = Expansion;
                else
                    return false;
            }

            return true;
        }

        public override bool GetBoolFromDataPath(DataPath dp, out bool result)
        {
            if(!base.GetBoolFromDataPath(dp, out result))
            {
                if (dp == DataPath.ForSale)
                    result = ForSale;
                else if (dp == DataPath.Tradeable)
                    result = Tradeable;
                else if (dp == DataPath.AllowRanked)
                    result = AllowRanked;
                else
                    return false;
            }

            return true;
        }

        public override bool GetEnumListFromDataPath(DataPath dp, out List<string> result)
        {
            result = null;
            if (dp == DataPath.Faction)
            {
                result = Faction;
                return true;
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (!(o is Rune))
                return false;

            Rune r = (Rune)o;

            if (Rarity != r.Rarity)
                return false;
            if (Hash != r.Hash)
                return false;
            if (Artist != r.Artist)
                return false;
            foreach (string s in Faction)
                if (!(r.Faction.Contains(s)))
                    return false;
            if (Faction.Count != r.Faction.Count)
                return false;
            if (Expansion != r.Expansion)
                return false;
            if (ForSale != r.ForSale)
                return false;
            if (Tradeable != r.Tradeable)
                return false;
            if (AllowRanked != r.AllowRanked)
                return false;
            if (DeckLimit != r.DeckLimit)
                return false;
            if (Cooldown != r.Cooldown)
                return false;

            return base.Equals((DataElement)o);
        }
    }
}
