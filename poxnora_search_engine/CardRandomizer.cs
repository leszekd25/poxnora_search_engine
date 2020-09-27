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

namespace poxnora_search_engine
{
    public partial class CardRandomizer : Form
    {
        public class CardRandomizerCardNumber
        {
            public uint Champions;
            public uint Spells;
            public uint Relics;
            public uint Equips;
        }

        public class CardRandomizerFlags
        {
            public bool IsHighlander;
            public bool AllowUnforgeableCards;
            public bool AllowBannedCards;
            public bool AllowSplitCards;
        }

        public class CardRandomizerCardPool
        {
            List<int> Champions = new List<int>();
            List<int> Spells = new List<int>();
            List<int> Relics = new List<int>();
            List<int> Equips = new List<int>();

            private bool VerifyCard(Pox.Rune rune, List<string> factions, CardRandomizerFlags flags)
            {
                // limited cards not allowed
                if (rune.Rarity == "LIMITED")
                    return false;

                // check if its a split card
                if ((rune.Faction.Count > 1) && (!flags.AllowSplitCards))
                    return false;

                // check if its banned card
                if ((!rune.AllowRanked) && (!flags.AllowBannedCards))
                    return false;

                // check if its forgeable
                if ((rune.Expansion == "Planar Disturbances") && (!flags.AllowUnforgeableCards))
                    return false;

                // check if factions match
                bool faction_match = false;
                foreach (string f in factions)
                    if (rune.Faction.Contains(f))
                    {
                        faction_match = true;
                        break;
                    }

                if (!faction_match)
                    return false;

                return true;
            }

            public void LoadCardsFromDatabase(List<string> factions, CardRandomizerFlags flags)
            {
                if (!Program.database.ready)
                    return;

                // load champions
                foreach (var kv in Program.database.Champions)
                {
                    if (!VerifyCard(kv.Value, factions, flags))
                        continue;

                    bool is_valid_hero = true;
                    // check if this rune is a hero or a split-hero
                    foreach(var ab in kv.Value.BaseAbilities_refs)
                    {
                        if((ab.Name == "Hero")&&(factions.Count > 1))
                        {
                            is_valid_hero = false;
                            break;
                        }
                        if((ab.Name == "Split Hero")&&(factions.Count == 1))
                        {
                            is_valid_hero = false;
                            break;
                        }
                    }
                    if (!is_valid_hero)
                        continue;

                    // add champion to the pool
                    if (flags.IsHighlander)
                        Champions.Add(kv.Key);
                    else
                        for (int i = 0; i < kv.Value.DeckLimit; i++)
                            Champions.Add(kv.Key);
                }

                // add spells
                foreach (var kv in Program.database.Spells)
                {
                    if (!VerifyCard(kv.Value, factions, flags))
                        continue;

                    // add spell to the pool
                    if (flags.IsHighlander)
                        Spells.Add(kv.Key);
                    else
                        for (int i = 0; i < kv.Value.DeckLimit; i++)
                            Spells.Add(kv.Key);
                }

                // add relics
                foreach (var kv in Program.database.Relics)
                {
                    if (!VerifyCard(kv.Value, factions, flags))
                        continue;

                    // add relic to the pool
                    if (flags.IsHighlander)
                        Relics.Add(kv.Key);
                    else
                        for (int i = 0; i < kv.Value.DeckLimit; i++)
                            Relics.Add(kv.Key);
                }

                // add equips
                foreach (var kv in Program.database.Equipments)
                {
                    if (!VerifyCard(kv.Value, factions, flags))
                        continue;

                    // add equip to the pool
                    if (flags.IsHighlander)
                        Equips.Add(kv.Key);
                    else
                        for (int i = 0; i < kv.Value.DeckLimit; i++)
                            Equips.Add(kv.Key);
                }
            }

            public int ExtractRandomChampion(string faction)
            {
                int result = -1;

                for (int i = 0; i < 500; i++)
                {
                    if (Champions.Count == 0)
                        return -1;

                    int c_index = Utility.RNG.Next(0, Champions.Count);
                    result = Champions[c_index];
                    if (!Program.database.Champions[result].Faction.Contains(faction))
                        continue;

                    Champions.RemoveAt(c_index);
                    break;
                }

                return result;
            }

            public int ExtractRandomSpell(string faction)
            {
                if (Spells.Count == 0)
                    return -1;

                int result = -1;

                for (int i = 0; i < 500; i++)
                {
                    int c_index = Utility.RNG.Next(0, Spells.Count);
                    result = Spells[c_index];
                    if (!Program.database.Spells[result].Faction.Contains(faction))
                        continue;

                    Spells.RemoveAt(c_index);
                    break;
                }

                return result;
            }

