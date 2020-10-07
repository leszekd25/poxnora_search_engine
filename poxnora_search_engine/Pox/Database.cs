using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace poxnora_search_engine.Pox
{
    public enum DataPath
    {
        // INT
        ID = 0,
        NoraCost = 1,
        DeckLimit = 2,
        MaxRNG = 3,
        MinRNG = 4,
        Defense = 5,
        Speed = 6,
        Damage = 7,
        HitPoints = 8,
        Size = 9,
        APCost = 10,
        Level = 11,
        Cooldown = 12,
        DefaultNoraCost = 13,
        MinimumNoraCost = 14,
        MaximumNoraCost = 15,
        BaseNoraCost = 16,
        PrognosedBaseNoraCost = 17,
        PrognosedBaseNoraCostDifference = 18,

        // STRING
        Name = 100,
        Description = 101,
        Artist = 102,
        FlavorText = 103,

        // ENUMS
        Rarity = 200,
        Expansion = 201,

        // BOOLEANS
        ForSale = 300,
        Tradeable = 301,
        AllowRanked = 302,

        // LIST (ENUM)
        Faction = 400,
        Class = 401,
        Race = 402,

        // LIST (ABILITY)
        AllAbilities = 500,
        BaseAbilities = 501,
        UpgradeAbilities = 502,

        // NONE
        None = -1
    }

    public delegate void OnDatabaseReady();

    public class Database
    {

        public const string POXNORA_JSON_SITE = "https://www.poxnora.com/api/feed.do?t=json";

        System.Net.WebClient wc;
        public OnDatabaseReady ready_trigger;

        JObject json_main = null;
        public Dictionary<int, Champion> Champions { get; } = new Dictionary<int, Champion>();
        public Dictionary<int, Ability> Abilities { get; } = new Dictionary<int, Ability>();
        public Dictionary<string, List<int>> Abilities_similar { get; } = new Dictionary<string, List<int>>();
        public Dictionary<int, Spell> Spells { get; } = new Dictionary<int, Spell>();
        public Dictionary<int, Relic> Relics { get; } = new Dictionary<int, Relic>();
        public Dictionary<int, Equipment> Equipments { get; } = new Dictionary<int, Equipment>();

        public StringLibrary Factions { get; } = new StringLibrary();
        public StringLibrary Races { get; } = new StringLibrary();
        public StringLibrary Classes { get; } = new StringLibrary();
        public StringLibrary Rarities { get; } = new StringLibrary();
        public StringLibrary Expansions { get; } = new StringLibrary();
        public StringLibrary AbilityNames { get; } = new StringLibrary();


        public bool ready { get; private set; } = false;

        public void LoadJSON(string local_db, string online_backup)
        {
            if (!File.Exists(local_db))//"database.json"))
            {
                if (online_backup == "")
                    return;

                Log.Info("Database.LoadJSON(): database.json not found, retrieving from server...");

                wc = new System.Net.WebClient();
                wc.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(RetrieveJSON_completed);

                Uri dw_string = new Uri(online_backup);//POXNORA_JSON_SITE);
                wc.DownloadStringAsync(dw_string);
            }
            else
            {
                if(online_backup != "")    // first load
                    Log.Info("Database.LoadJSON(): database.json found, loading...");

                string json = File.ReadAllText(local_db);//"database.json");

                ParseFromJSON(json);
            }
        }

        void RetrieveJSON_completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            wc.DownloadStringCompleted -= new System.Net.DownloadStringCompletedEventHandler(RetrieveJSON_completed);
            wc = null;

            if (e.Cancelled)
            {
                Log.Error("Database.RetrieveJSON_completed(): Could not retrieve JSON");
                return;
            }
            if (e.Error != null)
            {
                Log.Error("Database.RetrieveJSON_completed(): Error while retrieving JSON");
                return;
            }

            File.WriteAllText("database.json", e.Result);

            Log.Info("Database.RetrieveJSON_completed(): database.json created, loading...");

            ParseFromJSON(e.Result);
        }

        void ParseFromJSON(string json)
        {
            try
            {
                json_main = JObject.Parse(json);
            }
            catch (Exception)
            {
                Log.Error("Database.ParseFromJSON(): Input JSON is invalid!");
                return;
            }

            try
            {
                JToken champs = json_main.SelectToken("champs");
                foreach (JToken champ in champs.Children())
                    AddChampionFromJSON(champ);

                JToken spells = json_main.SelectToken("spells");
                foreach (JToken spell in spells.Children())
                    AddSpellFromJSON(spell);

                JToken relics = json_main.SelectToken("relics");
                foreach (JToken relic in relics.Children())
                    AddRelicFromJSON(relic);

                JToken equips = json_main.SelectToken("equips");
                foreach (JToken equip in equips.Children())
                    AddEquipmentFromJSON(equip);

                foreach (Champion c in Champions.Values)
                    SetupChampionAbilities(c);

                Abilities.Add(0, new Ability() { Name = "<INVALID_ABILITY>" });

                ResolveSimilarAbilities();

                Log.Info("Champs loaded: " + Champions.Count + ", Abilities loaded: " + Abilities.Count + ", Spells loaded: " + Spells.Count + ", Relics loaded: " + Relics.Count + ", Equipments loaded: " + Equipments.Count);
            }
            catch (Exception e)
            {
                Log.Error("Database.RetrieveJSON_completed(): Error while parsing the JSON");
                return;
            }
            finally
            {
                json_main = null;
            }

            ready = true;
            if(ready_trigger != null)
                ready_trigger();
        }

        void AddChampionFromJSON(JToken champ)
        {
            Champion c = new Champion();
            c.LoadFromJSON(champ);

            if (champ.SelectToken("maxRng") != null)
                c.MaxRNG = champ.SelectToken("maxRng").ToObject<int>();

            if (champ.SelectToken("minRng") != null)
                c.MinRNG = champ.SelectToken("minRng").ToObject<int>();

            if (champ.SelectToken("defense") != null)
                c.Defense = champ.SelectToken("defense").ToObject<int>();

            if (champ.SelectToken("speed") != null)
                c.Speed = champ.SelectToken("speed").ToObject<int>();

            if (champ.SelectToken("damage") != null)
                c.Damage = champ.SelectToken("damage").ToObject<int>();

            if (champ.SelectToken("hitPoints") != null)
                c.HitPoints = champ.SelectToken("hitPoints").ToObject<int>();

            if (champ.SelectToken("size") != null)
                c.Size = ((champ.SelectToken("size").ToObject<string>())[0] == '2'? 2: 1);

            if (champ.SelectToken("classes") != null)
            {
                foreach (JToken cl in champ.SelectToken("classes"))
                {
                    c.Class.Add(cl.ToObject<string>());
                    Classes.AllowedStrings.Add(cl.ToObject<string>());
                }
            }

            if (champ.SelectToken("races") != null)
            {
                foreach (JToken r in champ.SelectToken("races"))
                {
                    c.Race.Add(r.ToObject<string>());
                    Races.AllowedStrings.Add(r.ToObject<string>());
                }
            }

            if(champ.SelectToken("startingAbilities") != null)
            {
                JToken abs = champ.SelectToken("startingAbilities");
                foreach (JToken ab in abs.Children())
                {
                    Ability a = AddAbilityFromJSON(ab);
                    if (a != null)
                        c.Abilities.Add(a.ID);
                }
            }

            if(champ.SelectToken("abilitySets") != null)
            {
                JToken absets = champ.SelectToken("abilitySets");

                JToken abset = absets.First;
                if (abset != null)
                {
                    if (abset.SelectToken("abilities") != null)
                    {
                        JToken abs = abset.SelectToken("abilities");
                        int cur_ab_index = 0;
                        foreach (JToken ab in abs.Children())
                        {
                            if (ab.SelectToken("default") != null)
                            {
                                if (ab.SelectToken("default").ToObject<bool>())
                                    c.DefaultUpgrade1Index = cur_ab_index;
                            }
                            Ability a = AddAbilityFromJSON(ab);
                            if (a != null)
                                c.Upgrade1.Add(a.ID);

                            cur_ab_index += 1;
                        }
                    }
                }

                abset = abset.Next;
                if (abset != null)
                {
                    if (abset.SelectToken("abilities") != null)
                    {
                        JToken abs = abset.SelectToken("abilities");
                        int cur_ab_index = 0;
                        foreach (JToken ab in abs.Children())
                        {
                            if (ab.SelectToken("default") != null)
                            {
                                if (ab.SelectToken("default").ToObject<bool>())
                                    c.DefaultUpgrade2Index = cur_ab_index;
                            }
                            Ability a = AddAbilityFromJSON(ab);
                            if (a != null)
                                c.Upgrade2.Add(a.ID);

                            cur_ab_index += 1;
                        }
                    }
                }
            }

            // set up nora costs
            c.DefaultNoraCost = c.NoraCost;
            c.NoraCost -= Abilities[c.Upgrade1[c.DefaultUpgrade1Index]].NoraCost;
            c.NoraCost -= Abilities[c.Upgrade2[c.DefaultUpgrade2Index]].NoraCost;
            // find min and max nora cost among upgrade 1 and 2
            int minupg1 = Abilities[c.Upgrade1[0]].NoraCost;
            int minupg2 = Abilities[c.Upgrade2[0]].NoraCost;
            int maxupg1 = Abilities[c.Upgrade1[0]].NoraCost;
            int maxupg2 = Abilities[c.Upgrade2[0]].NoraCost;
            for (int i = 1; i < c.Upgrade1.Count; i++)
            {
                if (Abilities[c.Upgrade1[i]].NoraCost < minupg1)
                    minupg1 = Abilities[c.Upgrade1[i]].NoraCost;
                if (Abilities[c.Upgrade1[i]].NoraCost > maxupg1)
                    maxupg1 = Abilities[c.Upgrade1[i]].NoraCost;
            }
            for (int i = 1; i < c.Upgrade2.Count; i++)
            {
                if (Abilities[c.Upgrade2[i]].NoraCost < minupg2)
                    minupg2 = Abilities[c.Upgrade2[i]].NoraCost;
                if (Abilities[c.Upgrade2[i]].NoraCost > maxupg2)
                    maxupg2 = Abilities[c.Upgrade2[i]].NoraCost;
            }
            c.MinNoraCost = c.NoraCost + minupg1 + minupg2;
            c.MaxNoraCost = c.NoraCost + maxupg1 + maxupg2;
            // find base cost without ANY abilities
            int basecost = c.NoraCost;
            foreach (var ab in c.Abilities)
                basecost -= Abilities[ab].NoraCost;
            c.BaseNoraCost = basecost;

            Champions.Add(c.ID, c);
        }

        Ability AddAbilityFromJSON(JToken ability)
        {
            if (ability.SelectToken("id") == null)
                return null;

            if (Abilities.ContainsKey(ability.SelectToken("id").ToObject<int>()))
                return Abilities[ability.SelectToken("id").ToObject<int>()];

            Ability a = new Ability();

            if (ability.SelectToken("id") != null)
                a.ID = ability.SelectToken("id").ToObject<int>();

            if (ability.SelectToken("name") != null)
                a.Name = ability.SelectToken("name").ToObject<string>();

            if (ability.SelectToken("shortDescription") != null)
                a.Description = ability.SelectToken("shortDescription").ToObject<string>();

            if (ability.SelectToken("activationType") != null)
                a.ActivationType = ability.SelectToken("activationType").ToObject<int>();

            if (ability.SelectToken("noraCost") != null)
                a.NoraCost = ability.SelectToken("noraCost").ToObject<int>();

            if (ability.SelectToken("apCost") != null)
                a.APCost = ability.SelectToken("apCost").ToObject<int>();

            if (ability.SelectToken("level") != null)
                a.Level = ability.SelectToken("level").ToObject<int>();

            if (ability.SelectToken("cooldown") != null)
                a.Cooldown = ability.SelectToken("cooldown").ToObject<int>();

            if (ability.SelectToken("iconName") != null)
                a.IconName = ability.SelectToken("iconName").ToObject<string>();

            a.Description = ExtractAbilitiesAndConditions(a.Description, ref a.DescriptionAbilities, ref a.DescriptionConditions);

            Abilities.Add(a.ID, a);
            AbilityNames.AllowedStrings.Add(a.ToString());
            return a;
        }

        void AddSpellFromJSON(JToken spell)
        {
            Spell s = new Spell();
            s.LoadFromJSON(spell);

            if (spell.SelectToken("flavorText") != null)
                s.Flavor = spell.SelectToken("flavorText").ToObject<string>();

            s.Description = ExtractAbilitiesAndConditions(s.Description, ref s.DescriptionAbilities, ref s.DescriptionConditions);

            Spells.Add(s.ID, s);
        }

        void AddRelicFromJSON(JToken relic)
        {
            Relic r = new Relic();
            r.LoadFromJSON(relic);

            if (relic.SelectToken("flavorText") != null)
                r.Flavor = relic.SelectToken("flavorText").ToObject<string>();

            if (relic.SelectToken("defense") != null)
                r.Defense = relic.SelectToken("defense").ToObject<int>();

            if (relic.SelectToken("hitPoints") != null)
                r.HitPoints = relic.SelectToken("hitPoints").ToObject<int>();

            if (relic.SelectToken("size") != null)
                r.Size = ((relic.SelectToken("size").ToObject<string>())[0] == '2' ? 2 : 1);

            r.Description = ExtractAbilitiesAndConditions(r.Description, ref r.DescriptionAbilities, ref r.DescriptionConditions);

            Relics.Add(r.ID, r);
        }

        void AddEquipmentFromJSON(JToken equip)
        {
            Equipment e = new Equipment();
            e.LoadFromJSON(equip);

            if (equip.SelectToken("flavorText") != null)
                e.Flavor = equip.SelectToken("flavorText").ToObject<string>();

            e.Description = ExtractAbilitiesAndConditions(e.Description, ref e.DescriptionAbilities, ref e.DescriptionConditions);

            Equipments.Add(e.ID, e);
        }

        void SetupChampionAbilities(Champion c)
        {
            foreach(int a in c.Abilities)
            {
                c.BaseAbilities_refs.Add(Abilities[a]);
                c.AllAbilities_refs.Add(Abilities[a]);
            }
            foreach (int a in c.Upgrade1)
            {
                c.UpgradeAbilities1_refs.Add(Abilities[a]);
                c.AllUpgradeAbilities_refs.Add(Abilities[a]);
                c.AllAbilities_refs.Add(Abilities[a]);
            }
            foreach (int a in c.Upgrade2)
            {
                c.UpgradeAbilities2_refs.Add(Abilities[a]);
                c.AllUpgradeAbilities_refs.Add(Abilities[a]);
                c.AllAbilities_refs.Add(Abilities[a]);
            }

            c.CalculatePrognosedBaseNoraCost();
            c.PrognosedBaseNoraCostDifference = c.BaseNoraCost - c.PrognosedBaseNoraCost;
        }

        /*void SetupAbilityAbilities(Ability ab)
        {
            foreach (int a in ab.DescriptionAbilities)
                ab.DescriptionAbilities_refs.Add(Abilities[a]);
        }

        void SetupSpellAbilities(Spell s)
        {
            foreach (int a in s.DescriptionAbilities)
                s.DescriptionAbilities_refs.Add(Abilities[a]);
        }

        void SetupRelicAbilities(Relic r)
        {
            foreach (int a in r.DescriptionAbilities)
                r.DescriptionAbilities_refs.Add(Abilities[a]);
        }

        void SetupEquipmentAbilities(Equipment e)
        {
            foreach (int a in e.DescriptionAbilities)
                e.DescriptionAbilities_refs.Add(Abilities[a]);
        }*/

        void ResolveSimilarAbilities()
        {
            foreach(var kv in Abilities)
            {
                if (!Abilities_similar.ContainsKey(kv.Value.Name))
                    Abilities_similar.Add(kv.Value.Name, new List<int>());

                Abilities_similar[kv.Value.Name].Add(kv.Key);
            }
        }

        public string ExtractAbilitiesAndConditions(string description, ref List<int> abs, ref List<string> cons)
        {
            // if it was done before, dont do it again
            if((abs.Count > 0)||(cons.Count > 0))
                return description;

            List<Tuple<int, int>> ranges = new List<Tuple<int, int>>();
            // step 1: extract abilities/conditions
            for(int i = 0; i < description.Length; i++)
            {
                if(description[i] == '<')
                {
                    int tags_found = 0;
                    // find '>'
                    for(int j = i+1; j < description.Length; j++)
                    {
                        if(description[j] == '>')
                        {
                            tags_found += 1;
                            if(tags_found == 2)
                            {
                                ranges.Add(new Tuple<int, int>(i, j));
                                break;
                            }
                        }
                    }

                    // extract ability/condition from the string
                    if (description[i + 1] == 'a')
                        abs.Add(ExtractAbility(description, ranges[ranges.Count - 1]));
                    else if (description[i + 1] == 'c')
                        cons.Add(ExtractCondition(description, ranges[ranges.Count - 1]));

                    i = ranges[ranges.Count - 1].Item2;
                }
            }

            // step 2: remove tags
            int offset = 0;
            foreach (var range in ranges)
            {
                int cur_l = description.Length;
                description = RemoveTags(description, range, offset);
                cur_l = cur_l - description.Length;
                offset += cur_l;
            }

            return description;
        }

        int ExtractAbility(string desc, Tuple<int, int> range)
        {
            int ab_start = range.Item1 - 1;
            int ab_end = range.Item2 + 1;

            for (int i = range.Item1; i <= range.Item2; i++)
            {
                if (desc[i] == '=')
                {
                    ab_start = i+1;
                }
                if (desc[i] == '>')
                {
                    ab_end = i-1;
                    break;
                }
            }

            string ab_str = desc.Substring(ab_start, ab_end - ab_start + 1);
            int ret = -1;
            int.TryParse(ab_str, out ret);
            return ret;
        }

        string ExtractCondition(string desc, Tuple<int, int> range)
        {
            int con_start = range.Item1 - 1;
            int con_end = range.Item2 + 1;

            for (int i = range.Item1; i <= range.Item2; i++)
            {
                if (desc[i] == '=')
                {
                    con_start = i + 1;
                }
                if (desc[i] == '>')
                {
                    con_end = i - 1;
                    break;
                }
            }

            string con_str = desc.Substring(con_start, con_end  - con_start + 1);
            return con_str;
        }

        string RemoveTags(string desc, Tuple<int, int> range, int offset)
        {
            range = new Tuple<int, int>(range.Item1 - offset, range.Item2 - offset);

            int tag1_end = range.Item1 - 1;
            int tag2_start = range.Item2 + 1;
            bool found_inner_text = false;

            for (int i = range.Item1; i <= range.Item2; i++)
            {
                if(desc[i] == '>')
                {
                    tag1_end = i;
                    found_inner_text = true;
                }
                if((desc[i] == '<')&&(found_inner_text))
                {
                    tag2_start = i;
                    break;
                }
            }

            string inner_text = desc.Substring(tag1_end + 1, tag2_start - tag1_end - 1);

            return desc.Remove(range.Item1, range.Item2 - range.Item1 + 1)
                       .Insert(range.Item1, inner_text);
        }

        public int GetAbilityIDByName(string name)
        {
            foreach (var kv in Abilities)
                if (kv.Value.ToString() == name)
                    return kv.Key;

            return 0;
        }
    }
}
