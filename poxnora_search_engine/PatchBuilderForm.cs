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
    public partial class PatchBuilderForm : Form
    {
        Pox.Diff.DatabaseDifferenceCalculator diff_calculator = new Pox.Diff.DatabaseDifferenceCalculator();
        Pox.Diff.DifferencePresentation diff_presentation;
        Pox.Diff.DifferenceLink diff_link = null;

        class ChampionPatchData
        {
            public ushort ID;
            public byte Damage;
            public byte Defense;
            public byte MinRange;
            public byte MaxRange;
            public byte Speed;
            public byte Health;
            public sbyte NoraMod;
            public List<ushort> BaseAbilities = new List<ushort>();
            public List<ushort> U1Abilities = new List<ushort>();
            public List<ushort> U2Abilities = new List<ushort>();
            public ushort U1Default;
            public ushort U2Default;
        }

        const ushort PATCH_VERSION = 0;

        Dictionary<ushort, ChampionPatchData> PatchChampions = new Dictionary<ushort, ChampionPatchData>();

        enum AbilityListMode { BASE = 0, U1, U2 };
        AbilityListMode CurrentAbilityListMode = AbilityListMode.BASE;
        string PatchName = "";

        public PatchBuilderForm()
        {
            InitializeComponent();

            diff_calculator.LoadDatabases("database.json");   // loads both databases from the same source; current database of this will be getting modified
            if (!diff_calculator.ready)
            {
                MessageBox.Show("Could not load databases...");
                this.Close();
                return;
            }

            LoadChampionNames();
            LoadAbilityNames();

            diff_presentation = new Pox.Diff.DifferencePresentation()
            {
                PanelRef = PanelChangeList,
                ButtonPreviousRef = ButtonPrevious,
                ButtonCurrentRef = ButtonCurrent,
                DiffCalculatorRef = diff_calculator
            };

            RuneDescription.database_ref = diff_calculator.CurrentDatabase_ref;
        }

        void SetStatusText(string text, bool error = false)
        {
            if(error)
            {
                StatusText.ForeColor = Color.DarkRed;
            }
            else
            {
                StatusText.ForeColor = Color.Black;
            }
            StatusText.Text = text;
        }

        private void LoadChampionNames()
        {
            HashSet<string> ChampionNameList = new HashSet<string>();
            foreach (var c in diff_calculator.PreviousDatabase.Champions)
                ChampionNameList.Add(c.Value.Name);

            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(ChampionNameList.ToArray());
            TextBoxEntry.AutoCompleteCustomSource = acsc;
            TextBoxEntry.AutoCompleteMode = AutoCompleteMode.Suggest;
            TextBoxEntry.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void LoadAbilityNames()
        {
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(diff_calculator.PreviousDatabase.AbilityNames.AllowedStrings.ToArray());
            TextAbility.AutoCompleteCustomSource = acsc;
            TextAbility.AutoCompleteMode = AutoCompleteMode.Suggest;
            TextAbility.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        int GetNewEntrySortedIndex(string name)
        {
            for(int i = 0; i < ListBoxEntries.Items.Count; i++)
            {
                int cmpresult = ListBoxEntries.Items[i].ToString().CompareTo(name);
                if(cmpresult == 0)   // same name, not allowed
                {
                    return Utility.NO_INDEX;
                }
                if (cmpresult > 0)
                {
                    return i;
                }
            }
            return ListBoxEntries.Items.Count;
        }

        ChampionPatchData GetChampionPatchDataFromName(string name)
        {
            int champ_id = Utility.NO_INDEX;
            foreach (var kv in PatchChampions)
            {
                if (diff_calculator.CurrentDatabase_ref.Champions[kv.Key].Name == name)
                {
                    champ_id = kv.Key;
                    break;
                }
            }
            if (champ_id == Utility.NO_INDEX)
            {
                return null;
            }
            if(PatchChampions.ContainsKey((ushort)champ_id))
            {
                return PatchChampions[(ushort)champ_id];
            }
            return null;
        }

        private void ButtonEntryAdd_Click(object sender, EventArgs e)
        {
            int champ_id = Utility.NO_INDEX;
            foreach (var ch in diff_calculator.PreviousDatabase.Champions)
            {
                if (ch.Value.Name == TextBoxEntry.Text)
                {
                    champ_id = ch.Key;
                    break;
                }
            }
            if(champ_id == Utility.NO_INDEX)
            {
                SetStatusText($"Invalid champion name \"{TextBoxEntry.Text}\"", true);
                return;
            }

            // find where the champion should be added to list
            int new_index = GetNewEntrySortedIndex(TextBoxEntry.Text);
            if(new_index == Utility.NO_INDEX)
            {
                SetStatusText($"Could not put champion \"{TextBoxEntry.Text}\" on the list", true);
                return;
            }

            ChampionPatchData cpd = new ChampionPatchData();
            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[champ_id];
            cpd.ID = (ushort)champ_id;
            cpd.Damage = (byte)c.Damage;
            cpd.Speed = (byte)c.Speed;
            cpd.MinRange = (byte)c.MinRNG;
            cpd.MaxRange = (byte)c.MaxRNG;
            cpd.Defense = (byte)c.Defense;
            cpd.Health = (byte)c.HitPoints;
            cpd.NoraMod = (sbyte)c.PrognosedBaseNoraCostDifference;
            foreach (var ab in c.Abilities)
            {
                cpd.BaseAbilities.Add((ushort)ab);
            }
            foreach (var ab in c.Upgrade1)
            {
                cpd.U1Abilities.Add((ushort)ab);
            }
            foreach (var ab in c.Upgrade2)
            {
                cpd.U2Abilities.Add((ushort)ab);
            }
            cpd.U1Default = (ushort)c.Upgrade1[c.DefaultUpgrade1Index];
            cpd.U2Default = (ushort)c.Upgrade2[c.DefaultUpgrade2Index];

            PatchChampions.Add((ushort)champ_id, cpd);
            ListBoxEntries.Items.Insert(new_index, TextBoxEntry.Text);

            ListBoxEntries.SelectedIndex = new_index;
            SetStatusText($"Added champion \"{TextBoxEntry.Text}\"");
        }

        private void ButtonEntryRemove_Click(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                SetStatusText($"No champion selected for removal", true);
                return;
            }

            string champ_name = ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString();
            int champ_id = Utility.NO_INDEX;
            foreach(var kv in PatchChampions)
            {
                if(diff_calculator.PreviousDatabase.Champions[kv.Key].Name == champ_name)
                {
                    champ_id = kv.Key;
                    break;
                }
            }
            if (champ_id == Utility.NO_INDEX)
            {
                SetStatusText($"Champion \"{champ_name}\" not on the list", true);
                return;
            }

            ChampionRevert((ushort)champ_id);
            PatchChampions.Remove((ushort)champ_id);
            ListBoxEntries.Items.RemoveAt(ListBoxEntries.SelectedIndex);
            SetStatusText($"Removed champion \"{champ_name}\"");
        }

        void RefreshNoraCostLabels(ushort id)
        {
            LabelStatCost.Text = "Stat cost: " + diff_calculator.CurrentDatabase_ref.Champions[id].PrognosedBaseNoraCost;
            LabelBaseCost.Text = "Base cost: " + diff_calculator.CurrentDatabase_ref.Champions[id].BaseNoraCost;
            LabelDefaultCost.Text = "Default cost: " + diff_calculator.CurrentDatabase_ref.Champions[id].DefaultNoraCost;
            LabelNoraRange.Text = "Nora cost range: " + diff_calculator.CurrentDatabase_ref.Champions[id].MinNoraCost + "-" + diff_calculator.CurrentDatabase_ref.Champions[id].MaxNoraCost;
        }

        void RefreshChampionChangeInfo()
        {
            diff_presentation.ClearChangeInfo();
            diff_presentation.LoadChangeInfo(diff_link);

            RuneDescription.SetChampionRune(RuneDescription.database_ref.Champions[diff_link.CurrentElement.ID],
                        RuneDescription.database_ref.Champions[diff_link.CurrentElement.ID].DefaultUpgrade1Index,
                        RuneDescription.database_ref.Champions[diff_link.CurrentElement.ID].DefaultUpgrade2Index);
        }

        private void ListBoxEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            diff_presentation.ClearChangeInfo();

            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                PanelEntryData.Enabled = false;
                return;
            }

            string champ_name = ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString();

            ChampionPatchData cpd = GetChampionPatchDataFromName(champ_name);
            if (cpd == null)
            {
                SetStatusText($"Selected champion is not on the patch list!", true);
                PanelEntryData.Enabled = false;
                return;
            }

            diff_link = new Pox.Diff.DifferenceLink()
            {
                ElemType = Pox.DataElement.ElementType.CHAMPION,
                PreviousElement = diff_calculator.PreviousDatabase.Champions[cpd.ID],
                CurrentElement = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID]
            };

            PanelEntryData.Enabled = true;

            LabelChampionID.Text = "ID: " + cpd.ID;
            LabelChampionName.Text = "Name: " + diff_calculator.CurrentDatabase_ref.Champions[cpd.ID].Name;

            TextDamage.Text = cpd.Damage.ToString();
            TextSpeed.Text = cpd.Speed.ToString();
            TextMinRange.Text = cpd.MinRange.ToString();
            TextMaxRange.Text = cpd.MaxRange.ToString();
            TextDefense.Text = cpd.Defense.ToString();
            TextHitPoints.Text = cpd.Health.ToString();
            TextNoraMod.Text = cpd.NoraMod.ToString();

            RefreshAbilityList();

            ComboU1.Items.Clear();
            foreach (var ab in cpd.U1Abilities)
            {
                Pox.Ability ab_ref = diff_calculator.CurrentDatabase_ref.Abilities[ab];
                ComboU1.Items.Add(ab_ref.ToString() + " (" + ab_ref.NoraCost + " nora)");
                if(ab == cpd.U1Default)
                {
                    ComboU1.SelectedIndex = ComboU1.Items.Count - 1;
                }
            }

            ComboU2.Items.Clear();
            foreach (var ab in cpd.U2Abilities)
            {
                Pox.Ability ab_ref = diff_calculator.CurrentDatabase_ref.Abilities[ab];
                ComboU2.Items.Add(ab_ref.ToString() + " (" + ab_ref.NoraCost + " nora)");
                if (ab == cpd.U2Default)
                {
                    ComboU2.SelectedIndex = ComboU2.Items.Count - 1;
                }
            }

            RefreshNoraCostLabels(cpd.ID);
            RefreshChampionChangeInfo();
            SetStatusText($"Ready");
        }

        void ChampionReset(Pox.Champion previous, Pox.Champion current)
        {
            current.Damage = previous.Damage;
            current.Speed = previous.Speed;
            current.MinRNG = previous.MinRNG;
            current.MaxRNG = previous.MaxRNG;
            current.Defense = previous.Defense;
            current.HitPoints = previous.HitPoints;

            current.BaseNoraCost = previous.BaseNoraCost;
            current.NoraCost = previous.NoraCost;
            current.DefaultNoraCost = previous.DefaultNoraCost;
            current.MinNoraCost = previous.MinNoraCost;
            current.MaxNoraCost = previous.MaxNoraCost;
            current.PrognosedBaseNoraCost = previous.PrognosedBaseNoraCost;
            current.PrognosedBaseNoraCostDifference = previous.PrognosedBaseNoraCostDifference;

            foreach (var ab in current.Abilities)
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if(diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            current.Abilities.Clear();
            current.AllAbilities_refs.Clear();
            current.AllUpgradeAbilities_refs.Clear();
            current.BaseAbilities_refs.Clear();
            foreach (var ab in current.Upgrade1)
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            current.Upgrade1.Clear();
            foreach (var ab in current.Upgrade2)
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            current.Upgrade2.Clear();
            current.UpgradeAbilities1_refs.Clear();
            current.UpgradeAbilities2_refs.Clear();

            foreach (var ab in previous.Abilities)
            {
                current.Abilities.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;
            }
            foreach (var ab in previous.Upgrade1)
            {
                current.Upgrade1.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;
            }
            foreach (var ab in previous.Upgrade2)
            {
                current.Upgrade2.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;
            }
            diff_calculator.CurrentDatabase_ref.SetupChampionAbilities(current);
        }

        public void ChampionRevert(ushort id)
        {
            if(!diff_calculator.PreviousDatabase.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not revert champion ID {id}", true);
                return;
            }
            ChampionReset(diff_calculator.PreviousDatabase.Champions[id], diff_calculator.CurrentDatabase_ref.Champions[id]);
        }

        public void PatchRevert()
        {
            foreach(var kv in PatchChampions)
            {
                ChampionRevert(kv.Key);
            }
            PatchChampions.Clear();
        }

        void ChampionRecalculateNoraCost(ushort id)
        {
            if (!diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not recalculate nora costs for champion ID {id}", true);
                return;
            }

            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[id];

            // known: stat nora cost, nora mod, abilities and their costs
            // unknown: default, min, max, base nora cost
            c.BaseNoraCost = c.PrognosedBaseNoraCost + c.PrognosedBaseNoraCostDifference;
            c.NoraCost = c.BaseNoraCost;
            foreach(var ab in c.BaseAbilities_refs)
            {
                c.NoraCost += ab.NoraCost;
            }
            c.DefaultNoraCost = c.NoraCost + c.UpgradeAbilities1_refs[c.DefaultUpgrade1Index].NoraCost + c.UpgradeAbilities2_refs[c.DefaultUpgrade2Index].NoraCost;
            // find min and max nora cost among upgrade 1 and 2
            int minupg1 = c.UpgradeAbilities1_refs[0].NoraCost;
            int minupg2 = c.UpgradeAbilities2_refs[0].NoraCost;
            int maxupg1 = c.UpgradeAbilities1_refs[0].NoraCost;
            int maxupg2 = c.UpgradeAbilities2_refs[0].NoraCost;
            for (int i = 1; i < c.Upgrade1.Count; i++)
            {
                if (c.UpgradeAbilities1_refs[i].NoraCost < minupg1)
                    minupg1 = c.UpgradeAbilities1_refs[i].NoraCost;
                if (c.UpgradeAbilities1_refs[i].NoraCost > maxupg1)
                    maxupg1 = c.UpgradeAbilities1_refs[i].NoraCost;
            }
            for (int i = 1; i < c.Upgrade2.Count; i++)
            {
                if (c.UpgradeAbilities2_refs[i].NoraCost < minupg2)
                    minupg2 = c.UpgradeAbilities2_refs[i].NoraCost;
                if (c.UpgradeAbilities2_refs[i].NoraCost > maxupg2)
                    maxupg2 = c.UpgradeAbilities2_refs[i].NoraCost;
            }
            c.MinNoraCost = c.NoraCost + minupg1 + minupg2;
            c.MaxNoraCost = c.NoraCost + maxupg1 + maxupg2;

            RefreshNoraCostLabels((ushort)c.ID);
        }

        public void ChampionApplyStats(ushort id)
        {
            if(!diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not apply stats for champion ID {id}", true);
                return;
            }

            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[id];
            ChampionPatchData cpd = PatchChampions[id];

            c.Damage = cpd.Damage;
            c.Speed = cpd.Speed;
            c.MinRNG = cpd.MinRange;
            c.MaxRNG = cpd.MaxRange;
            c.Defense = cpd.Defense;
            c.HitPoints = cpd.Health;
            c.CalculatePrognosedBaseNoraCostNew();
            c.PrognosedBaseNoraCostDifference = cpd.NoraMod;
        }

        public void ChampionApplyAbilitiesBase(ushort id)
        {
            if (!diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not apply base abilities for champion ID {id}", true);
                return;
            }

            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[id];
            ChampionPatchData cpd = PatchChampions[id];

            foreach(var ab in c.Abilities)
            {
                for (int i = 0; i < c.AllAbilities_refs.Count; i++)
                {
                    if (c.AllAbilities_refs[i].ID == ab)
                    {
                        c.AllAbilities_refs.RemoveAt(i);
                        break;
                    }
                }

                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            c.Abilities.Clear();
            c.BaseAbilities_refs.Clear();

            foreach(var ab in cpd.BaseAbilities)
            {
                c.Abilities.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                }
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;

                c.AllAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
                c.BaseAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
            }
        }

        public void ChampionApplyAbilitiesUpgrade1(ushort id)
        {
            if (!diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not apply upgrade abilities for champion ID {id}", true);
                return;
            }

            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[id];
            ChampionPatchData cpd = PatchChampions[id];

            foreach (var ab in c.Upgrade1)
            {
                for (int i = 0; i < c.AllAbilities_refs.Count; i++)
                {
                    if (c.AllAbilities_refs[i].ID == ab)
                    {
                        c.AllAbilities_refs.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < c.AllUpgradeAbilities_refs.Count; i++)
                {
                    if (c.AllUpgradeAbilities_refs[i].ID == ab)
                    {
                        c.AllUpgradeAbilities_refs.RemoveAt(i);
                        break;
                    }
                }

                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            c.Upgrade1.Clear();
            c.UpgradeAbilities1_refs.Clear();

            foreach (var ab in cpd.U1Abilities)
            {
                c.Upgrade1.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                }
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;

                c.AllAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
                c.AllUpgradeAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
                c.UpgradeAbilities1_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
            }

            c.DefaultUpgrade1Index = c.Upgrade1.IndexOf(cpd.U1Default);
            if(c.DefaultUpgrade1Index == Utility.NO_INDEX)
            {
                c.DefaultUpgrade1Index = 0;
            }
        }

        public void ChampionApplyAbilitiesUpgrade2(ushort id)
        {
            if (!diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(id))
            {
                SetStatusText($"Could not apply upgrade abilities for champion ID {id}", true);
                return;
            }

            Pox.Champion c = diff_calculator.CurrentDatabase_ref.Champions[id];
            ChampionPatchData cpd = PatchChampions[id];

            foreach (var ab in c.Upgrade2)
            {
                for (int i = 0; i < c.AllAbilities_refs.Count; i++)
                {
                    if (c.AllAbilities_refs[i].ID == ab)
                    {
                        c.AllAbilities_refs.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < c.AllUpgradeAbilities_refs.Count; i++)
                {
                    if (c.AllUpgradeAbilities_refs[i].ID == ab)
                    {
                        c.AllUpgradeAbilities_refs.RemoveAt(i);
                        break;
                    }
                }

                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] -= 1;
                if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] == 0)
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(ab);
                }
            }
            c.Upgrade2.Clear();
            c.UpgradeAbilities2_refs.Clear();

            foreach (var ab in cpd.U2Abilities)
            {
                c.Upgrade2.Add(ab);

                if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab))
                {
                    diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab, 0);
                }
                diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab] += 1;

                c.AllAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
                c.AllUpgradeAbilities_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
                c.UpgradeAbilities2_refs.Add(diff_calculator.CurrentDatabase_ref.Abilities[ab]);
            }

            c.DefaultUpgrade2Index = c.Upgrade2.IndexOf(cpd.U2Default);
            if (c.DefaultUpgrade2Index == Utility.NO_INDEX)
            {
                c.DefaultUpgrade2Index = 0;
            }
        }

        public void PatchApply()
        {
            foreach (var kv in PatchChampions)
            {
                ChampionApplyStats(kv.Key);
                ChampionApplyAbilitiesBase(kv.Key);
                ChampionApplyAbilitiesUpgrade1(kv.Key);
                ChampionApplyAbilitiesUpgrade2(kv.Key);

                ChampionRecalculateNoraCost(kv.Key);
            }
        }

        public bool PatchLoad(string fname)
        {
            if(!File.Exists(fname))
            {
                SetStatusText($"Could not load patch {fname}: file not found", true);
                return false;
            }

            Dictionary<ushort, ChampionPatchData> loaded_patch = new Dictionary<ushort, ChampionPatchData>();

            try
            {
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if(br.ReadUInt16() != PATCH_VERSION)
                        {
                            SetStatusText($"Could not load patch {fname}: invalid file format", true);
                            return false;
                        }

                        ushort champ_num = br.ReadUInt16();
                        for(int i = 0; i < champ_num; i++)
                        {
                            ChampionPatchData cpd = new ChampionPatchData();
                            cpd.ID = br.ReadUInt16();
                            cpd.Damage = br.ReadByte();
                            cpd.Defense = br.ReadByte();
                            cpd.MinRange = br.ReadByte();
                            cpd.MaxRange = br.ReadByte();
                            cpd.Speed = br.ReadByte();
                            cpd.Health = br.ReadByte();
                            cpd.NoraMod = br.ReadSByte();

                            byte base_ab_num = br.ReadByte();
                            for (int j = 0; j < base_ab_num; j++)
                            {
                                cpd.BaseAbilities.Add(br.ReadUInt16());
                            }
                            byte u1_ab_num = br.ReadByte();
                            if(u1_ab_num == 0)
                            {
                                return false;
                            }
                            for (int j = 0; j < u1_ab_num; j++)
                            {
                                cpd.U1Abilities.Add(br.ReadUInt16());
                            }
                            byte u2_ab_num = br.ReadByte();
                            if(u2_ab_num == 0)
                            {
                                return false;
                            }
                            for (int j = 0; j < u2_ab_num; j++)
                            {
                                cpd.U2Abilities.Add(br.ReadUInt16());
                            }
                            cpd.U1Default = br.ReadUInt16();
                            if(cpd.U1Abilities.IndexOf(cpd.U1Default) == Utility.NO_INDEX)
                            {
                                cpd.U1Default = cpd.U1Abilities[0];
                            }
                            cpd.U2Default = br.ReadUInt16();
                            if (cpd.U2Abilities.IndexOf(cpd.U2Default) == Utility.NO_INDEX)
                            {
                                cpd.U2Default = cpd.U2Abilities[0];
                            }

                            loaded_patch.Add(cpd.ID, cpd);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                SetStatusText($"Could not load patch {fname}: unexpected error", true);
                return false;
            }

            PatchRevert();

            ListBoxEntries.Items.Clear();
            PatchChampions = loaded_patch;
            PanelEntryData.Enabled = false;
            foreach(var kv in PatchChampions)
            {
                ListBoxEntries.Items.Add(diff_calculator.PreviousDatabase.Champions[kv.Key].Name);
            }

            PatchApply();

            PatchName = Path.GetFileNameWithoutExtension(fname);
            Text = "Patch Builder - " + PatchName;
            SetStatusText($"Loaded patch {PatchName}");

            return true;
        }

        public bool PatchSave(string fname)
        {
            try
            {
                using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(PATCH_VERSION);
                        bw.Write((ushort)PatchChampions.Count);
                        foreach(var kv in PatchChampions)
                        {
                            ChampionPatchData cpd = kv.Value;
                            bw.Write(cpd.ID);
                            bw.Write(cpd.Damage);
                            bw.Write(cpd.Defense);
                            bw.Write(cpd.MinRange);
                            bw.Write(cpd.MaxRange);
                            bw.Write(cpd.Speed);
                            bw.Write(cpd.Health);
                            bw.Write(cpd.NoraMod);

                            bw.Write((byte)cpd.BaseAbilities.Count);
                            for (int j = 0; j < cpd.BaseAbilities.Count; j++)
                            {
                                bw.Write(cpd.BaseAbilities[j]);
                            }
                            bw.Write((byte)cpd.U1Abilities.Count);
                            for (int j = 0; j < cpd.U1Abilities.Count; j++)
                            {
                                bw.Write(cpd.U1Abilities[j]);
                            }
                            bw.Write((byte)cpd.U2Abilities.Count);
                            for (int j = 0; j < cpd.U2Abilities.Count; j++)
                            {
                                bw.Write(cpd.U2Abilities[j]);
                            }
                            bw.Write(cpd.U1Default);
                            bw.Write(cpd.U2Default);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SetStatusText($"Could not save patch {fname}: file not found", true);
                return false;
            }

            return true;
        }

        void SetAbilityMode(AbilityListMode mode)
        {
            if(mode == CurrentAbilityListMode)
            {
                return;
            }

            CurrentAbilityListMode = mode;
            RefreshAbilityList();
        }

        void RefreshAbilityList()
        {
            ListAbilities.Items.Clear();

            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            switch (CurrentAbilityListMode)
            {
                case AbilityListMode.BASE:
                    {
                        foreach (var ab in cpd.BaseAbilities)
                        {
                            Pox.Ability ab_ref = diff_calculator.CurrentDatabase_ref.Abilities[ab];
                            ListAbilities.Items.Add(ab_ref.ToString() + " (" + ab_ref.NoraCost + " nora)");
                        }
                    }
                    break;
                case AbilityListMode.U1:
                    {
                        foreach (var ab in cpd.U1Abilities)
                        {
                            Pox.Ability ab_ref = diff_calculator.CurrentDatabase_ref.Abilities[ab];
                            ListAbilities.Items.Add(ab_ref.ToString() + " (" + ab_ref.NoraCost + " nora)");
                        }
                    }
                    break;
                case AbilityListMode.U2:
                    {
                        foreach (var ab in cpd.U2Abilities)
                        {
                            Pox.Ability ab_ref = diff_calculator.CurrentDatabase_ref.Abilities[ab];
                            ListAbilities.Items.Add(ab_ref.ToString() + " (" + ab_ref.NoraCost + " nora)");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void TextDamage_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if(cpd == null)
            {
                return;
            }

            if(byte.TryParse(TextDamage.Text, out byte dmg))
            {
                cpd.Damage = dmg;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextDamage.Text = cpd.Damage.ToString();
            }
        }

        private void TextSpeed_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if (byte.TryParse(TextSpeed.Text, out byte spd))
            {
                cpd.Speed = spd;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextSpeed.Text = cpd.Speed.ToString();
            }
        }

        private void TextMinRange_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if ((byte.TryParse(TextMinRange.Text, out byte minrng))&&(minrng <= cpd.MaxRange))
            {
                cpd.MinRange = minrng;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextMinRange.Text = cpd.MinRange.ToString();
            }
        }

        private void TextMaxRange_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if ((byte.TryParse(TextMaxRange.Text, out byte maxrng))&& (maxrng >= cpd.MinRange))
            {
                cpd.MaxRange = maxrng;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextMaxRange.Text = cpd.MaxRange.ToString();
            }
        }

        private void TextDefense_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if (byte.TryParse(TextDefense.Text, out byte def))
            {
                cpd.Defense = def;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextDefense.Text = cpd.Defense.ToString();
            }
        }

        private void TextHitPoints_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if (byte.TryParse(TextHitPoints.Text, out byte hp))
            {
                cpd.Health = hp;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextHitPoints.Text = cpd.Health.ToString();
            }
        }

        private void TextNoraMod_Validated(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            if (sbyte.TryParse(TextNoraMod.Text, out sbyte mod))
            {
                cpd.NoraMod = mod;
                ChampionApplyStats(cpd.ID);
                ChampionRecalculateNoraCost(cpd.ID);
                RefreshChampionChangeInfo();
            }
            else
            {
                TextNoraMod.Text = cpd.NoraMod.ToString();
            }
        }

        private void ButtonBaseAbilities_Click(object sender, EventArgs e)
        {
            SetAbilityMode(AbilityListMode.BASE);
            ButtonBaseAbilities.BackColor = Color.Gold;
            ButtonU1Abilities.BackColor = SystemColors.Control;
            ButtonU2Abilities.BackColor = SystemColors.Control;
        }

        private void ButtonU1Abilities_Click(object sender, EventArgs e)
        {
            SetAbilityMode(AbilityListMode.U1);
            ButtonBaseAbilities.BackColor = SystemColors.Control;
            ButtonU1Abilities.BackColor = Color.Gold;
            ButtonU2Abilities.BackColor = SystemColors.Control;
        }

        private void ButtonU2Abilities_Click(object sender, EventArgs e)
        {
            SetAbilityMode(AbilityListMode.U2);
            ButtonBaseAbilities.BackColor = SystemColors.Control;
            ButtonU1Abilities.BackColor = SystemColors.Control;
            ButtonU2Abilities.BackColor = Color.Gold;
        }

        private void AddAbility_Click(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            // find ability
            int ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(TextAbility.Text);
            if(ab_id == 0)
            {
                SetStatusText($"Could not find ability {TextAbility.Text}", true);
                return;
            }
            Pox.Ability ab = diff_calculator.CurrentDatabase_ref.Abilities[ab_id];

            // find if ability of same kind wasnt already added to the unit
            Pox.Champion ch = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID];
            foreach(var ch_ab in ch.AllAbilities_refs)
            {
                if(ch_ab.ClassName == ab.ClassName)
                {
                    SetStatusText($"Ability of type {ab.ClassName} already exists on current champion", true);
                    return;
                }
            }

            if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab.ID))
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab.ID, 0);
            }
            diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab.ID] += 1;

            ListAbilities.Items.Add(ab.ToString() + " (" + ab.NoraCost + " nora)");
            ch.AllAbilities_refs.Add(ab);

            switch (CurrentAbilityListMode)
            {
                case AbilityListMode.BASE:
                    {
                        cpd.BaseAbilities.Add((ushort)ab.ID);

                        ch.Abilities.Add(ab.ID);

                        ch.BaseAbilities_refs.Add(ab);
                    }
                    break;
                case AbilityListMode.U1:
                    {
                        cpd.U1Abilities.Add((ushort)ab.ID);

                        ch.Upgrade1.Add(ab.ID);

                        ch.UpgradeAbilities1_refs.Add(ab);
                        ch.AllUpgradeAbilities_refs.Add(ab);

                        ComboU1.Items.Add(ab.ToString() + " (" + ab.NoraCost + " nora)");
                    }
                    break;
                case AbilityListMode.U2:
                    {
                        cpd.U2Abilities.Add((ushort)ab.ID);

                        ch.Upgrade2.Add(ab.ID);

                        ch.UpgradeAbilities2_refs.Add(ab);
                        ch.AllUpgradeAbilities_refs.Add(ab);

                        ComboU2.Items.Add(ab.ToString() + " (" + ab.NoraCost + " nora)");
                    }
                    break;
                default:
                    break;
            }

            ChampionRecalculateNoraCost(cpd.ID);
            RefreshChampionChangeInfo();
        }

        private void SetAbility_Click(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            // find ability
            int ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(TextAbility.Text);
            if (ab_id == 0)
            {
                SetStatusText($"Could not find ability {TextAbility.Text}", true);
                return;
            }
            Pox.Ability ab = diff_calculator.CurrentDatabase_ref.Abilities[ab_id];

            // find if ability of same kind wasnt already added to the unit
            Pox.Champion ch = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID];
            foreach (var ch_ab in ch.AllAbilities_refs)
            {
                if (ch_ab.ClassName == ab.ClassName)
                {
                    SetStatusText($"Ability of type {ab.ClassName} already exists on current champion", true);
                    return;
                }
            }

            if(ListAbilities.SelectedIndex == Utility.NO_INDEX)
            {
                SetStatusText($"No ability selected for replacement", true);
                return;
            }

            string ab_fullname = ListAbilities.Items[ListAbilities.SelectedIndex].ToString();
            string ab_name = ab_fullname.Substring(0, ab_fullname.IndexOf('(')).Trim();
            int prev_ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(ab_name);
            if (prev_ab_id == 0)
            {
                SetStatusText($"Could not find ability {ab_name}", true);
                return;
            }

            // replace current selected ability

            diff_calculator.CurrentDatabase_ref.AbilityUseCount[prev_ab_id] -= 1;
            if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[prev_ab_id] == 0)
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(prev_ab_id);
            }
            if (!diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(ab.ID))
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount.Add(ab.ID, 0);
            }
            diff_calculator.CurrentDatabase_ref.AbilityUseCount[ab.ID] += 1;

            ListAbilities.Items[ListAbilities.SelectedIndex] = ab.ToString() + " (" + ab.NoraCost + " nora)";
            for(int i = 0; i < ch.AllAbilities_refs.Count; i++)
            {
                if(ch.AllAbilities_refs[i].ID == prev_ab_id)
                {
                    ch.AllAbilities_refs[i] = ab;
                    break;
                }
            }

            switch (CurrentAbilityListMode)
            {
                case AbilityListMode.BASE:
                    {
                        for(int i = 0; i < cpd.BaseAbilities.Count; i++)
                        {
                            if(cpd.BaseAbilities[i] == (ushort)prev_ab_id)
                            {
                                cpd.BaseAbilities[i] = (ushort)ab.ID;
                                break;
                            }
                        }

                        for (int i = 0; i < ch.Abilities.Count; i++)
                        {
                            if (ch.Abilities[i] == (ushort)prev_ab_id)
                            {
                                ch.Abilities[i] = (ushort)ab.ID;
                                break;
                            }
                        }

                        for (int i = 0; i < ch.BaseAbilities_refs.Count; i++)
                        {
                            if (ch.BaseAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.BaseAbilities_refs[i] = ab;
                                break;
                            }
                        }
                    }
                    break;
                case AbilityListMode.U1:
                    {
                        for (int i = 0; i < cpd.U1Abilities.Count; i++)
                        {
                            if (cpd.U1Abilities[i] == (ushort)prev_ab_id)
                            {
                                cpd.U1Abilities[i] = (ushort)ab.ID;

                                ComboU1.Items[i] = ab.ToString() + " (" + ab.NoraCost + " nora)";
                                break;
                            }
                        }

                        for (int i = 0; i < ch.Upgrade1.Count; i++)
                        {
                            if (ch.Upgrade1[i] == (ushort)prev_ab_id)
                            {
                                ch.Upgrade1[i] = (ushort)ab.ID;
                                break;
                            }
                        }

                        for (int i = 0; i < ch.UpgradeAbilities1_refs.Count; i++)
                        {
                            if (ch.UpgradeAbilities1_refs[i].ID == prev_ab_id)
                            {
                                ch.UpgradeAbilities1_refs[i] = ab;
                                break;
                            }
                        }
                        for (int i = 0; i < ch.AllUpgradeAbilities_refs.Count; i++)
                        {
                            if (ch.AllUpgradeAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.AllUpgradeAbilities_refs[i] = ab;
                                break;
                            }
                        }
                    }
                    break;
                case AbilityListMode.U2:
                    {
                        for (int i = 0; i < cpd.U2Abilities.Count; i++)
                        {
                            if (cpd.U2Abilities[i] == (ushort)prev_ab_id)
                            {
                                cpd.U2Abilities[i] = (ushort)ab.ID;

                                ComboU2.Items[i] = ab.ToString() + " (" + ab.NoraCost + " nora)";
                                break;
                            }
                        }

                        for (int i = 0; i < ch.Upgrade2.Count; i++)
                        {
                            if (ch.Upgrade2[i] == (ushort)prev_ab_id)
                            {
                                ch.Upgrade2[i] = (ushort)ab.ID;
                                break;
                            }
                        }

                        for (int i = 0; i < ch.UpgradeAbilities2_refs.Count; i++)
                        {
                            if (ch.UpgradeAbilities2_refs[i].ID == prev_ab_id)
                            {
                                ch.UpgradeAbilities2_refs[i] = ab;
                                break;
                            }
                        }
                        for (int i = 0; i < ch.AllUpgradeAbilities_refs.Count; i++)
                        {
                            if (ch.AllUpgradeAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.AllUpgradeAbilities_refs[i] = ab;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            ChampionRecalculateNoraCost(cpd.ID);
            RefreshChampionChangeInfo();
        }

        private void RemoveAbility_Click(object sender, EventArgs e)
        {
            if (ListBoxEntries.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            Pox.Champion ch = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID];

            if(ListAbilities.SelectedIndex == Utility.NO_INDEX)
            {
                SetStatusText($"No ability selected for removal", true);
                return;
            }

            string ab_fullname = ListAbilities.Items[ListAbilities.SelectedIndex].ToString();
            string ab_name = ab_fullname.Substring(0, ab_fullname.IndexOf('(')).Trim();
            int prev_ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(ab_name);
            if (prev_ab_id == 0)
            {
                SetStatusText($"Could not find ability {ab_name}", true);
                return;
            }

            if(((CurrentAbilityListMode == AbilityListMode.U1)||(CurrentAbilityListMode == AbilityListMode.U2)) && (ListAbilities.Items.Count <= 1))
            {
                SetStatusText($"There must be at least one upgrade ability in this upgrade line", true);
                return;
            }

            // replace current selected ability

            diff_calculator.CurrentDatabase_ref.AbilityUseCount[prev_ab_id] -= 1;
            if (diff_calculator.CurrentDatabase_ref.AbilityUseCount[prev_ab_id] == 0)
            {
                diff_calculator.CurrentDatabase_ref.AbilityUseCount.Remove(prev_ab_id);
            }

            ListAbilities.Items.RemoveAt(ListAbilities.SelectedIndex);
            for (int i = 0; i < ch.AllAbilities_refs.Count; i++)
            {
                if (ch.AllAbilities_refs[i].ID == prev_ab_id)
                {
                    ch.AllAbilities_refs.RemoveAt(i);
                    break;
                }
            }

            switch (CurrentAbilityListMode)
            {
                case AbilityListMode.BASE:
                    {
                        cpd.BaseAbilities.Remove((ushort)prev_ab_id);
                        ch.Abilities.Remove((ushort)prev_ab_id);
                        for (int i = 0; i < ch.BaseAbilities_refs.Count; i++)
                        {
                            if (ch.BaseAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.BaseAbilities_refs.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;
                case AbilityListMode.U1:
                    {
                        for (int i = 0; i < cpd.U1Abilities.Count; i++)
                        {
                            if (cpd.U1Abilities[i] == (ushort)prev_ab_id)
                            {
                                cpd.U1Abilities.RemoveAt(i);
                                ComboU1.Items.RemoveAt(i);

                                if (cpd.U1Default == (ushort)prev_ab_id)
                                {
                                    cpd.U1Default = cpd.U1Abilities[0];
                                    ComboU1.SelectedIndex = 0;
                                }
                                break;
                            }
                        }

                        int cur_index = ch.Upgrade1.IndexOf(prev_ab_id);
                        ch.Upgrade1.Remove((ushort)prev_ab_id);
                        if(ch.DefaultUpgrade1Index == cur_index)
                        {
                            ch.DefaultUpgrade1Index = 0;
                        }
                        else if(ch.DefaultUpgrade1Index > cur_index)
                        {
                            ch.DefaultUpgrade1Index -= 1;
                        }

                        for (int i = 0; i < ch.UpgradeAbilities1_refs.Count; i++)
                        {
                            if (ch.UpgradeAbilities1_refs[i].ID == prev_ab_id)
                            {
                                ch.UpgradeAbilities1_refs.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < ch.AllUpgradeAbilities_refs.Count; i++)
                        {
                            if (ch.AllUpgradeAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.AllUpgradeAbilities_refs.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;
                case AbilityListMode.U2:
                    {
                        for (int i = 0; i < cpd.U2Abilities.Count; i++)
                        {
                            if (cpd.U2Abilities[i] == (ushort)prev_ab_id)
                            {
                                cpd.U2Abilities.RemoveAt(i);
                                ComboU2.Items.RemoveAt(i);

                                if (cpd.U2Default == (ushort)prev_ab_id)
                                {
                                    cpd.U2Default = cpd.U2Abilities[0];
                                    ComboU2.SelectedIndex = 0;
                                }
                                break;
                            }
                        }

                        int cur_index = ch.Upgrade2.IndexOf(prev_ab_id);
                        ch.Upgrade2.Remove((ushort)prev_ab_id);
                        if (ch.DefaultUpgrade2Index == cur_index)
                        {
                            ch.DefaultUpgrade2Index = 0;
                        }
                        else if (ch.DefaultUpgrade2Index > cur_index)
                        {
                            ch.DefaultUpgrade2Index -= 1;
                        }

                        for (int i = 0; i < ch.UpgradeAbilities2_refs.Count; i++)
                        {
                            if (ch.UpgradeAbilities2_refs[i].ID == prev_ab_id)
                            {
                                ch.UpgradeAbilities2_refs.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < ch.AllUpgradeAbilities_refs.Count; i++)
                        {
                            if (ch.AllUpgradeAbilities_refs[i].ID == prev_ab_id)
                            {
                                ch.AllUpgradeAbilities_refs.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            ChampionRecalculateNoraCost(cpd.ID);
            RefreshChampionChangeInfo();
        }

        private void ComboU1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboU1.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            // find ability
            string ab_fullname = ComboU1.Items[ComboU1.SelectedIndex].ToString();
            string ab_name = ab_fullname.Substring(0, ab_fullname.IndexOf('(')).Trim();
            int ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(ab_name);
            if (ab_id == 0)
            {
                return;
            }

            // find if ability of same kind wasnt already added to the unit
            Pox.Champion ch = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID];

            cpd.U1Default = (ushort)ab_id;
            ch.DefaultUpgrade1Index = ComboU1.SelectedIndex;

            ChampionRecalculateNoraCost(cpd.ID);
            RefreshChampionChangeInfo();
        }

        private void ComboU2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboU2.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            ChampionPatchData cpd = GetChampionPatchDataFromName(ListBoxEntries.Items[ListBoxEntries.SelectedIndex].ToString());
            if (cpd == null)
            {
                return;
            }

            // find ability
            string ab_fullname = ComboU2.Items[ComboU2.SelectedIndex].ToString();
            string ab_name = ab_fullname.Substring(0, ab_fullname.IndexOf('(')).Trim();
            int ab_id = diff_calculator.CurrentDatabase_ref.GetAbilityIDByName(ab_name);
            if (ab_id == 0)
            {
                return;
            }

            // find if ability of same kind wasnt already added to the unit
            Pox.Champion ch = diff_calculator.CurrentDatabase_ref.Champions[cpd.ID];

            cpd.U2Default = (ushort)ab_id;
            ch.DefaultUpgrade2Index = ComboU2.SelectedIndex;

            ChampionRecalculateNoraCost(cpd.ID);
            RefreshChampionChangeInfo();
        }

        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            RuneDescription.database_ref = diff_calculator.PreviousDatabase;
            RefreshChampionChangeInfo();
        }

        private void ButtonCurrent_Click(object sender, EventArgs e)
        {
            RuneDescription.database_ref = diff_calculator.CurrentDatabase_ref;
            RefreshChampionChangeInfo();
        }

        private void PatchBuilderForm_Load(object sender, EventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
        }

        private void PatchBuilderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
        }

        bool PatchSaveHelper()
        {
            PatchBuilderSaveForm pbsv = new PatchBuilderSaveForm();
            pbsv.PatchName = PatchName;
            pbsv.ShowDialog();
            if(pbsv.DialogResult != DialogResult.OK)
            {
                return false;
            }

            if(PatchSave("patches\\" + pbsv.PatchName + ".ptc"))
            {
                PatchName = pbsv.PatchName;
                Text = "Patch Builder - " + PatchName;
                SetStatusText($"Saved patch {PatchName}");
                return true;
            }
            return false;
        }

        private void newPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ListBoxEntries.Items.Count != 0)
            {
                DialogResult dr = MessageBox.Show("Do you want to save current changes?", "Save?", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                if (dr == DialogResult.Yes)
                {
                    if(!PatchSaveHelper())
                    {
                        return;
                    }
                }
            }

            PatchRevert();
            ListBoxEntries.Items.Clear();
            PanelEntryData.Enabled = false;
            PatchName = "";
            Text = "Patch Builder - *untitled*";
            SetStatusText($"Ready", true);
        }

        private void loadPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PatchBuilderLoadForm pblf = new PatchBuilderLoadForm();
            pblf.ShowDialog();
            if (pblf.patch_to_load == "")
            {
                return;
            }

            if (ListBoxEntries.Items.Count != 0)
            {
                DialogResult dr = MessageBox.Show("Do you want to save current changes?", "Save?", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                if (dr == DialogResult.Yes)
                {
                    if (!PatchSaveHelper())
                    {
                        return;
                    }
                }
            }

            PatchLoad(pblf.patch_to_load);
        }

        private void savePatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PatchSaveHelper();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonGetJSON_Click(object sender, EventArgs e)
        {
            // generate son from championpatchdata
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"Champions\":[");
            foreach(var c in PatchChampions.Values)
            {
                sb.Append($"{{\"id\":{c.ID},\"stats\":{{\"dmg\":{c.Damage},\"spd\":{c.Speed},\"minRng\":{c.MinRange},\"rng\":{c.MaxRange},\"hp\":{c.Health},\"noraMod\":{c.NoraMod}}},");

                sb.Append("\"baseAbilities\":[");
                if (c.BaseAbilities.Count != 0)
                {
                    for (int i = 0; i < c.BaseAbilities.Count - 1; i++)
                    {
                        sb.Append($"{c.BaseAbilities[i]},");
                    }
                    sb.Append($"{c.BaseAbilities[c.BaseAbilities.Count - 1]}");
                }
                sb.Append("],");

                sb.Append("\"upgradePath1Abilities\":[");
                if (c.U1Abilities.Count != 0)
                {
                    for (int i = 0; i < c.U1Abilities.Count - 1; i++)
                    {
                        sb.Append($"{c.U1Abilities[i]},");
                    }
                    sb.Append($"{c.U1Abilities[c.U1Abilities.Count - 1]}");
                }
                sb.Append($"],\"upgradePath1DefaultAbility\":{c.U1Default},");

                sb.Append("\"upgradePath2Abilities\":[");
                if (c.U2Abilities.Count != 0)
                {
                    for (int i = 0; i < c.U2Abilities.Count - 1; i++)
                    {
                        sb.Append($"{c.U2Abilities[i]},");
                    }
                    sb.Append($"{c.U2Abilities[c.U2Abilities.Count - 1]}");
                }
                sb.Append($"],\"upgradePath2DefaultAbility\":{c.U2Default}}},");
            }
            if (PatchChampions.Count != 0)   // remove trailing ','
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]}");

            Clipboard.SetText(sb.ToString());
        }
    }
}