            public int ExtractRandomRelic(string faction)
            {
                if (Relics.Count == 0)
                    return -1;

                int result = -1;

                for (int i = 0; i < 500; i++)
                {
                    int c_index = Utility.RNG.Next(0, Relics.Count);
                    result = Relics[c_index];
                    if (!Program.database.Relics[result].Faction.Contains(faction))
                        continue;

                    Relics.RemoveAt(c_index);
                    break;
                }

                return result;
            }

            public int ExtractRandomEquipment(string faction)
            {
                if (Equips.Count == 0)
                    return -1;

                int result = -1;

                for (int i = 0; i < 500; i++)
                {
                    int c_index = Utility.RNG.Next(0, Equips.Count);
                    result = Equips[c_index];
                    if (!Program.database.Equipments[result].Faction.Contains(faction))
                        continue;

                    Equips.RemoveAt(c_index);
                    break;
                }

                return result;
            }

        }

        public class CardRandomizerFactionResultList
        {
            List<string> Champions = new List<string>();
            List<string> Spells = new List<string>();
            List<string> Relics = new List<string>();
            List<string> Equips = new List<string>();

            public bool ExtractCards(string faction, CardRandomizerCardPool pool, CardRandomizerCardNumber numbers)
            {
                for (int i = 0; i < numbers.Champions; i++)
                {
                    int c = pool.ExtractRandomChampion(faction);
                    if (c == -1)
                        return false;

                    Champions.Add(Program.database.Champions[c].Name);
                }
                Champions.Sort();

                for (int i = 0; i < numbers.Spells; i++)
                {
                    int c = pool.ExtractRandomSpell(faction);
                    if (c == -1)
                        return false;

                    Spells.Add(Program.database.Spells[c].Name);
                }
                Spells.Sort();

                for (int i = 0; i < numbers.Relics; i++)
                {
                    int c = pool.ExtractRandomRelic(faction);
                    if (c == -1)
                        return false;

                    Relics.Add(Program.database.Relics[c].Name);
                }
                Relics.Sort();

                for (int i = 0; i < numbers.Equips; i++)
                {
                    int c = pool.ExtractRandomEquipment(faction);
                    if (c == -1)
                        return false;

                    Equips.Add(Program.database.Equipments[c].Name);
                }
                Equips.Sort();

                return true;
            }

            public override string ToString()
            {
                string result = "";
                foreach (string c in Champions)
                    result += c + "\n";
                foreach (string s in Spells)
                    result += s + "\n";
                foreach (string r in Relics)
                    result += r + "\n";
                foreach (string e in Equips)
                    result += e + "\n";

                return result;
            }
        }

        public CardRandomizer()
        {
            InitializeComponent();
        }

        public void SetStatus(string s)
        {
            toolStripStatusLabel1.Text = s;
            statusStrip1.Refresh();
        }

