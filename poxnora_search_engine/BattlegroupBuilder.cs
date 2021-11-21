using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using poxnora_search_engine.Pox;

namespace poxnora_search_engine
{
    public partial class BattlegroupBuilder : Form
    {
        public struct ChampionBG
        {
            public int ChampionID;
            public int Ability1;
            public int Ability2;
        }

        public class BattleGroup
        {
            const int CurrentVersion = 1;

            public List<ChampionBG> Champions = new List<ChampionBG>();
            public List<int> Spells = new List<int>();
            public List<int> Relics = new List<int>();
            public List<int> Equipments = new List<int>();
            public int SelectedAvatar;
            public int SelectedFactions;

            public int GetRuneCount()
            {
                return Champions.Count + Spells.Count + Relics.Count + Equipments.Count;
            }

            // used for iteration over runes
            public Rune GetRune(Database db, int rune_index)
            {
                if(rune_index < 0)
                {
                    return null;
                }

                // get champion
                if(rune_index < Champions.Count)
                {
                    if (!db.Champions.ContainsKey(Champions[rune_index].ChampionID))
                        return null;

                    return db.Champions[Champions[rune_index].ChampionID];
                }
                rune_index -= Champions.Count;

                // get spell
                if (rune_index < Spells.Count)
                {
                    if (!db.Spells.ContainsKey(Spells[rune_index]))
                        return null;

                    return db.Spells[Spells[rune_index]];
                }
                rune_index -= Spells.Count;

                // get relic
                if (rune_index < Relics.Count)
                {
                    if (!db.Relics.ContainsKey(Relics[rune_index]))
                        return null;

                    return db.Relics[Relics[rune_index]];
                }
                rune_index -= Relics.Count;

                // get equipment
                if (rune_index < Equipments.Count)
                {
                    if (!db.Equipments.ContainsKey(Equipments[rune_index]))
                        return null;

                    return db.Equipments[Equipments[rune_index]];
                }
                rune_index -= Equipments.Count;

                return null;
            }

            public byte[] ToRawMemory()
            {
                byte[] data = null;
                byte[] data2 = null;

                using (MemoryStream ms2 = new MemoryStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            // version
                            bw.Write((byte)CurrentVersion);

                            // rune counts
                            bw.Write((byte)GetRuneCount());
                            bw.Write((byte)Champions.Count);
                            bw.Write((byte)Spells.Count);
                            bw.Write((byte)Relics.Count);
                            bw.Write((byte)Equipments.Count);

                            // champions
                            foreach (var cbg in Champions)
                            {
                                bw.Write((ushort)cbg.ChampionID);
                                bw.Write((ushort)cbg.Ability1);
                                bw.Write((ushort)cbg.Ability2);
                            }

                            // spells
                            foreach (var s in Spells)
                                bw.Write((ushort)s);

                            // relics
                            foreach (var r in Relics)
                                bw.Write((ushort)r);

                            // equipments
                            foreach (var e in Equipments)
                                bw.Write((ushort)e);

                            // faction data
                            bw.Write((sbyte)SelectedAvatar);
                            bw.Write((sbyte)SelectedFactions);

                            data = ms.ToArray();

                        }
                    }

                    // compress
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            using (DeflateStream ds = new DeflateStream(ms2, CompressionMode.Compress))
                            {
                                ms.CopyTo(ds);
                            }
                        }

                        data2 = ms2.ToArray();
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }

