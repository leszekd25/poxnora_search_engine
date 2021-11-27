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
            public BattleGroupHistogram<string> RunesPerRarity = new BattleGroupHistogram<string>();
            public BattleGroupHistogram<string> RunesPerFaction = new BattleGroupHistogram<string>();
            public BattleGroupHistogramInt ChampionStatDataInt = new BattleGroupHistogramInt();
            public BattleGroupHistogram<string> ChampionStatDataString = new BattleGroupHistogram<string>();
            public BattleGroupHistogram<Ability> ChampionStatDataAbility = new BattleGroupHistogram<Ability>();

            public BattleGroupHistogramInt RuneGroupsPerRune = new BattleGroupHistogramInt();   // used for rune groups


            public int NoraShardCostBuy = 0;
            public int NoraShardCostSell = 0;
            public int RangedChampionCount = 0;
            public int HybridRangeChampionCount = 0;
            public int MeleeChampionCount = 0;
            public int BigChampionCount = 0;
            public int NoAttackChampionCount = 0;
        }

        public enum BGStatsChartType { DAMAGE = 0, SPEED, MINRANGE, MAXRANGE, COMPOUNDRANGE, DEFENSE, HEALTH, NORACOST, ABILITIES, CLASSES, RACES }


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
        bool ShowRuneFilter = true;

        // static cache
        List<string> FactionToName = new List<string>();
        Dictionary<string, string> FactionNameToFactionShortName = new Dictionary<string, string>();
        Dictionary<string, Tuple<int, int>> CostPerRarity = new Dictionary<string, Tuple<int, int>>();  // key: rarity name, value: (sell, buy)
        BGStatsChartType ChartType = BGStatsChartType.DAMAGE;
        bool IsHistogram = true;

        // dynamic cache
        Dictionary<int, bool> NoBasicAttackChampCache = new Dictionary<int, bool>();

        public BattlegroupBuilder()
        {
            InitializeComponent();
            PageRuneBrowser.MouseWheel += BattlegroupBuilder_MouseWheel;
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
                if (!c.AllowRanked)
                {
                    messages.Add(string.Format("{0} is banned from ranked play", c.Name));
                }

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
                else if (!db.Spells[bg.Spells[i]].AllowRanked)
                {
                    messages.Add(string.Format("{0} is banned from ranked play", db.Spells[bg.Spells[i]].Name));
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
                else if (!db.Relics[bg.Relics[i]].AllowRanked)
                {
                    messages.Add(string.Format("{0} is banned from ranked play", db.Relics[bg.Relics[i]].Name));
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
                else if (!db.Equipments[bg.Equipments[i]].AllowRanked)
                {
                    messages.Add(string.Format("{0} is banned from ranked play", db.Equipments[bg.Equipments[i]].Name));
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

            // 5. hardcoded rune group check
            ParseBattlegroupRuneGroups(db, bg, bgs, messages);

            // 6. calculate stats
            CalculateBattlegroupStats(db, bg, bgs, calculate_cost_stats, calculate_distribution_stats);

            // 7. check inconsistencies regarding factions (not needed if there arent 30 runes in the bg)
            if (bg.GetRuneCount() == 30)
            {
                // needs to have either 30 runes from one faction, or at least 2 factions with more than 15 runes
                int HalfFactionNum = 0;
                int FullFactionNum = 0;
                foreach (var s in bgs.RunesPerFaction.GetKeyList())
                {
                    if (bgs.RunesPerFaction.GetKeyValue(s) == 30)
                    {
                        FullFactionNum += 1;
                    }
                    else if (bgs.RunesPerFaction.GetKeyValue(s) >= 15)
                    {
                        HalfFactionNum += 1;
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
                StatusText.Text = "BG can be used in ranked";
            }
            else
            {
                StatusText.Text = "BG can't be used in ranked - see Battlegroup Log page";

                foreach (var s in messages)
                    ListBoxBGErrorLog.Items.Add(s);
            }

            SetChartMode(ChartType);
        }

        private void ParseBattlegroupRuneGroups(Database db, BattleGroup bg, BattleGroupStats bgs, List<string> messages)
        {
            if(!db.Plugin_RuneGroups.Ready)
            {
                messages.Add("DATABASE WARNING: Rune groups are not loaded, can't check for rune group errors");
                return;
            }

            int offset = 0;

            // champions
            bgs.RuneGroupsPerRune.Clear();
            for (int i = 0; i < bg.Champions.Count; i++)
            {
                int id = bg.Champions[i].ChampionID;
                RuneGroup rg = db.Plugin_RuneGroups.Champions.GetGroup(id);
                if (rg == null)
                    continue;

                bgs.RuneGroupsPerRune.Add(rg.ID);
            }
            foreach (var rg_id in bgs.RuneGroupsPerRune.DataCount.Keys)
            {
                RuneGroup rg = db.Plugin_RuneGroups.Champions.RuneGroups[rg_id];
                if (rg.RuneLimit < bgs.RuneGroupsPerRune.GetKeyValue(rg_id))
                {
                    messages.Add(string.Format("Too many champions of group {0}: maximum allowed {1}, found {2}",
                        rg.Name, rg.RuneLimit, bgs.RuneGroupsPerRune.GetKeyValue(rg_id)));
                }
            }

            // spells
            bgs.RuneGroupsPerRune.Clear();
            for (int i = 0; i < bg.Spells.Count; i++)
            {
                int id = bg.Spells[i];
                RuneGroup rg = db.Plugin_RuneGroups.Spells.GetGroup(id);
                if (rg == null)
                    continue;

                bgs.RuneGroupsPerRune.Add(rg.ID);
            }
            foreach (var rg_id in bgs.RuneGroupsPerRune.DataCount.Keys)
            {
                RuneGroup rg = db.Plugin_RuneGroups.Spells.RuneGroups[rg_id];
                if (rg.RuneLimit < bgs.RuneGroupsPerRune.GetKeyValue(rg_id))
                {
                    messages.Add(string.Format("Too many spells of group {0}: maximum allowed {1}, found {2}",
                        rg.Name, rg.RuneLimit, bgs.RuneGroupsPerRune.GetKeyValue(rg_id)));
                }
            }

            // relics
            bgs.RuneGroupsPerRune.Clear();
            for (int i = 0; i < bg.Relics.Count; i++)
            {
                int id = bg.Relics[i];
                RuneGroup rg = db.Plugin_RuneGroups.Relics.GetGroup(id);
                if (rg == null)
                    continue;

                bgs.RuneGroupsPerRune.Add(rg.ID);
            }
            foreach (var rg_id in bgs.RuneGroupsPerRune.DataCount.Keys)
            {
                RuneGroup rg = db.Plugin_RuneGroups.Relics.RuneGroups[rg_id];
                if (rg.RuneLimit < bgs.RuneGroupsPerRune.GetKeyValue(rg_id))
                {
                    messages.Add(string.Format("Too many relics of group {0}: maximum allowed {1}, found {2}",
                        rg.Name, rg.RuneLimit, bgs.RuneGroupsPerRune.GetKeyValue(rg_id)));
                }
            }

            // equipments
            bgs.RuneGroupsPerRune.Clear();
            for (int i = 0; i < bg.Equipments.Count; i++)
            {
                int id = bg.Equipments[i];
                RuneGroup rg = db.Plugin_RuneGroups.Equipments.GetGroup(id);
                if (rg == null)
                    continue;

                bgs.RuneGroupsPerRune.Add(rg.ID);
            }
            foreach (var rg_id in bgs.RuneGroupsPerRune.DataCount.Keys)
            {
                RuneGroup rg = db.Plugin_RuneGroups.Equipments.RuneGroups[rg_id];
                if (rg.RuneLimit < bgs.RuneGroupsPerRune.GetKeyValue(rg_id))
                {
                    messages.Add(string.Format("Too many equipments of group {0}: maximum allowed {1}, found {2}",
                        rg.Name, rg.RuneLimit, bgs.RuneGroupsPerRune.GetKeyValue(rg_id)));
                }
            }
        }

        private void CalculateBattlegroupStats(Database db, BattleGroup bg, BattleGroupStats bgs, bool calc_cost, bool calc_distribution)
        {
            bgs.RunesPerFaction.Clear();
            bgs.RunesPerRarity.Clear();
            bgs.ChampionStatDataInt.Clear();
            bgs.ChampionStatDataString.Clear();
            bgs.ChampionStatDataAbility.Clear();
            bgs.NoraShardCostBuy = 0;
            bgs.NoraShardCostSell = 0;
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
                        bgs.RunesPerFaction.Add(FactionNameToFactionShortName[f]);
                        // todo: deal with duplicate factions in a few runes (example: kiergana)
                    }

                    bgs.RunesPerRarity.Add(r.Rarity);
                }

                Tuple<int, int> rune_price = GetRuneShardCost(r);
                bgs.NoraShardCostBuy += rune_price.Item2;
                bgs.NoraShardCostSell += rune_price.Item1;
            }

            // calculate average champion stats
            // only champs which exist in DB count
            foreach(var cbg in bg.Champions)
            {
                Champion c = null;
                if (db.Champions.ContainsKey(cbg.ChampionID))
                    c = db.Champions[cbg.ChampionID];
                else
                    continue;

                if (c.MaxRNG <= 1)
                    bgs.MeleeChampionCount += 1;
                if (c.MinRNG > 1)
                    bgs.RangedChampionCount += 1;
                if ((c.MinRNG <= 1) && (c.MaxRNG > 1))
                    bgs.HybridRangeChampionCount += 1;
                if (c.Size == 2)
                    bgs.BigChampionCount += 1;
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

            string label_txt1 = "";
            string label_txt2 = "";
            foreach (var key in bgs.RunesPerFaction.GetKeyList())
            {
                label_txt1 += string.Format("{0}: {1})             ", key, bgs.RunesPerFaction.GetKeyValue(key));
            }
            LabelFactionCounts.Text = label_txt1;

            label_txt1 = "";
            label_txt2 = "";
            foreach(var key in FactionNameToFactionShortName.Values)
            {
                label_txt1 += string.Format("{0}:\r\n", key);
                label_txt2 += string.Format("{0}:\r\n", bgs.RunesPerFaction.GetKeyValue(key));
            }
            LabelRuneFactions.Text = label_txt1;
            LabelRuneCountByFaction.Text = label_txt2;

            LabelRuneCountByRarity.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}",
                bgs.RunesPerRarity.GetKeyValue("COMMON"),
                bgs.RunesPerRarity.GetKeyValue("UNCOMMON"),
                bgs.RunesPerRarity.GetKeyValue("RARE"),
                bgs.RunesPerRarity.GetKeyValue("EXOTIC"),
                bgs.RunesPerRarity.GetKeyValue("LEGENDARY"),
                bgs.RunesPerRarity.GetKeyValue("LIMITED"));

            LabelBGShardCost.Text = string.Format("BG shard cost: {0}/{1} (buy/sell)",
                bgs.NoraShardCostBuy,
                bgs.NoraShardCostSell);

            LabelChampionCounts.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}",
                bgs.MeleeChampionCount,
                bgs.HybridRangeChampionCount,
                bgs.RangedChampionCount,
                bgs.BigChampionCount);
        }

        void SetChartMode(BGStatsChartType type)
        {
            ChartType = type;
            bool is_histogram = (RadioHistogram.Checked);

            PanelHistogramElements.Hide();
            PanelChartMode.Hide();
            ChartStatHistogram.Hide();
            RadioHistogram.Text = "Histogram";
            RadioList.Text = "List";

            switch (type)
            {
                case BGStatsChartType.DAMAGE:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.Damage);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 6;
                        BGStats.ChampionStatDataInt.RangeEnd = 14;
                        BGStats.ChampionStatDataInt.RangeStep = 2;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Damage";

                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                        }
                        else
                        {
                            foreach(int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.SPEED:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.Speed);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 5;
                        BGStats.ChampionStatDataInt.RangeEnd = 7;
                        BGStats.ChampionStatDataInt.RangeStep = 1;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Speed";

                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.MINRANGE:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.MinRNG);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 2;
                        BGStats.ChampionStatDataInt.RangeEnd = 6;
                        BGStats.ChampionStatDataInt.RangeStep = 1;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Min RNG";

                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                            // fix for range 1
                            ChartStatHistogram.Series["Values"].Points[0].AxisLabel = "1";
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.MAXRANGE:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.MaxRNG);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 2;
                        BGStats.ChampionStatDataInt.RangeEnd = 6;
                        BGStats.ChampionStatDataInt.RangeStep = 1;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Max RNG";


                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                            // fix for range 1
                            ChartStatHistogram.Series["Values"].Points[0].AxisLabel = "1";
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.COMPOUNDRANGE:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            for (int j = c.MinRNG; j <= c.MaxRNG; j++)
                                BGStats.ChampionStatDataInt.Add(j);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 2;
                        BGStats.ChampionStatDataInt.RangeEnd = 6;
                        BGStats.ChampionStatDataInt.RangeStep = 1;

                        BGStats.ChampionStatDataInt.Update();

                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Compound RNG";

                        for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                        {
                            ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                        }
                        // fix for range 1
                        ChartStatHistogram.Series["Values"].Points[0].AxisLabel = "1";

                    }
                    break;
                case BGStatsChartType.DEFENSE:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.Defense);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 1;
                        BGStats.ChampionStatDataInt.RangeEnd = 3;
                        BGStats.ChampionStatDataInt.RangeStep = 1;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Defense";


                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                            // fix for range 1
                            ChartStatHistogram.Series["Values"].Points[0].AxisLabel = "0";
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.HEALTH:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            BGStats.ChampionStatDataInt.Add(c.HitPoints);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 40;
                        BGStats.ChampionStatDataInt.RangeEnd = 55;
                        BGStats.ChampionStatDataInt.RangeStep = 5;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Hit points";

                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.NORACOST:
                    {
                        BGStats.ChampionStatDataInt.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            int nora_cost = c.BaseNoraCost;
                            foreach (var ab in c.BaseAbilities_refs)
                                nora_cost += ab.NoraCost;
                            ChampionBG cbg = BG.Champions[i];
                            if (Program.database.Abilities.ContainsKey(cbg.Ability1))
                                nora_cost += Program.database.Abilities[cbg.Ability1].NoraCost;
                            if (Program.database.Abilities.ContainsKey(cbg.Ability2))
                                nora_cost += Program.database.Abilities[cbg.Ability2].NoraCost;

                            BGStats.ChampionStatDataInt.Add(nora_cost);
                        }

                        BGStats.ChampionStatDataInt.RangeStart = 55;
                        BGStats.ChampionStatDataInt.RangeEnd = 80;
                        BGStats.ChampionStatDataInt.RangeStep = 5;

                        BGStats.ChampionStatDataInt.Update();

                        PanelChartMode.Show();
                        ChartStatHistogram.Show();
                        ChartStatHistogram.Series["Values"].Points.Clear();
                        ChartStatHistogram.Series["Values"].LegendText = "Nora cost";

                        if (is_histogram)
                        {
                            for (int i = 0; i < BGStats.ChampionStatDataInt.GetColumnCount(); i++)
                            {
                                ChartStatHistogram.Series["Values"].Points.AddXY(BGStats.ChampionStatDataInt.GetColumnLabel(i), BGStats.ChampionStatDataInt.GetColumnValue(i));
                            }
                        }
                        else
                        {
                            foreach (int val in BGStats.ChampionStatDataInt.GetValues())
                            {
                                ChartStatHistogram.Series["Values"].Points.AddY(val);
                            }
                        }
                    }
                    break;
                case BGStatsChartType.ABILITIES:
                {
                    BGStats.ChampionStatDataAbility.Clear();
                    for (int i = 0; i < BG.Champions.Count; i++)
                    {
                        Rune r = BG.GetRune(Program.database, i);
                        if (r == null)
                            continue;
                        Champion c = (Champion)r;
                        ChampionBG cbg = BG.Champions[i];

                        foreach (var ab in c.BaseAbilities_refs)
                            BGStats.ChampionStatDataAbility.Add(ab);
                        BGStats.ChampionStatDataAbility.Add(Program.database.Abilities[cbg.Ability1]);
                        BGStats.ChampionStatDataAbility.Add(Program.database.Abilities[cbg.Ability2]);
                    }

                    PanelChartMode.Show();
                    RadioHistogram.Text = "Sort by name";
                    RadioList.Text = "Sort by count";

                    PanelHistogramElements.Show();
                    PanelHistogramElements.Location = ChartStatHistogram.Location;
                    foreach (var control in PanelHistogramElements.Controls)
                    {
                        if (control is AbilityControl_Histogram)
                            Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl_Histogram)control);
                    }
                    PanelHistogramElements.Controls.Clear();

                    int label_width = 200;
                    int label_height = 38;
                    int label_row_count = PanelHistogramElements.Height / label_height;
                    int label_row = 0;
                    int label_col = 0;
                    foreach (var ab in BGStats.ChampionStatDataAbility.GetKeyList(RadioList.Checked))
                    {
                        AbilityControl_Histogram ach = new AbilityControl_Histogram();
                        PanelHistogramElements.Controls.Add(ach);
                        ach.LabelAbilityNum.Text = BGStats.ChampionStatDataAbility.GetKeyValue(ab).ToString() + "x";
                        ach.LabelAbilityName.Text = ab.ToString();
                        Program.image_cache.AddAbilityImageSubscriber(ab.IconName, ach);
                        ach.Location = new Point(label_col * label_width, label_row * label_height);

                        label_row++;
                        if (label_row == label_row_count)
                        {
                            label_row = 0;
                            label_col += 1;
                        }
                    }
                }
                break;
                case BGStatsChartType.CLASSES:
                    {
                        BGStats.ChampionStatDataString.Clear();
                        for (int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            foreach (var cl in c.Class)
                                BGStats.ChampionStatDataString.Add(cl);
                        }

                        PanelHistogramElements.Show();
                        PanelHistogramElements.Location = ChartStatHistogram.Location;
                        foreach (var control in PanelHistogramElements.Controls)
                        {
                            if (control is AbilityControl)
                                Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl)control);
                        }
                        PanelHistogramElements.Controls.Clear();

                        int label_width = 150;
                        int label_height = 16;
                        int label_row_count = PanelHistogramElements.Height / label_height;
                        int label_row = 0;
                        int label_col = 0;
                        foreach (var cl in BGStats.ChampionStatDataString.GetKeyList())
                        {
                            Label lb_class = new Label();
                            PanelHistogramElements.Controls.Add(lb_class);
                            lb_class.AutoSize = false;
                            lb_class.Width = label_width;
                            lb_class.Height = label_height;
                            lb_class.Font = new Font("Arial", 10);
                            lb_class.Location = new Point(label_col * label_width, label_row * label_height);
                            lb_class.Text = string.Format("{0}x {1}", BGStats.ChampionStatDataString.GetKeyValue(cl), cl);

                            label_row++;
                            if (label_row == label_row_count)
                            {
                                label_row = 0;
                                label_col += 1;
                            }
                        }
                    }
                    break;
                case BGStatsChartType.RACES:
                    {
                        BGStats.ChampionStatDataString.Clear();
                        for(int i = 0; i < BG.Champions.Count; i++)
                        {
                            Rune r = BG.GetRune(Program.database, i);
                            if (r == null)
                                continue;
                            Champion c = (Champion)r;

                            foreach (var race in c.Race)
                                BGStats.ChampionStatDataString.Add(race);
                        }

                        PanelHistogramElements.Show();
                        PanelHistogramElements.Location = ChartStatHistogram.Location;
                        foreach(var control in PanelHistogramElements.Controls)
                        {
                            if (control is AbilityControl)
                                Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl)control);
                        }
                        PanelHistogramElements.Controls.Clear();

                        int label_width = 150;
                        int label_height = 16;
                        int label_row_count = PanelHistogramElements.Height / label_height;
                        int label_row = 0;
                        int label_col = 0;
                        foreach(var race in BGStats.ChampionStatDataString.GetKeyList())
                        {
                            Label lb_race = new Label();
                            PanelHistogramElements.Controls.Add(lb_race);
                            lb_race.AutoSize = false;
                            lb_race.Width = label_width;
                            lb_race.Height = label_height;
                            lb_race.Font = new Font("Arial", 10);
                            lb_race.Location = new Point(label_col * label_width, label_row * label_height);
                            lb_race.Text = string.Format("{0}x {1}", BGStats.ChampionStatDataString.GetKeyValue(race), race);

                            label_row++;
                            if(label_row == label_row_count)
                            {
                                label_row = 0;
                                label_col += 1;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
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

            // done here as well as in updaterunelist (resizes do not update rune list, but still need to update runepage count
            if (RuneList.Count == 0)
            {
                RunePageCount = 0;
            }
            else
            {
                RunePageCount = (RuneList.Count / RunesPerPage) + ((RuneList.Count % RunesPerPage) == 0 ? 0 : 1);
            }
        }

        private void UpdateRuneList()
        {
            Database db = Program.database;
            Pox.Filters.BaseFilter filter = SlideIn_DatabaseFilter.SearchFilter;
            RuneList.Clear();

            switch (RunePageType)
            {
                case DataElement.ElementType.CHAMPION:
                    {
                        foreach (var kv in db.Champions)
                        {
                            if ((filter == null) || (!SlideIn_DatabaseFilter.ApplyFilters) || (filter.Satisfies(kv.Value)))
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
                            if ((filter == null) || (!SlideIn_DatabaseFilter.ApplyFilters) || (filter.Satisfies(kv.Value)))
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
                            if ((filter == null) || (!SlideIn_DatabaseFilter.ApplyFilters) || (filter.Satisfies(kv.Value)))
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
                            if ((filter == null) || (!SlideIn_DatabaseFilter.ApplyFilters) || (filter.Satisfies(kv.Value)))
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

                Rune r = null;
                switch (RunePageType)
                {
                    case DataElement.ElementType.CHAMPION:
                        r = db.Champions[id];
                        break;
                    case DataElement.ElementType.SPELL:
                        r = db.Spells[id];
                        break;
                    case DataElement.ElementType.RELIC:
                        r = db.Relics[id];
                        break;
                    case DataElement.ElementType.EQUIPMENT:
                        r = db.Equipments[id];
                        break;
                    default:
                        break;
                }
                if (r == null)
                    continue;


                rpc.Show();
                rpc.ElemID = id;
                rpc.RunePreviewImage.Image = null;
                rpc.LabelText.Text = r.Name;
                Program.image_cache.AddRunePreviewSubscriber(r.Hash, rpc);

                if (r.Faction.Count == 1)
                {
                    if (r.Faction[0] == "Forglar Swamp")
                    {
                        rpc.SetBGColor(Color.FromArgb(26, 217, 193));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else if (r.Faction[0] == "Forsaken Wastes")
                    {
                        rpc.SetBGColor(Color.FromArgb(82, 71, 95));
                        rpc.SetTXTColor(Color.White);
                    }
                    else if (r.Faction[0] == "Ironfist Stronghold")
                    {
                        rpc.SetBGColor(Color.FromArgb(119, 83, 60));
                        rpc.SetTXTColor(Color.White);
                    }
                    else if (r.Faction[0] == "K'thir Forest")
                    {
                        rpc.SetBGColor(Color.FromArgb(0, 179, 71));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else if (r.Faction[0] == "Sundered Lands")
                    {
                        rpc.SetBGColor(Color.FromArgb(155, 132, 0));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else if (r.Faction[0] == "Shattered Peaks")
                    {
                        rpc.SetBGColor(Color.FromArgb(223, 134, 0));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else if (r.Faction[0] == "Savage Tundra")
                    {
                        rpc.SetBGColor(Color.FromArgb(165, 236, 239));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else if (r.Faction[0] == "Underdepths")
                    {
                        rpc.SetBGColor(Color.FromArgb(231, 49, 49));
                        rpc.SetTXTColor(Color.Black);
                    }
                    else
                    {
                        rpc.SetBGColor(Color.FromArgb(180, 180, 180));
                        rpc.SetTXTColor(Color.Black);
                    }
                }
                else
                {
                    rpc.SetBGColor(Color.FromArgb(180, 180, 180));
                    rpc.SetTXTColor(Color.Black);
                }

            }
            for(int i = rune_count; i < PanelRuneIcons.Controls.Count; i++)
            {
                PanelRuneIcons.Controls[i].Hide();
            }
        }

        private void RuneFilterHide()
        {
            if(!ShowRuneFilter)
            {
                return;
            }

            SlideIn_DatabaseFilter.Hide();
            ButtonToggleFilter.Location = new Point(SlideIn_DatabaseFilter.Location.X, ButtonToggleFilter.Location.Y);
            PanelRuneIcons.Location = new Point(ButtonToggleFilter.Location.X + ButtonToggleFilter.Width + 3, PanelRuneIcons.Location.Y);
            PanelRuneIcons.Width = PageRuneBrowser.Width - ButtonToggleFilter.Width - 12;
            CalculateRunePreviewPageSize();
            DisplayRunePage(Program.database, 0);

            ShowRuneFilter = false;
        }

        private void RuneFilterShow()
        {
            if(ShowRuneFilter)
            {
                return;
            }

            SlideIn_DatabaseFilter.Show();
            ButtonToggleFilter.Location = new Point(SlideIn_DatabaseFilter.Location.X + SlideIn_DatabaseFilter.Width + 3, ButtonToggleFilter.Location.Y);
            PanelRuneIcons.Location = new Point(ButtonToggleFilter.Location.X + ButtonToggleFilter.Width + 3, PanelRuneIcons.Location.Y);
            PanelRuneIcons.Width = PageRuneBrowser.Width - 15 - SlideIn_DatabaseFilter.Width - ButtonToggleFilter.Width;
            CalculateRunePreviewPageSize();
            DisplayRunePage(Program.database, 0);

            ShowRuneFilter = true;
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

            Log.Info(Log.LogSource.BGBuilder, "Adding rune " + r.ToString());
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

                    RuneDescription.SetChampionRune((Champion)r, u1_index, u2_index);
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
                    if (bgs.RunesPerFaction.GetKeyValue(FactionNameToFactionShortName[s]) == 30)
                    {
                        FullFactionNum += 1;
                    }
                    else if (bgs.RunesPerFaction.GetKeyValue(FactionNameToFactionShortName[s]) >= 15)
                    {
                        HalfFactionNum += 1;
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
                        if (bgs.RunesPerFaction.GetKeyValue(FactionNameToFactionShortName[s]) >= 15)
                            ComboAvatar.Items.Add(FactionNameToFactionShortName[s]);
                    }
                    ComboAvatar.SelectedIndex = Utility.NO_INDEX;
                    ComboAvatar.SelectedIndex = 0;

                    // combo factions
                    if(HalfFactionNum > 2)
                    {
                        for(int i = 0; i < FactionToName.Count; i++)
                        {
                            string s1 = FactionToName[i];
                            if (bgs.RunesPerFaction.GetKeyValue(FactionNameToFactionShortName[s1]) < 15)
                                continue;

                            for(int j = i+1; j < FactionToName.Count; j++)
                            {
                                string s2 = FactionToName[j];
                                if (bgs.RunesPerFaction.GetKeyValue(FactionNameToFactionShortName[s2]) < 15)
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

        private string GetBGText(BattleGroup bg)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, int> ChampionNames = new Dictionary<string, int>();
            Dictionary<string, int> SpellNames = new Dictionary<string, int>();
            Dictionary<string, int> RelicNames = new Dictionary<string, int>();
            Dictionary<string, int> EquipmentNames = new Dictionary<string, int>();

            int rune_index = 0;
            string cur_name = "";
            for (int i = 0; i < BG.GetRuneCount(); i++)
            {
                Rune r = BG.GetRune(Program.database, i + rune_index);
                if (r == null)
                    cur_name = "<missing>";
                else
                    cur_name = r.Name;

                Dictionary<string, int> cur_dict = ChampionNames;
                if (i >= BG.Champions.Count)
                    cur_dict = SpellNames;
                if (i >= BG.Champions.Count + BG.Spells.Count)
                    cur_dict = RelicNames;
                if (i >= BG.Champions.Count + BG.Spells.Count + BG.Relics.Count)
                    cur_dict = EquipmentNames;

                if (!cur_dict.ContainsKey(cur_name))
                    cur_dict.Add(cur_name, 0);

                cur_dict[cur_name] += 1;
            }

            List<string> lines = new List<string>();

            foreach (var dict in new Dictionary<string, int>[] { ChampionNames, SpellNames, RelicNames, EquipmentNames })
            {
                lines.Clear();
                foreach (var kv in dict)
                    lines.Add(string.Format("{0}x {1}", kv.Value, kv.Key));
                lines.Sort();
                foreach (var l in lines)
                    sb.AppendLine(l);

                if(dict != EquipmentNames)
                    sb.AppendLine("");
            }

            return sb.ToString();
        }

        private void BattlegroupBuilder_Load(object sender, EventArgs e)
        {
            Log.Info(Log.LogSource.BGBuilder, "BattlegroupBuilder_Load called");

            Program.database.Plugin_RuneGroups.Load();

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

            SlideIn_DatabaseFilter.ApplyFilters_callback = UpdateRuneList;
            RuneFilterHide();

            // CalculateRunePreviewPageSize();   <- calculated via RuneFilterHide()
            UpdateRuneList();
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

            Log.Info(Log.LogSource.BGBuilder, "Loading BG [code: " + TextboxBGBCode.Text + "]");

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
            Log.Info(Log.LogSource.BGBuilder, "Generating BG code");
            Clipboard.SetText(GetBGCode(BG));
            StatusText.Text = "BG code copied to clipboard";
        }

        // when BG rune is clicked
        private void PreviewImage_MouseClick(object sender, MouseEventArgs e)
        {
            int id = ((RunePreviewControl)(((PictureBox)sender).Parent)).ElemID;

            if (e.Button == MouseButtons.Right)
            {

                Log.Info(Log.LogSource.BGBuilder, "Removing rune " + ((RunePreviewControl)(((PictureBox)sender).Parent)).LabelText.Text);
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
                                RuneDescription.SetChampionRune(Program.database.Champions[id],
                                    Program.database.Champions[id].DefaultUpgrade1Index,
                                    Program.database.Champions[id].DefaultUpgrade2Index);
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
            UpdateRuneList();
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

        private void BattlegroupBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Info(Log.LogSource.BGBuilder, "BattlegroupBuilder_FormClosed called");

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
            Log.Info(Log.LogSource.BGBuilder, "Clearing BG");

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
                Log.Info(Log.LogSource.BGBuilder, "Loading BG from file [code: " + load.SelectedCode + "]");
                OpenedBattlegroup = load.SelectedName;
                TextboxBGBCode.Text = load.SelectedCode;
            }
        }

        private void saveBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BGBSaveForm save = new BGBSaveForm();
            save.SelectedName = OpenedBattlegroup;
            save.SelectedCode = GetBGCode(BG);

            Log.Info(Log.LogSource.BGBuilder, "Saving BG to file");
            save.ShowDialog();
        }

        private void ButtonToggleFilter_Click(object sender, EventArgs e)
        {
            if (!ShowRuneFilter)
                RuneFilterShow();
            else
                RuneFilterHide();
        }

        private void ButtonQuickFilter_Click(object sender, EventArgs e)
        {
            SlideIn_DatabaseFilter.InvokeQuickFilter();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboChartMode.SelectedIndex == Utility.NO_INDEX)
                return;

            if (ChartType == (BGStatsChartType)(ComboChartMode.SelectedIndex))
                return;

            SetChartMode((BGStatsChartType)(ComboChartMode.SelectedIndex));
        }


        private void BattlegroupBuilder_MouseWheel(object sender, MouseEventArgs e)
        {
            if (RunePageCount == 0)
                return;

            if(e.Delta > 0)
            {
                RuneCurrentPage -= 1;
                if (RuneCurrentPage < 0)
                    RuneCurrentPage += RunePageCount;
            }
            else if(e.Delta < 0)
            {
                RuneCurrentPage += 1;
                if (RuneCurrentPage >= RunePageCount)
                    RuneCurrentPage -= RunePageCount;
            }

            DisplayRunePage(Program.database, RuneCurrentPage);
        }

        private void ButtonGenerateBGText_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GetBGText(BG));
            StatusText.Text = "BG text copied to clipboard";
        }

        private void RadioHistogram_CheckedChanged(object sender, EventArgs e)
        {
            if(RadioHistogram.Checked)
                SetChartMode(ChartType);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(RadioList.Checked)
                SetChartMode(ChartType);
        }

        private void BattlegroupBuilder_Resize(object sender, EventArgs e)
        {
            PanelBGShare.Width = this.Width - 14;
            ButtonGenerateCode.Location = new Point(PanelBGShare.Width - ButtonGenerateCode.Width - 3, ButtonGenerateCode.Location.Y);
            ButtonGenerateBGText.Location = new Point(ButtonGenerateCode.Location.X - ButtonGenerateBGText.Width - 16, ButtonGenerateCode.Location.Y);
            TextboxBGBCode.Width = this.Width - 377;

            RuneDescription.Location = new Point(PanelBGShare.Width - RuneDescription.Width - 5, RuneDescription.Location.Y);
            RuneDescription.SetHeight(this.Height - 127);

            PanelRuneList.Location = new Point((PanelBGShare.Width - PanelRuneList.Width - 293) / 2, PanelRuneList.Location.Y);
            PanelBaseStats.Location = new Point(PanelRuneList.Location.X, PanelBaseStats.Location.Y);

            PageContainer.Size = new Size(this.Width - 309, this.Height - 399);

            ButtonToggleFilter.Height = PageContainer.Height - 41 - 29;
            PanelRuneIcons.Size = new Size(PageContainer.Width - 8 - ButtonToggleFilter.Location.X - ButtonToggleFilter.Width - 6, ButtonToggleFilter.Height);
            PanelRunePageSettings.Width = PageContainer.Width - 8 - 15;
            PanelRunePageSettings.Location = new Point(6, PageContainer.Height - PanelRunePageSettings.Height - 4 - 29);
            PanelRunePageSelection.Location = new Point(PanelRunePageSettings.Width - PanelRunePageSelection.Width, 0);
            CalculateRunePreviewPageSize();
            DisplayRunePage(Program.database, 0);

            PanelHistogramElements.Width = PageContainer.Width - 8 - 9;
            PanelHistogramElements.Height = PageContainer.Height - PanelHistogramElements.Location.Y - 6 - 29; ;  // 29   // 8
            SetChartMode(ChartType);

            ListBoxBGErrorLog.Size = new Size(PageContainer.Width - 8 - 6, PageContainer.Height - 29 - 6);
            ListBoxBGErrorLog.Location = new Point(3, 3);
        }
    }
}