        public bool ValidateSettings()
        {
            if(ComboMainFaction.SelectedIndex < 1)
            {
                SetStatus("Select main faction first");
                return false;
            }

            uint tmp_parse;

            if (!uint.TryParse(MFCChampions.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Main faction champion count");
                return false;
            }
            if (!uint.TryParse(MFCSpells.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Main faction spell count");
                return false;
            }
            if (!uint.TryParse(MFCRelics.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Main faction relic count");
                return false;
            }
            if (!uint.TryParse(MFCEquips.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Main faction equip count");
                return false;
            }
            if (!uint.TryParse(SFCChampions.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Secondary faction champion count");
                return false;
            }
            if (!uint.TryParse(SFCSpells.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Secondary faction spell count");
                return false;
            }
            if (!uint.TryParse(SFCRelics.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Secondary faction relic count");
                return false;
            }
            if (!uint.TryParse(SFCEquips.Text, out tmp_parse))
            {
                SetStatus("Invalid setting: Secondary faction equip count");
                return false;
            }

            return true;
        }

        private void SaveSettings()
        {
            try
            {
                FileStream fs = new FileStream("randomizer_settings", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(ComboMainFaction.SelectedItem.ToString());
                sw.WriteLine(MFCChampions.Text);
                sw.WriteLine(MFCSpells.Text);
                sw.WriteLine(MFCRelics.Text);
                sw.WriteLine(MFCEquips.Text);

                sw.WriteLine(ComboSecondaryFaction.SelectedItem.ToString());
                sw.WriteLine(SFCChampions.Text);
                sw.WriteLine(SFCSpells.Text);
                sw.WriteLine(SFCRelics.Text);
                sw.WriteLine(SFCEquips.Text);

                sw.WriteLine(CheckHighlander.Checked.ToString());
                sw.WriteLine(CheckAllowUnforgeableCards.Checked.ToString());
                sw.WriteLine(CheckboxAllowBannedCards.Checked.ToString());
                sw.WriteLine(CheckAllowSplitCards.Checked.ToString());

                sw.Close();
            }
            catch(Exception e)
            {
                Log.Error("CardRandomizer.SaveSettings() failed: " + e.ToString());
            }
        }

        private void LoadSettings()
        {
            if(!File.Exists("randomizer_settings"))
            {
                SetStatus("Settings file not found");
                return;
            }

            try
            {
                FileStream fs = new FileStream("randomizer_settings", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                string MainFaction = sr.ReadLine();
                foreach(var o in ComboMainFaction.Items)
                {
                    if(o.ToString() == MainFaction)
                    {
                        ComboMainFaction.SelectedItem = o;
                        break;
                    }
                }
                MFCChampions.Text = sr.ReadLine();
                MFCSpells.Text = sr.ReadLine();
                MFCRelics.Text = sr.ReadLine();
                MFCEquips.Text = sr.ReadLine();

                string SecondaryFaction = sr.ReadLine();
                foreach (var o in ComboSecondaryFaction.Items)
                {
                    if (o.ToString() == SecondaryFaction)
                    {
                        ComboSecondaryFaction.SelectedItem = o;
                        break;
                    }
                }
                SFCChampions.Text = sr.ReadLine();
                SFCSpells.Text = sr.ReadLine();
                SFCRelics.Text = sr.ReadLine();
                SFCEquips.Text = sr.ReadLine();

                CheckHighlander.Checked = (sr.ReadLine() == "True");
                CheckAllowUnforgeableCards.Checked = (sr.ReadLine() == "True");
                CheckboxAllowBannedCards.Checked = (sr.ReadLine() == "True");
                CheckAllowSplitCards.Checked = (sr.ReadLine() == "True");

                sr.Close();
            }
            catch (Exception e)
            {
                SetStatus("Could not load previous session settings");
                Log.Error("CardRandomizer.LoadSettings() failed: " + e.ToString());
            }
        }


        // events

        private void CardRandomizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void CardRandomizer_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
        
        private void CardRandomizer_Deactivate(object sender, EventArgs e)
        {
            SetStatus("Ready");
        }

        private void ComboMainFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            PanelMainFactionCards.Enabled = (ComboMainFaction.SelectedIndex >= 1);
        }

        private void ComboSecondaryFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            PanelSecondaryFactionCards.Enabled = (ComboSecondaryFaction.SelectedIndex >= 1);
        }

        private void ButtonGenerateCards_Click(object sender, EventArgs e)
        {
            if(!Program.database.ready)
            {
                SetStatus("Database is not ready");
                return;
            }

            // stage 1: generate settings
            if (!ValidateSettings())
                return;

            string MainFaction = ComboMainFaction.SelectedItem.ToString();
            string SecondaryFaction = ComboSecondaryFaction.SelectedItem.ToString();

            CardRandomizerCardNumber mfcn = new CardRandomizerCardNumber();
            CardRandomizerCardNumber sfcn = new CardRandomizerCardNumber();
            CardRandomizerFlags crf = new CardRandomizerFlags();

            uint.TryParse(MFCChampions.Text, out mfcn.Champions);
            uint.TryParse(MFCSpells.Text, out mfcn.Spells);
            uint.TryParse(MFCRelics.Text, out mfcn.Relics);
            uint.TryParse(MFCEquips.Text, out mfcn.Equips);
            uint.TryParse(SFCChampions.Text, out sfcn.Champions);
            uint.TryParse(SFCSpells.Text, out sfcn.Spells);
            uint.TryParse(SFCRelics.Text, out sfcn.Relics);
            uint.TryParse(SFCEquips.Text, out sfcn.Equips);

            crf.IsHighlander = CheckHighlander.Checked;
            crf.AllowUnforgeableCards = CheckAllowUnforgeableCards.Checked;
            crf.AllowBannedCards = CheckboxAllowBannedCards.Checked;
            crf.AllowSplitCards = CheckAllowSplitCards.Checked;

            // stage 2: add runes to the pool, according to the settings
            SetStatus("Loading cards to the pool...");

            List<string> factions = new List<string>();
            factions.Add(MainFaction);
            if (SecondaryFaction != "None")
                factions.Add(SecondaryFaction);

            CardRandomizerCardPool pool = new CardRandomizerCardPool();
            pool.LoadCardsFromDatabase(factions, crf);

            // stage 3: extract runes from the pool, according to the settings

            SetStatus("Generating result...");
            CardRandomizerFactionResultList mfresult = new CardRandomizerFactionResultList();
            CardRandomizerFactionResultList sfresult = new CardRandomizerFactionResultList();

            if (!mfresult.ExtractCards(MainFaction, pool, mfcn))
            {
                SetStatus("Failed to generate result for main faction");
                return;
            }

            if(SecondaryFaction != "None")
            {
                if (!sfresult.ExtractCards(SecondaryFaction, pool, sfcn))
                {
                    SetStatus("Failed to generate result for secondary faction");
                    return;
                }
            }

            Clipboard.SetText(mfresult.ToString() + (SecondaryFaction == "None" ? "" : "\n" + sfresult.ToString()));
            SetStatus("Copied to clipboard");
        }
    }
}