                return data2;
            }

            public bool FromRawMemory(byte[] array)
            {
                Champions.Clear();
                Spells.Clear();
                Relics.Clear();
                Equipments.Clear();
                SelectedAvatar = Utility.NO_INDEX;
                SelectedFactions = Utility.NO_INDEX;

                byte[] data = null;

                // decompress
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        using (MemoryStream ms2 = new MemoryStream(array))
                        {
                            using (DeflateStream ds = new DeflateStream(ms2, CompressionMode.Decompress))
                            {
                                ds.CopyTo(ms);
                            }
                        }
                        data = ms.ToArray();
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }

                // read
                using (MemoryStream ms = new MemoryStream(data))
                { 
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        // validation
                        if (ms.Length - ms.Position < sizeof(byte))
                        {
                            return false;
                        }

                        byte Version = br.ReadByte();       // useful later on
                        if (Version > CurrentVersion)
                        {
                            return false;
                        }

                        // validation
                        if (ms.Length - ms.Position < 5 * sizeof(byte))
                        {
                            return false;
                        }

                        byte RuneCount, ChampCount, SpellCount, RelicCount, EquipCount = 0;
                        RuneCount = br.ReadByte();
                        ChampCount = br.ReadByte();
                        SpellCount = br.ReadByte();
                        RelicCount = br.ReadByte();
                        EquipCount = br.ReadByte();

                        // validation
                        if ((int)RuneCount != (ChampCount + SpellCount + RelicCount + EquipCount))
                        {
                            return false;
                        }

                        // validation
                        if (ms.Length - ms.Position < ChampCount * sizeof(ushort) * 3)
                        {
                            return false;
                        }

                        for (int i = 0; i < ChampCount; i++)
                        {
                            ChampionBG cua = new ChampionBG();
                            cua.ChampionID = br.ReadUInt16();
                            cua.Ability1 = br.ReadUInt16();
                            cua.Ability2 = br.ReadUInt16();
                            Champions.Add(cua);
                        }

                        // validation
                        if (ms.Length - ms.Position < SpellCount * sizeof(ushort))
                        {
                            return false;
                        }

                        for (int i = 0; i < SpellCount; i++)
                        {
                            Spells.Add(br.ReadUInt16());
                        }

                        // validation
                        if (ms.Length - ms.Position < RelicCount * sizeof(ushort))
                        {
                            return false;
                        }

                        for (int i = 0; i < RelicCount; i++)
                        {
                            Relics.Add(br.ReadUInt16());
                        }

                        // validation
                        if (ms.Length - ms.Position < EquipCount * sizeof(ushort))
                        {
                            return false;
                        }

                        for (int i = 0; i < EquipCount; i++)
                        {
                            Equipments.Add(br.ReadUInt16());
                        }

                        // validation
                        if (ms.Length - ms.Position < 2 * sizeof(sbyte))
                        {
                            return false;
                        }

                        SelectedAvatar = br.ReadSByte();
                        SelectedFactions = br.ReadSByte();
                    }
                }

                return true;
            }
        }

        // returns 2 values: val1 - sell price, val2 - buy price
        private Tuple<int, int> GetRuneShardCost(Rune r)
        {
            if (r.Expansion == "Planar Disturbances")
                return new Tuple<int, int>(0, 0);

            if (!r.ForSale)
                return new Tuple<int, int>(0, 0);

            if (!CostPerRarity.ContainsKey(r.Rarity))
                return new Tuple<int, int>(0, 0);

            bool cheap_expansion = ((r.Expansion == "Pox Nora Release Set") || (r.Expansion == "Savage Tundra Expansion Set") || (r.Expansion == "Shattered Peaks Expansion Set")
                || (r.Expansion == "Drums of War") || (r.Expansion == "Skeezick Rebellion") || (r.Expansion == "Grimlic's Descent"));

            Tuple<int, int> val = CostPerRarity[r.Rarity];
            if (!cheap_expansion)
                val = new Tuple<int, int>(val.Item1 * 2, val.Item2 * 2);

            return val;
        }

        public struct BGStats_MeleeRangedValue<T>
        {
            public T Item1;
            public T Item2;

            public BGStats_MeleeRangedValue(T i1, T i2)
            {
                Item1 = i1;
                Item2 = i2;
            }
        }


        public class BattleGroupStats
        {
            public Dictionary<string, int> RunesPerFaction = new Dictionary<string, int>();
            public Dictionary<string, int> RunesPerRarity = new Dictionary<string, int>();
            public int NoraShardCostBuy = 0;
            public int NoraShardCostSell = 0;
            public Dictionary<DataElement.ElementType, float> AverageNoraCostPerRuneType = new Dictionary<DataElement.ElementType, float>();
            public Dictionary<DataElement.ElementType, int> TotalNoraCostPerRuneType = new Dictionary<DataElement.ElementType, int>();
            public BGStats_MeleeRangedValue<float> AverageChampionDamage = new BGStats_MeleeRangedValue<float>(0, 0);    // melee, ranged
            public BGStats_MeleeRangedValue<float> AverageChampionSpeed = new BGStats_MeleeRangedValue<float>(0, 0);
            public BGStats_MeleeRangedValue<float> AverageChampionMinRNG = new BGStats_MeleeRangedValue<float>(0, 0);
            public BGStats_MeleeRangedValue<float> AverageChampionMaxRNG = new BGStats_MeleeRangedValue<float>(0, 0);
            public BGStats_MeleeRangedValue<float> AverageChampionDefense = new BGStats_MeleeRangedValue<float>(0, 0);
            public BGStats_MeleeRangedValue<float> AverageChampionHealth = new BGStats_MeleeRangedValue<float>(0, 0);
            public int RangedChampionCount = 0;
            public int HybridRangeChampionCount = 0;
            public int MeleeChampionCount = 0;
            public int BigChampionCount = 0;
            public int NoAttackChampionCount = 0;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        BattleGroup BG = new BattleGroup();
        BattleGroupStats BGStats = new BattleGroupStats();
        string OpenedBattlegroup = "";
        int SelectedRuneIndex = Utility.NO_INDEX;

        int RunesPerRow = 0;
        int RunesPerColumn = 0;
        int RunesPerPage = 0;
        int RunePageCount = 0;
        int RuneCurrentPage = 0;
        List<int> RuneList = new List<int>();
        Pox.DataElement.ElementType RunePageType = DataElement.ElementType.CHAMPION;

        // static cache
        List<string> FactionToName = new List<string>();
        Dictionary<string, string> FactionNameToFactionShortName = new Dictionary<string, string>();
        Dictionary<string, Tuple<int, int>> CostPerRarity = new Dictionary<string, Tuple<int, int>>();  // key: rarity name, value: (sell, buy)

        // dynamic cache
        Dictionary<int, bool> NoBasicAttackChampCache = new Dictionary<int, bool>();

        public BattlegroupBuilder()
        {
            InitializeComponent();
        }

        public struct RuneBGSort: IEquatable<RuneBGSort>, IComparable<RuneBGSort>
        {
            public int RuneIndex;
            public string RuneName;

            public bool Equals(RuneBGSort other)
            {
                return (RuneIndex == other.RuneIndex);
            }

            public int CompareTo(RuneBGSort comparePart)
            {
                return RuneName.CompareTo(comparePart.RuneName);
            }
        }

        // bg is sorted only on load, to ensure integrity
        public void SortBattlegroup(Database db, BattleGroup bg)
        {
            // sort battlegroup, so the runes displayed are sorted by name in each of categories
            List<RuneBGSort> sort_list = new List<RuneBGSort>();
            string tmp_name;
            
            // sort champs
            sort_list.Clear();
            for (int i = 0; i < bg.Champions.Count; i++)
            {
                tmp_name = "";
                if (db.Champions.ContainsKey(bg.Champions[i].ChampionID))
                    tmp_name = db.Champions[bg.Champions[i].ChampionID].Name;

                sort_list.Add(new RuneBGSort() { RuneIndex = i, RuneName = tmp_name });
            }
            sort_list.Sort();
            List<ChampionBG> new_champs = new List<ChampionBG>();
            for (int i = 0; i < sort_list.Count; i++)
            {
                new_champs.Add(bg.Champions[sort_list[i].RuneIndex]);
            }
            bg.Champions = new_champs;

            // sort spells
            sort_list.Clear();
            for (int i = 0; i < bg.Spells.Count; i++)
            {
                tmp_name = "";
                if (db.Spells.ContainsKey(bg.Spells[i]))
                    tmp_name = db.Spells[bg.Spells[i]].Name;

                sort_list.Add(new RuneBGSort() { RuneIndex = i, RuneName = tmp_name });
            }
            sort_list.Sort();
            List<int> new_spells = new List<int>();
            for (int i = 0; i < sort_list.Count; i++)
            {
                new_spells.Add(bg.Spells[sort_list[i].RuneIndex]);
            }
            bg.Spells = new_spells;

            // sort relics
            sort_list.Clear();
            for (int i = 0; i < bg.Relics.Count; i++)
            {
                tmp_name = "";
                if (db.Relics.ContainsKey(bg.Relics[i]))
                    tmp_name = db.Relics[bg.Relics[i]].Name;

                sort_list.Add(new RuneBGSort() { RuneIndex = i, RuneName = tmp_name });
            }
            sort_list.Sort();
            List<int> new_relics = new List<int>();
            for (int i = 0; i < sort_list.Count; i++)
            {
                new_relics.Add(bg.Relics[sort_list[i].RuneIndex]);
            }
            bg.Relics = new_relics;

            // sort equipments
            sort_list.Clear();
            for (int i = 0; i < bg.Equipments.Count; i++)
            {
                tmp_name = "";
                if (db.Equipments.ContainsKey(bg.Equipments[i]))
                    tmp_name = db.Equipments[bg.Equipments[i]].Name;

                sort_list.Add(new RuneBGSort() { RuneIndex = i, RuneName = tmp_name });
            }
            sort_list.Sort();
            List<int> new_equipments = new List<int>();
            for (int i = 0; i < sort_list.Count; i++)
            {
                new_equipments.Add(bg.Equipments[sort_list[i].RuneIndex]);
            }
            bg.Equipments = new_equipments;
        }

        // analyzes battlegroup, looking for inconsistencies which would not allow the bg to be picked in ranked game
        // should be called whenever the bg changes
        public void ParseBattlegroupRanked(Database db, BattleGroup bg, BattleGroupStats bgs)
        {
            List<string> messages = new List<string>();

            // at this point, BG is sorted and has some runes in it, and some selected factions
            bool calculate_cost_stats = true;
            bool calculate_distribution_stats = true;

            // 1. check if the BG has 30 runes
            if (bg.GetRuneCount() != 30)
            {
                messages.Add(string.Format("Incorrect amount of runes (30 required, {0} found)", bg.GetRuneCount()));
            }

            // 2. look for inconsistencies in champions - check if champions are valid, and if upgrade abilities are possible
            for (int i = 0; i < bg.Champions.Count; i++)
            {
                if (!db.Champions.ContainsKey(bg.Champions[i].ChampionID))
                {
                    messages.Add(string.Format("Invalid champion ID: {0}", bg.Champions[i]));
                    calculate_cost_stats = false;
                    calculate_distribution_stats = false;
                }

                Champion c = db.Champions[bg.Champions[i].ChampionID];
                ChampionBG cua = bg.Champions[i];

                // upgrade 1
                bool found_ability = false;
                for (int j = 0; j < c.Upgrade1.Count; j++)
                {
                    if (c.Upgrade1[j] == cua.Ability1)
                    {
                        found_ability = true;
                        break;
                    }
                }

                if (!found_ability)
                {
                    string ab_name = "<missing>";
                    if (db.Abilities.ContainsKey(cua.Ability1))
                    {
                        ab_name = db.Abilities[cua.Ability1].Name;
                    }

                    messages.Add(string.Format("Invalid U1 for {0}: {1}", c.Name, ab_name));
                    calculate_cost_stats = false;
                }

                // upgrade 2
                found_ability = false;
                for (int j = 0; j < c.Upgrade2.Count; j++)
                {
                    if (c.Upgrade2[j] == cua.Ability2)
                    {
                        found_ability = true;
                        break;
                    }
                }

                if (!found_ability)
                {
                    string ab_name = "<missing>";
                    if (db.Abilities.ContainsKey(cua.Ability2))
                    {
                        ab_name = db.Abilities[cua.Ability2].Name;
                    }

                    messages.Add(string.Format("Invalid U2 for {0}: {1}", c.Name, ab_name));
                    calculate_cost_stats = false;
                }
            }

            // 3. check if spells, relics, equips are valid
            for (int i = 0; i < bg.Spells.Count; i++)
            {
                if (!db.Spells.ContainsKey(bg.Spells[i]))
                {
                    messages.Add(string.Format("Invalid spell ID: {0}", bg.Spells[i]));
                    calculate_cost_stats = false;
                    calculate_distribution_stats = false;
                }
            }

            for (int i = 0; i < bg.Relics.Count; i++)
            {
                if (!db.Relics.ContainsKey(bg.Relics[i]))
                {
                    messages.Add(string.Format("Invalid relic ID: {0}", bg.Relics[i]));
                    calculate_cost_stats = false;
                    calculate_distribution_stats = false;
                }
            }

            for (int i = 0; i < bg.Equipments.Count; i++)
            {
                if (!db.Equipments.ContainsKey(bg.Equipments[i]))
                {
                    messages.Add(string.Format("Invalid equipment ID: {0}", bg.Equipments[i]));
                    calculate_cost_stats = false;
                    calculate_distribution_stats = false;
                }
            }

            // 4. check deck limits of each rune
            Dictionary<int, int> rune_counter = new Dictionary<int, int>();

            // champs
            rune_counter.Clear();
            for (int i = 0; i < bg.Champions.Count; i++)
            {
                if (!rune_counter.ContainsKey(bg.Champions[i].ChampionID))
                    rune_counter.Add(bg.Champions[i].ChampionID, 0);
                rune_counter[bg.Champions[i].ChampionID] += 1;
            }
            foreach (var kv in rune_counter)
            {
                if (db.Champions[kv.Key].DeckLimit < kv.Value)
                    messages.Add(string.Format("Exceeded limit of {0} champions of type {1} (found {2})", db.Champions[kv.Key].DeckLimit, db.Champions[kv.Key].Name, kv.Value));
            }

            // spells
            rune_counter.Clear();
            for (int i = 0; i < bg.Spells.Count; i++)
            {
                if (!rune_counter.ContainsKey(bg.Spells[i]))
                    rune_counter.Add(bg.Spells[i], 0);
                rune_counter[bg.Spells[i]] += 1;
            }
            foreach (var kv in rune_counter)
            {
                if (db.Spells[kv.Key].DeckLimit < kv.Value)
                    messages.Add(string.Format("Exceeded limit of {0} spells of type {1} (found {2})", db.Spells[kv.Key].DeckLimit, db.Spells[kv.Key].Name, kv.Value));
            }

            // relics
            rune_counter.Clear();
            for (int i = 0; i < bg.Relics.Count; i++)
            {
                if (!rune_counter.ContainsKey(bg.Relics[i]))
                    rune_counter.Add(bg.Relics[i], 0);
                rune_counter[bg.Relics[i]] += 1;
            }
            foreach (var kv in rune_counter)
            {
                if (db.Relics[kv.Key].DeckLimit < kv.Value)
                    messages.Add(string.Format("Exceeded limit of {0} relics of type {1} (found {2})", db.Relics[kv.Key].DeckLimit, db.Relics[kv.Key].Name, kv.Value));
            }

            // equips
            rune_counter.Clear();
            for (int i = 0; i < bg.Equipments.Count; i++)
            {
                if (!rune_counter.ContainsKey(bg.Equipments[i]))
                    rune_counter.Add(bg.Equipments[i], 0);
                rune_counter[bg.Equipments[i]] += 1;
            }
            foreach (var kv in rune_counter)
            {
                if (db.Equipments[kv.Key].DeckLimit < kv.Value)
                    messages.Add(string.Format("Exceeded limit of {0} equipments of type {1} (found {2})", db.Equipments[kv.Key].DeckLimit, db.Equipments[kv.Key].Name, kv.Value));
            }

            // 5. hardcoded rune group check, todo
            ParseBattlegroupSpecialCheck(db, bg, messages);

            // 6. calculate stats
            CalculateBattlegroupStats(db, bg, bgs, calculate_cost_stats, calculate_distribution_stats);

            // 7. check inconsistencies regarding factions (not needed if there arent 30 runes in the bg)
            if (bg.GetRuneCount() == 30)
            {
                // needs to have either 30 runes from one faction, or at least 2 factions with more than 15 runes
                int HalfFactionNum = 0;
                int FullFactionNum = 0;
                foreach (var s in FactionToName)
                {
                    if (bgs.RunesPerFaction.ContainsKey(s))
                    {
                        if (bgs.RunesPerFaction[s] == 30)
                        {
                            FullFactionNum += 1;
                        }
                        else if (bgs.RunesPerFaction[s] >= 15)
                        {
                            HalfFactionNum += 1;
                        }
                    }
                }

                if ((FullFactionNum == 0) && (HalfFactionNum < 2))
                {
                    messages.Add("Not enough integrity (requires at least 15 runes of two different factions, or 30 runes of a single faction)");
                }
                else if (FullFactionNum >= 2)
                {
                    messages.Add("Too much integrity (30 runes of two or more different factions present in the battlegroup)");
                }

                // if above conditions are met, selected factions will ALWAYS have enough runes of each faction - guaranteed by battlegroup builder
            }

            ListBoxBGErrorLog.Items.Clear();
            if (messages.Count == 0)
            {
                LabelBGStatus.Text = "BG Status: Can use in ranked";
            }
            else
            {
                LabelBGStatus.Text = "BG Status: CAN'T use in ranked (see below)";

                foreach (var s in messages)
                    ListBoxBGErrorLog.Items.Add(s);
            }
        }

        private void ParseBattlegroupSpecialCheck(Database db, BattleGroup bg, List<string> messages)
        {
            
        }

        private void CalculateBattlegroupStats(Database db, BattleGroup bg, BattleGroupStats bgs, bool calc_cost, bool calc_distribution)
        {
            bgs.RunesPerFaction.Clear();
            bgs.RunesPerRarity.Clear();
            bgs.NoraShardCostBuy = 0;
            bgs.NoraShardCostSell = 0;
            bgs.AverageNoraCostPerRuneType.Clear();
            bgs.AverageNoraCostPerRuneType.Add(DataElement.ElementType.CHAMPION, 0);
            bgs.AverageNoraCostPerRuneType.Add(DataElement.ElementType.SPELL, 0);
            bgs.AverageNoraCostPerRuneType.Add(DataElement.ElementType.RELIC, 0);
            bgs.AverageNoraCostPerRuneType.Add(DataElement.ElementType.EQUIPMENT, 0);
            bgs.TotalNoraCostPerRuneType.Clear();
            bgs.TotalNoraCostPerRuneType.Add(DataElement.ElementType.CHAMPION, 0);
            bgs.TotalNoraCostPerRuneType.Add(DataElement.ElementType.SPELL, 0);
            bgs.TotalNoraCostPerRuneType.Add(DataElement.ElementType.RELIC, 0);
            bgs.TotalNoraCostPerRuneType.Add(DataElement.ElementType.EQUIPMENT, 0);
            bgs.AverageChampionDamage = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.AverageChampionDefense = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.AverageChampionHealth = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.AverageChampionMaxRNG = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.AverageChampionMinRNG = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.AverageChampionSpeed = new BGStats_MeleeRangedValue<float>(0, 0);
            bgs.BigChampionCount = 0;
            bgs.RangedChampionCount = 0;
            bgs.HybridRangeChampionCount = 0;
            bgs.MeleeChampionCount = 0;
            bgs.NoAttackChampionCount = 0;

            // calculate runes per faction and per rarity
            for (int i = 0; i < bg.GetRuneCount(); i++)
            {
                Rune r = bg.GetRune(db, i);
                if(r == null)
                    continue;

                if (calc_distribution)
                {
                    foreach (var f in r.Faction)
                    {
                        if (!bgs.RunesPerFaction.ContainsKey(f))
                            bgs.RunesPerFaction.Add(f, 0);

                        // todo: deal with duplicate factions in a few runes (example: kiergana)
                        bgs.RunesPerFaction[f] += 1;
                    }

                    if (!bgs.RunesPerRarity.ContainsKey(r.Rarity))
                        bgs.RunesPerRarity.Add(r.Rarity, 0);

                    bgs.RunesPerRarity[r.Rarity] += 1;
                }

                Tuple<int, int> rune_price = GetRuneShardCost(r);
                bgs.NoraShardCostBuy += rune_price.Item2;
                bgs.NoraShardCostSell += rune_price.Item1;
            }

            if(calc_cost)
            {
                // calc champ cost
                foreach(var cbg in bg.Champions)
                {
                    if (!db.Champions.ContainsKey(cbg.ChampionID))
                        continue;

                    Champion c = db.Champions[cbg.ChampionID];
                    int champ_cost = c.BaseNoraCost;
                    foreach (var ab in c.BaseAbilities_refs)
                        champ_cost += ab.NoraCost;

                    if (db.Abilities.ContainsKey(cbg.Ability1))
                        champ_cost += db.Abilities[cbg.Ability1].NoraCost;
                    if (db.Abilities.ContainsKey(cbg.Ability2))
                        champ_cost += db.Abilities[cbg.Ability2].NoraCost;

                    bgs.TotalNoraCostPerRuneType[DataElement.ElementType.CHAMPION] += champ_cost;
                }

                if(bg.Champions.Count != 0)
                    bgs.AverageNoraCostPerRuneType[DataElement.ElementType.CHAMPION] = bgs.TotalNoraCostPerRuneType[DataElement.ElementType.CHAMPION] / (float)bg.Champions.Count;

                // calc spell cost
                foreach (var s in bg.Spells)
                {
                    if (!db.Spells.ContainsKey(s))
                        continue;

                    bgs.TotalNoraCostPerRuneType[DataElement.ElementType.SPELL] += db.Spells[s].NoraCost;
                }

                if(bg.Spells.Count != 0)
                    bgs.AverageNoraCostPerRuneType[DataElement.ElementType.SPELL] = bgs.TotalNoraCostPerRuneType[DataElement.ElementType.SPELL] / (float)bg.Spells.Count;

                // calc relic cost
                foreach (var r in bg.Relics)
                {
                    if (!db.Relics.ContainsKey(r))
                        continue;

                    bgs.TotalNoraCostPerRuneType[DataElement.ElementType.RELIC] += db.Relics[r].NoraCost;
                }

                if(bg.Relics.Count != 0)
                    bgs.AverageNoraCostPerRuneType[DataElement.ElementType.RELIC] = bgs.TotalNoraCostPerRuneType[DataElement.ElementType.RELIC] / (float)bg.Relics.Count;

                // calc equipment cost
                foreach (var e in bg.Equipments)
                {
                    if (!db.Equipments.ContainsKey(e))
                        continue;

                    bgs.TotalNoraCostPerRuneType[DataElement.ElementType.EQUIPMENT] += db.Equipments[e].NoraCost;
                }

                if(bg.Equipments.Count != 0)
                    bgs.AverageNoraCostPerRuneType[DataElement.ElementType.EQUIPMENT] = bgs.TotalNoraCostPerRuneType[DataElement.ElementType.EQUIPMENT] / (float)bg.Equipments.Count;
            }

            // calculate average champion stats
            // only champs which exist in DB count
            int melee_champ_count = 0;
            int ranged_champ_count = 0;
            foreach(var cbg in bg.Champions)
            {
                Champion c = null;
                if (db.Champions.ContainsKey(cbg.ChampionID))
                    c = db.Champions[cbg.ChampionID];
                else
                    continue;

                bool is_melee = true;

                if (c.MaxRNG <= 1)
                    bgs.MeleeChampionCount += 1;
                if (c.MinRNG > 1)
                {
                    bgs.RangedChampionCount += 1;
                    is_melee = false;
                }
                if ((c.MinRNG <= 1) && (c.MaxRNG > 1))
                    bgs.HybridRangeChampionCount += 1;
                if (c.Size == 2)
                    bgs.BigChampionCount += 1;

                // todo: no basic attack champs

                if (is_melee)
                {
                    melee_champ_count += 1;
                    bgs.AverageChampionDamage.Item1 += c.Damage;
                    bgs.AverageChampionDefense.Item1 += c.Defense;
                    bgs.AverageChampionHealth.Item1 += c.HitPoints;
                    bgs.AverageChampionMaxRNG.Item1 += c.MaxRNG;
                    bgs.AverageChampionMinRNG.Item1 += c.MinRNG;
                    bgs.AverageChampionSpeed.Item1 += c.Speed;
                }
                else
                {
                    ranged_champ_count += 1;
                    bgs.AverageChampionDamage.Item2 += c.Damage;
                    bgs.AverageChampionDefense.Item2 += c.Defense;
                    bgs.AverageChampionHealth.Item2 += c.HitPoints;
                    bgs.AverageChampionMaxRNG.Item2 += c.MaxRNG;
                    bgs.AverageChampionMinRNG.Item2 += c.MinRNG;
                    bgs.AverageChampionSpeed.Item2 += c.Speed;
                }
            }
            if(melee_champ_count != 0)
            {
                bgs.AverageChampionDamage.Item1 /= melee_champ_count;
                bgs.AverageChampionDefense.Item1 /= melee_champ_count;
                bgs.AverageChampionHealth.Item1 /= melee_champ_count;
                bgs.AverageChampionMaxRNG.Item1 /= melee_champ_count;
                bgs.AverageChampionMinRNG.Item1 /= melee_champ_count;
                bgs.AverageChampionSpeed.Item1 /= melee_champ_count;
            }
            if(ranged_champ_count != 0)
            {
                bgs.AverageChampionDamage.Item2 /= ranged_champ_count;
                bgs.AverageChampionDefense.Item2 /= ranged_champ_count;
                bgs.AverageChampionHealth.Item2 /= ranged_champ_count;
                bgs.AverageChampionMaxRNG.Item2 /= ranged_champ_count;
                bgs.AverageChampionMinRNG.Item2 /= ranged_champ_count;
                bgs.AverageChampionSpeed.Item2 /= ranged_champ_count;
            }

            // this doesnt have to be here, but it is for now
            UpdateBGStatsUI(bg, bgs);
        }

        private void UpdateBGStatsUI(BattleGroup bg, BattleGroupStats bgs)
        {
            LabelChampions.Text = "Champions: " + bg.Champions.Count.ToString();
            LabelSpells.Text = "Spells: " + bg.Spells.Count.ToString();
            LabelRelics.Text = "Relics: " + bg.Relics.Count.ToString();
            LabelEquipments.Text = "Equipments: " + bg.Equipments.Count.ToString();

            LabelRuneFS.Text = "FS: " + (bgs.RunesPerFaction.ContainsKey("Forglar Swamp") ? bgs.RunesPerFaction["Forglar Swamp"].ToString() : "0");
            LabelRuneFW.Text = "FW: " + (bgs.RunesPerFaction.ContainsKey("Forsaken Wastes") ? bgs.RunesPerFaction["Forsaken Wastes"].ToString() : "0");
            LabelRuneIS.Text = "IS: " + (bgs.RunesPerFaction.ContainsKey("Ironfist Stronghold") ? bgs.RunesPerFaction["Ironfist Stronghold"].ToString() : "0");
            LabelRuneKF.Text = "KF: " + (bgs.RunesPerFaction.ContainsKey("K'thir Forest") ? bgs.RunesPerFaction["K'thir Forest"].ToString() : "0");
            LabelRuneSL.Text = "SL: " + (bgs.RunesPerFaction.ContainsKey("Sundered Lands") ? bgs.RunesPerFaction["Sundered Lands"].ToString() : "0");
            LabelRuneSP.Text = "SP: " + (bgs.RunesPerFaction.ContainsKey("Shattered Peaks") ? bgs.RunesPerFaction["Shattered Peaks"].ToString() : "0");
            LabelRuneST.Text = "ST: " + (bgs.RunesPerFaction.ContainsKey("Savage Tundra") ? bgs.RunesPerFaction["Savage Tundra"].ToString() : "0");
            LabelRuneUD.Text = "UD: " + (bgs.RunesPerFaction.ContainsKey("Underdepths") ? bgs.RunesPerFaction["Underdepths"].ToString() : "0");

            LabelRuneCountByFaction.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}",
                (bgs.RunesPerFaction.ContainsKey("Forglar Swamp") ? bgs.RunesPerFaction["Forglar Swamp"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Forsaken Wastes") ? bgs.RunesPerFaction["Forsaken Wastes"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Ironfist Stronghold") ? bgs.RunesPerFaction["Ironfist Stronghold"] : 0),
                (bgs.RunesPerFaction.ContainsKey("K'thir Forest") ? bgs.RunesPerFaction["K'thir Forest"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Sundered Lands") ? bgs.RunesPerFaction["Sundered Lands"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Shattered Peaks") ? bgs.RunesPerFaction["Shattered Peaks"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Savage Tundra") ? bgs.RunesPerFaction["Savage Tundra"] : 0),
                (bgs.RunesPerFaction.ContainsKey("Underdepths") ? bgs.RunesPerFaction["Underdepths"] : 0));

            LabelRuneCountByRarity.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}",
                (bgs.RunesPerRarity.ContainsKey("COMMON") ? bgs.RunesPerRarity["COMMON"] : 0),
                (bgs.RunesPerRarity.ContainsKey("UNCOMMON") ? bgs.RunesPerRarity["UNCOMMON"] : 0),
                (bgs.RunesPerRarity.ContainsKey("RARE") ? bgs.RunesPerRarity["RARE"] : 0),
                (bgs.RunesPerRarity.ContainsKey("EXOTIC") ? bgs.RunesPerRarity["EXOTIC"] : 0),
                (bgs.RunesPerRarity.ContainsKey("LEGENDARY") ? bgs.RunesPerRarity["LEGENDARY"] : 0),
                (bgs.RunesPerRarity.ContainsKey("LIMITED") ? bgs.RunesPerRarity["LIMITED"] : 0));

            float total_average = 0;
            if(bg.GetRuneCount() != 0)
                total_average = (bgs.TotalNoraCostPerRuneType[DataElement.ElementType.CHAMPION] + bgs.TotalNoraCostPerRuneType[DataElement.ElementType.SPELL]
                + bgs.TotalNoraCostPerRuneType[DataElement.ElementType.RELIC] + bgs.TotalNoraCostPerRuneType[DataElement.ElementType.EQUIPMENT]) / (float)(bg.GetRuneCount());

            LabelNoraCostByRuneType.Text = string.Format("{0:N1}\r\n{1:N1}\r\n{2:N1}\r\n{3:N1}\r\n{4:N1}",
                bgs.AverageNoraCostPerRuneType[DataElement.ElementType.CHAMPION],
                bgs.AverageNoraCostPerRuneType[DataElement.ElementType.SPELL],
                bgs.AverageNoraCostPerRuneType[DataElement.ElementType.RELIC],
                bgs.AverageNoraCostPerRuneType[DataElement.ElementType.EQUIPMENT],
                total_average);

            LabelBGShardCost.Text = string.Format("BG shard cost: {0}/{1} (buy/sell)",
                bgs.NoraShardCostBuy,
                bgs.NoraShardCostSell);

            LabelAverageChampionStats.Text = string.Format("{0:N1}/{1:N1}\r\n{2:N1}/{3:N1}\r\n{4:N1}/{5:N1}\r\n{6:N1}/{7:N1}\r\n{8:N1}/{9:N1}\r\n{10:N1}/{11:N1}",
                bgs.AverageChampionDamage.Item1, bgs.AverageChampionDamage.Item2,
                bgs.AverageChampionSpeed.Item1, bgs.AverageChampionSpeed.Item2,
                bgs.AverageChampionMinRNG.Item1, bgs.AverageChampionMinRNG.Item2,
                bgs.AverageChampionMaxRNG.Item1, bgs.AverageChampionMaxRNG.Item2,
                bgs.AverageChampionDefense.Item1, bgs.AverageChampionDefense.Item2,
                bgs.AverageChampionHealth.Item1, bgs.AverageChampionHealth.Item2);

            LabelChampionCounts.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}",
                bgs.MeleeChampionCount,
                bgs.HybridRangeChampionCount,
                bgs.RangedChampionCount,
                bgs.BigChampionCount,
                bgs.NoAttackChampionCount);
        }

        private void CalculateRunePreviewPageSize()
        {
            int rune_width = 76;
            int rune_height = 107;
            int rune_offset_x = 0;
            int rune_offset_y = 0;

            RunesPerRow = (PanelRuneIcons.Width + rune_offset_x) / (rune_width + rune_offset_x);
            RunesPerColumn = (PanelRuneIcons.Height + rune_offset_y) / (rune_height + rune_offset_y);
            RunesPerPage = RunesPerRow * RunesPerColumn;

            if(PanelRuneIcons.Controls.Count > RunesPerPage)
            {
                for(int i = PanelRuneIcons.Controls.Count - 1; i >= RunesPerPage; i--)
                {
                    Program.image_cache.RemoveRunePreviewSubscriber((RunePreviewControl)(PanelRuneIcons.Controls[i]));
                    PanelRuneIcons.Controls.RemoveAt(i);
                }
            }
            else if(PanelRuneIcons.Controls.Count < RunesPerPage)
            {
                for(int i = PanelRuneIcons.Controls.Count; i < RunesPerPage; i++)
                {
                    RunePreviewControl rpc = new RunePreviewControl();
                    PanelRuneIcons.Controls.Add(rpc);
                    rpc.RunePreviewImage.MouseDown += RunePageImage_MouseClick;
                    rpc.Hide();
                }
            }

            for(int i = 0; i < RunesPerColumn; i++)
            {
                for(int j = 0; j < RunesPerRow; j++)
                {
                    RunePreviewControl rpc = (RunePreviewControl)(PanelRuneIcons.Controls[i * RunesPerRow + j]);
                    rpc.Location = new Point(j * (rune_width + rune_offset_x), i * (rune_height + rune_offset_y));
                }
            }
        }

        private void UpdateRuneList(Database db)
        {
            RuneListInfo rlf = Program.main_form.GetSelectedCard();

            RuneList.Clear();

            switch (RunePageType)
            {
                case DataElement.ElementType.CHAMPION:
                    {
                        foreach (var kv in db.Champions)
                        {
                            if ((rlf.Filter == null) || (!CheckboxApplyFilter.Checked) || (rlf.Filter.Satisfies(kv.Value))) 
                            {
                                RuneList.Add(kv.Key);
                            }
                        }
                    }
                    break;
                case DataElement.ElementType.SPELL:
                    {
                        foreach (var kv in db.Spells)
                        {
                            if ((rlf.Filter == null) || (!CheckboxApplyFilter.Checked) || (rlf.Filter.Satisfies(kv.Value)))
                            {
                                RuneList.Add(kv.Key);
                            }
                        }
                    }
                    break;
                case DataElement.ElementType.RELIC:
                    {
                        foreach (var kv in db.Relics)
                        {
                            if ((rlf.Filter == null) || (!CheckboxApplyFilter.Checked) || (rlf.Filter.Satisfies(kv.Value)))
                            {
                                RuneList.Add(kv.Key);
                            }
                        }
                    }
                    break;
                case DataElement.ElementType.EQUIPMENT:
                    {
                        foreach (var kv in db.Equipments)
                        {
                            if ((rlf.Filter == null) || (!CheckboxApplyFilter.Checked) || (rlf.Filter.Satisfies(kv.Value)))
                            {
                                RuneList.Add(kv.Key);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            if(RuneList.Count == 0)
            {
                RunePageCount = 0;
            }
            else
            {
                RunePageCount = (RuneList.Count / RunesPerPage) + ((RuneList.Count % RunesPerPage) == 0 ? 0 : 1);
            }

            DisplayRunePage(db, 0);
        }

        private void DisplayRunePage(Database db, int page_index)
        {
            foreach (RunePreviewControl c in PanelRuneIcons.Controls)
                Program.image_cache.RemoveRunePreviewSubscriber(c);

            RuneCurrentPage = page_index;

            if (RunePageCount == 0)
            {
                LabelPageNum.Text = "Page 0/0";
            }
            else
            {
                LabelPageNum.Text = string.Format("Page {0}/{1}", RuneCurrentPage + 1, RunePageCount);
            }

            int start_rune = RunesPerPage * RuneCurrentPage;
            int rune_count = Math.Min(RunesPerPage, RuneList.Count - start_rune);

            for(int i = 0; i < rune_count; i++)
            {
                int id = RuneList[start_rune + i];
                RunePreviewControl rpc = (RunePreviewControl)(PanelRuneIcons.Controls[i]);
                rpc.Show();
                rpc.ElemID = id;
                rpc.RunePreviewImage.Image = null;
                switch (RunePageType)
                {
                    case DataElement.ElementType.CHAMPION:
                        {
                            rpc.LabelText.Text = db.Champions[id].Name;
                            Program.image_cache.AddRunePreviewSubscriber(db.Champions[id].Hash, rpc);
                        }
                        break;
                    case DataElement.ElementType.SPELL:
                        {
                            rpc.LabelText.Text = db.Spells[id].Name;
                            Program.image_cache.AddRunePreviewSubscriber(db.Spells[id].Hash, rpc);
                        }
                        break;
                    case DataElement.ElementType.RELIC:
                        {
                            rpc.LabelText.Text = db.Relics[id].Name;
                            Program.image_cache.AddRunePreviewSubscriber(db.Relics[id].Hash, rpc);
                        }
                        break;
                    case DataElement.ElementType.EQUIPMENT:
                        {
                            rpc.LabelText.Text = db.Equipments[id].Name;
                            Program.image_cache.AddRunePreviewSubscriber(db.Equipments[id].Hash, rpc);
                        }
                        break;
                    default:
                        break;
                }
            }
            for(int i = rune_count; i < PanelRuneIcons.Controls.Count; i++)
            {
                PanelRuneIcons.Controls[i].Hide();
            }
        }

        private void AddBGRune(Database db, BattleGroup bg, BattleGroupStats bgs, Pox.DataElement.ElementType rune_type, int rune_id)
        {
            if(bg.GetRuneCount() >= 30)
            {
                return;
            }

            int runepreview_width = 60;
            int runepreview_height = 81;

            // 1. get rune from db
            Rune r = null;
            switch (rune_type)
            {
                case DataElement.ElementType.CHAMPION:
                    {
                        r = (db.Champions.ContainsKey(rune_id) ? db.Champions[rune_id] : null);
                    }
                    break;
                case DataElement.ElementType.SPELL:
                    {
                        r = (db.Spells.ContainsKey(rune_id) ? db.Spells[rune_id] : null);
                    }
                    break;
                case DataElement.ElementType.RELIC:
                    {
                        r = (db.Relics.ContainsKey(rune_id) ? db.Relics[rune_id] : null);
                    }
                    break;
                case DataElement.ElementType.EQUIPMENT:
                    {
                        r = (db.Equipments.ContainsKey(rune_id) ? db.Equipments[rune_id] : null);
                    }
                    break;
                default:
                    break;
            }
            if (r == null)
            {
                return;
            }

            // 2. place the rune in the BG and determine where to place it in BG panel
            int rune_panel_index = 0;
            bool placed_rune = false;
            switch (rune_type)
            {
                case DataElement.ElementType.CHAMPION:
                    {
                        ChampionBG cbg = new ChampionBG();
                        cbg.ChampionID = r.ID;
                        cbg.Ability1 = ((Champion)r).UpgradeAbilities1_refs[((Champion)r).DefaultUpgrade1Index].ID;
                        cbg.Ability2 = ((Champion)r).UpgradeAbilities2_refs[((Champion)r).DefaultUpgrade2Index].ID;

                        for (int i = 0; i < bg.Champions.Count; i++)
                        {
                            int compare_result = r.Name.CompareTo(db.Champions[bg.Champions[i].ChampionID].Name);
                            if(compare_result < 0)
                            {
                                bg.Champions.Insert(i, cbg);
                                rune_panel_index = i;
                                placed_rune = true;
                                break;
                            }
                        }

                        if(!placed_rune)
                        {
                            bg.Champions.Add(cbg);
                            rune_panel_index = bg.Champions.Count - 1;
                        }
                    }
                    break;
                case DataElement.ElementType.SPELL:
                    {
                        for (int i = 0; i < bg.Spells.Count; i++)
                        {
                            int compare_result = r.Name.CompareTo(db.Spells[bg.Spells[i]].Name);
                            if (compare_result < 0)
                            {
                                bg.Spells.Insert(i, r.ID);
                                rune_panel_index = i + bg.Champions.Count;
                                placed_rune = true;
                                break;
                            }
                        }

                        if(!placed_rune)
                        {
                            bg.Spells.Add(r.ID);
                            rune_panel_index = bg.Champions.Count + bg.Spells.Count - 1;
                        }
                    }
                    break;
                case DataElement.ElementType.RELIC:
                    {
                        for (int i = 0; i < bg.Relics.Count; i++)
                        {
                            int compare_result = r.Name.CompareTo(db.Relics[bg.Relics[i]].Name);
                            if (compare_result < 0)
                            {
                                bg.Relics.Insert(i, r.ID);
                                rune_panel_index = i + bg.Champions.Count + bg.Spells.Count;
                                placed_rune = true;
                                break;
                            }
                        }

                        if(!placed_rune)
                        {
                            bg.Relics.Add(r.ID);
                            rune_panel_index = bg.Champions.Count + bg.Spells.Count + bg.Relics.Count - 1;
                        }
                    }
                    break;
                case DataElement.ElementType.EQUIPMENT:
                    {
                        for (int i = 0; i < bg.Equipments.Count; i++)
                        {
                            int compare_result = r.Name.CompareTo(db.Equipments[bg.Equipments[i]].Name);
                            if (compare_result < 0)
                            {
                                bg.Equipments.Insert(i, r.ID);
                                rune_panel_index = i + bg.Champions.Count + bg.Spells.Count + bg.Relics.Count;
                                placed_rune = true;
                                break;
                            }
                        }

                        if (!placed_rune)
                        {
                            bg.Equipments.Add(r.ID);
                            rune_panel_index = bg.Champions.Count + bg.Spells.Count + bg.Relics.Count + bg.Equipments.Count - 1;
                        }
                    }
                    break;
                default:
                    break;
            }

            // 3. add a rune in the BG panel
            RunePreviewControl rpc = new RunePreviewControl();
            PanelRuneList.Controls.Add(rpc);
            rpc.SetMiniMode(true);
            rpc.Size = new Size(runepreview_width, runepreview_height);
            rpc.RunePreviewImage.MouseDown += PreviewImage_MouseClick;
            RepositionBGRunes(db, bg);
            SelectBGRune(rune_panel_index);
            ParseBattlegroupRanked(db, bg, bgs);

            // 4. update factions
            if(bg.GetRuneCount() == 30)
            {
                FindAvailableFactionCombos(bg, bgs);
            }
        }

        private void SelectBGRune(int rune_index)
        {
            foreach(RunePreviewControl rpc in PanelRuneList.Controls)
                rpc.SetBGColor(SystemColors.Control);

            SelectedRuneIndex = rune_index;
            if (SelectedRuneIndex != Utility.NO_INDEX)
                ((RunePreviewControl)PanelRuneList.Controls[rune_index]).SetBGColor(SystemColors.Highlight);

            if(rune_index == Utility.NO_INDEX)
            {
                RuneDescription.ClearDescription();
            }
            else
            {
                Rune r = BG.GetRune(Program.database, rune_index);
                if (r is Champion)
                {
                    // if the rune is a champion, then rune index is also champion index in the bg
                    ChampionBG cbg = BG.Champions[rune_index];
                    int u1_index = Utility.NO_INDEX;
                    int u2_index = Utility.NO_INDEX;
                    for (int i = 0; i < ((Champion)r).UpgradeAbilities1_refs.Count; i++)
                    {
                        Ability ab = ((Champion)r).UpgradeAbilities1_refs[i];
                        if (ab.ID == cbg.Ability1)
                        {
                            u1_index = i;
                            break;
                        }
                    }
                    for (int i = 0; i < ((Champion)r).UpgradeAbilities2_refs.Count; i++)
                    {
                        Ability ab = ((Champion)r).UpgradeAbilities2_refs[i];
                        if (ab.ID == cbg.Ability2)
                        {
                            u2_index = i;
                            break;
                        }
                    }

                    RuneDescription.ShowAbilitySelection(u1_index, u2_index);
                    RuneDescription.SetChampionRune((Champion)r);
                }
                else if (r is Spell)
                    RuneDescription.SetSpellRune((Spell)r);
                else if (r is Relic)
                    RuneDescription.SetRelicRune((Relic)r);
                else if (r is Equipment)
                    RuneDescription.SetEquipmentRune((Equipment)r);
                else
                    RuneDescription.ClearDescription();
            }
        }

        private void RemoveBGRune(Database db, BattleGroup bg, BattleGroupStats bgs, int rune_index)
        {
            if (rune_index >= bg.GetRuneCount())
            {
                return;
            }

            // remove rune from bg
            if (rune_index < bg.Champions.Count)
            {
                bg.Champions.RemoveAt(rune_index);
            }
            else
            {
                rune_index -= bg.Champions.Count;
                if(rune_index < bg.Spells.Count)
                {
                    bg.Spells.RemoveAt(rune_index);
                }
                else
                {
                    rune_index -= bg.Spells.Count;
                    if (rune_index < bg.Relics.Count)
                    {
                        bg.Relics.RemoveAt(rune_index);
                    }
                    else
                    {
                        rune_index -= bg.Relics.Count;
                        if (rune_index < bg.Equipments.Count)
                        {
                            bg.Equipments.RemoveAt(rune_index);
                        }
                    }
                }
            }

            SelectBGRune(Utility.NO_INDEX);
            // remove rune from panel
            PanelRuneList.Controls.RemoveAt(PanelRuneList.Controls.Count - 1);
            RepositionBGRunes(db, bg);
            ParseBattlegroupRanked(db, bg, bgs);

            if (bg.GetRuneCount() != 30)
            {
                ResetBGFactions(bg);
            }
        }

        private void RepositionBGRunes(Database db, BattleGroup bg)
        {
            int runepreview_width = 60;
            int runepreview_height = 81;
            int runepreview_cur_offset_x = 0;
            int runepreview_cur_offset_y = 0;
            int rune_cur_index = 0;

            for(int i = 0; i < bg.GetRuneCount(); i++)
            {
                Rune r = bg.GetRune(db, i);

                RunePreviewControl rpc = (RunePreviewControl)(PanelRuneList.Controls[rune_cur_index]);
                rpc.ElemID = rune_cur_index;
                rpc.LabelText.Text = r != null ? r.Name : "<missing>";
                rpc.Location = new Point(runepreview_cur_offset_x, runepreview_cur_offset_y);

                Program.image_cache.AddRunePreviewSubscriber(r != null ? r.Hash : "", rpc);

                runepreview_cur_offset_x += runepreview_width;
                rune_cur_index += 1;
                if (rune_cur_index == 15)
                {
                    runepreview_cur_offset_x = 0;
                    runepreview_cur_offset_y += runepreview_height;
                }
            }

        }

        private void SetPanelBG(Database db, BattleGroup bg)
        {
            int runepreview_width = 60;
            int runepreview_height = 81;

            PanelRuneList.Controls.Clear();
            for(int i = 0; i < bg.GetRuneCount(); i++)
            {
                RunePreviewControl rpc = new RunePreviewControl();
                PanelRuneList.Controls.Add(rpc);
                rpc.SetMiniMode(true);
                rpc.Size = new Size(runepreview_width, runepreview_height);
                rpc.RunePreviewImage.MouseDown += PreviewImage_MouseClick;
            }

            SelectBGRune(Utility.NO_INDEX);
            RepositionBGRunes(db, bg);
        }

        private void FindAvailableFactionCombos(BattleGroup bg, BattleGroupStats bgs)
        {
            ResetBGFactions(bg);

            if (bg.GetRuneCount() == 30)
            {
                int HalfFactionNum = 0;
                int FullFactionNum = 0;
                foreach (var s in FactionToName)
                {
                    if (bgs.RunesPerFaction.ContainsKey(s))
                    {
                        if (bgs.RunesPerFaction[s] == 30)
                        {
                            FullFactionNum += 1;
                        }
                        else if (bgs.RunesPerFaction[s] >= 15)
                        {
                            HalfFactionNum += 1;
                        }
                    }
                }

                if (FullFactionNum > 1)
                {
                    return;
                }
                if(FullFactionNum == 1)
                {
                    bg.SelectedAvatar = Utility.NO_INDEX;
                    bg.SelectedFactions = Utility.NO_INDEX;
                }
                if(FullFactionNum == 0)
                {
                    if(HalfFactionNum < 2)
                    {
                        return;
                    }

                    // combo avatar
                    foreach(var s in FactionToName)
                    {
                        if (bgs.RunesPerFaction.ContainsKey(s))
                        {
                            if (bgs.RunesPerFaction[s] >= 15)
                                ComboAvatar.Items.Add(FactionNameToFactionShortName[s]);
                        }
                    }
                    ComboAvatar.SelectedIndex = Utility.NO_INDEX;
                    ComboAvatar.SelectedIndex = 0;

                    // combo factions
                    if(HalfFactionNum > 2)
                    {
                        for(int i = 0; i < FactionToName.Count; i++)
                        {
                            string s1 = FactionToName[i];
                            if ((!bgs.RunesPerFaction.ContainsKey(s1)) || (bgs.RunesPerFaction[s1] < 15))
                                continue;

                            for(int j = i+1; j < FactionToName.Count; j++)
                            {
                                string s2 = FactionToName[j];
                                if ((!bgs.RunesPerFaction.ContainsKey(s2)) || (bgs.RunesPerFaction[s2] < 15))
                                    continue;

                                ComboFactions.Items.Add(string.Format("{0}/{1}", FactionNameToFactionShortName[s1], FactionNameToFactionShortName[s2]));
                            }
                        }

                        ComboFactions.SelectedIndex = Utility.NO_INDEX;
                        ComboFactions.SelectedIndex = 0;
                    }
                }
            }
        }

        private void ResetBGFactions(BattleGroup bg)
        {
            bg.SelectedAvatar = Utility.NO_INDEX;
            bg.SelectedFactions = Utility.NO_INDEX;
            ComboAvatar.Items.Clear();
            ComboFactions.Items.Clear();
        }

        private string GetBGCode(BattleGroup bg)
        {
            byte[] bg_data = BG.ToRawMemory();
            if (bg_data == null)
            {
                return "";
            }

            return Convert.ToBase64String(bg_data);
        }

        private void BattlegroupBuilder_Load(object sender, EventArgs e)
        {
            // generate available faction names list
            FactionToName = Program.database.Factions.AllowedStrings.ToList();
            FactionToName.Sort();
            // hardcoded short names
            FactionNameToFactionShortName.Add("Forglar Swamp", "FS");
            FactionNameToFactionShortName.Add("Forsaken Wastes", "FW");
            FactionNameToFactionShortName.Add("Ironfist Stronghold", "IS");
            FactionNameToFactionShortName.Add("K'thir Forest", "KF");
            FactionNameToFactionShortName.Add("Sundered Lands", "SL");
            FactionNameToFactionShortName.Add("Shattered Peaks", "SP");
            FactionNameToFactionShortName.Add("Savage Tundra", "ST");
            FactionNameToFactionShortName.Add("Underdepths", "UD");

            // card costs per rarity
            CostPerRarity.Add("COMMON", new Tuple<int, int>(2, 4));
            CostPerRarity.Add("UNCOMMON", new Tuple<int, int>(4, 8));
            CostPerRarity.Add("RARE", new Tuple<int, int>(20, 40));
            CostPerRarity.Add("EXOTIC", new Tuple<int, int>(100, 250));
            CostPerRarity.Add("LEGENDARY", new Tuple<int, int>(150, 400));

            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
            RuneDescription.database_ref = Program.database;
            RuneDescription.SetHeight(RuneDescription.Height - 1);
            RuneDescription.SetHeight(RuneDescription.Height + 1);
            RuneDescription.UpgradeClicked = RuneDescription_AbilityClicked;

            CalculateRunePreviewPageSize();
            // rune list is updated via Activated event
        }

        private void TextboxBGBCode_TextChanged(object sender, EventArgs e)
        {
            if (TextboxBGBCode.Text == "")
                return;

            byte[] str2b64array = null;
            try
            {
                str2b64array = Convert.FromBase64String(TextboxBGBCode.Text);
            }
            catch(Exception ex)
            {
                return;
            }

            BattleGroup bg = new BattleGroup();
            if(!bg.FromRawMemory(str2b64array))
            {
                return;
            }

            // update battlegroup
            BG = bg;
            SortBattlegroup(Program.database, BG);
            ParseBattlegroupRanked(Program.database, BG, BGStats);


            // update UI
            SetPanelBG(Program.database, BG);

            if (BG.GetRuneCount() == 30)
            {
                int f1 = BG.SelectedAvatar;
                int f2 = BG.SelectedFactions;
                FindAvailableFactionCombos(BG, BGStats);
                ComboAvatar.SelectedIndex = f1;
                ComboFactions.SelectedIndex = f2;
            }
        }

        private void ButtonGenerateCode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GetBGCode(BG));
        }

        // when BG rune is clicked
        private void PreviewImage_MouseClick(object sender, MouseEventArgs e)
        {
            int id = ((RunePreviewControl)(((PictureBox)sender).Parent)).ElemID;

            if (e.Button == MouseButtons.Right)
            {
                RemoveBGRune(Program.database, BG, BGStats, id);
            }
            else if(e.Button == MouseButtons.Left)
            {
                SelectBGRune(id);
            }
        }

        // when rune page preview image is clicked
        private void RunePageImage_MouseClick(object sender, MouseEventArgs e)
        {
            int id = ((RunePreviewControl)(((PictureBox)sender).Parent)).ElemID;

            if(e.Button == MouseButtons.Right)
            {
                AddBGRune(Program.database, BG, BGStats, RunePageType, id);
            }
            else if (e.Button == MouseButtons.Left)
            {
                SelectBGRune(Utility.NO_INDEX);
                switch (RunePageType)
                {
                    case DataElement.ElementType.CHAMPION:
                        {
                            if (Program.database.Champions.ContainsKey(id))
                                RuneDescription.SetChampionRune(Program.database.Champions[id]);
                        }
                        break;
                    case DataElement.ElementType.SPELL:
                        {
                            if (Program.database.Spells.ContainsKey(id))
                                RuneDescription.SetSpellRune(Program.database.Spells[id]);
                        }
                        break;
                    case DataElement.ElementType.RELIC:
                        {
                            if (Program.database.Relics.ContainsKey(id))
                                RuneDescription.SetRelicRune(Program.database.Relics[id]);
                        }
                        break;
                    case DataElement.ElementType.EQUIPMENT:
                        {
                            if (Program.database.Equipments.ContainsKey(id))
                                RuneDescription.SetEquipmentRune(Program.database.Equipments[id]);
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }

            }
        }

        private void ComboRuneType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ComboRuneType.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            DataElement.ElementType elem = DataElement.ElementType.NONE;

            string s = ComboRuneType.SelectedItem.ToString();
            if (s == "Champions")
                elem = DataElement.ElementType.CHAMPION;
            else if (s == "Spells")
                elem = DataElement.ElementType.SPELL;
            else if (s == "Relics")
                elem = DataElement.ElementType.RELIC;
            else if (s == "Equipments")
                elem = DataElement.ElementType.EQUIPMENT;

            if (elem == DataElement.ElementType.NONE)
                return;
            if (elem == RunePageType)
                return;

            RunePageType = elem;
            UpdateRuneList(Program.database);
        }

        private void ButtonPreviousPage_Click(object sender, EventArgs e)
        {
            if (RunePageCount == 0)
                return;

            RuneCurrentPage -= 1;
            if (RuneCurrentPage < 0)
                RuneCurrentPage += RunePageCount;

            DisplayRunePage(Program.database, RuneCurrentPage);
        }

        private void ButtonNextPage_Click(object sender, EventArgs e)
        {
            if (RunePageCount == 0)
                return;

            RuneCurrentPage += 1;
            if (RuneCurrentPage >= RunePageCount)
                RuneCurrentPage -= RunePageCount;

            DisplayRunePage(Program.database, RuneCurrentPage);
        }

        private void BattlegroupBuilder_Activated(object sender, EventArgs e)
        {
            UpdateRuneList(Program.database);
        }

        private void CheckboxApplyFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRuneList(Program.database);
        }

        private void BattlegroupBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
            Program.image_cache.BreakRunePreviewDownload();
        }

        private void RuneDescription_AbilityClicked(int upgrade_id, int upgrade_index)
        {
            if(SelectedRuneIndex == Utility.NO_INDEX)
            {
                return;
            }

            // set upgrades of selected champion
            if(upgrade_id == 1)
            {
                ChampionBG cbg = BG.Champions[SelectedRuneIndex];
                cbg.Ability1 = ((Champion)BG.GetRune(Program.database, SelectedRuneIndex)).UpgradeAbilities1_refs[upgrade_index].ID;
                BG.Champions[SelectedRuneIndex] = cbg;
            }
            else if (upgrade_id == 2)
            {
                ChampionBG cbg = BG.Champions[SelectedRuneIndex];
                cbg.Ability2 = ((Champion)BG.GetRune(Program.database, SelectedRuneIndex)).UpgradeAbilities2_refs[upgrade_index].ID;
                BG.Champions[SelectedRuneIndex] = cbg;
            }

            ParseBattlegroupRanked(Program.database, BG, BGStats);
        }

        private void ComboAvatar_SelectedIndexChanged(object sender, EventArgs e)
        {
            BG.SelectedAvatar = ComboAvatar.SelectedIndex;
        }

        private void ComboFactions_SelectedIndexChanged(object sender, EventArgs e)
        {
            BG.SelectedFactions = ComboFactions.SelectedIndex;
        }

        private void newBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextboxBGBCode.Text = "";
            BG = new BattleGroup();
            BGStats = new BattleGroupStats();
            OpenedBattlegroup = "";
            SelectedRuneIndex = Utility.NO_INDEX;

            ParseBattlegroupRanked(Program.database, BG, BGStats);

            // update UI
            SetPanelBG(Program.database, BG);

            if (BG.GetRuneCount() == 30)
            {
                int f1 = BG.SelectedAvatar;
                int f2 = BG.SelectedFactions;
                FindAvailableFactionCombos(BG, BGStats);
                ComboAvatar.SelectedIndex = f1;
                ComboFactions.SelectedIndex = f2;
            }
        }

        private void loadBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BGBLoadForm load = new BGBLoadForm();
            load.ShowDialog();

            if(load.SelectedName != "")
            {
                OpenedBattlegroup = load.SelectedName;
                TextboxBGBCode.Text = load.SelectedCode;
            }
        }

        private void saveBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BGBSaveForm save = new BGBSaveForm();
            save.SelectedName = OpenedBattlegroup;
            save.SelectedCode = GetBGCode(BG);

            save.ShowDialog();
        }
    }
}
