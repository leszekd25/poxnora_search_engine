using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine
{
    public partial class DifferenceCalculator : Form
    {
        enum ChangeListMode { CATEGORY, FACTION, ABILITY };

        Pox.Diff.DatabaseDifferenceCalculator diff_calculator = new Pox.Diff.DatabaseDifferenceCalculator();
        TreeNode selected_treenode = null;
        Font ChangeInfoFont = new Font("Arial", 10);
        ChangeListMode changelist_mode = ChangeListMode.CATEGORY;

        public DifferenceCalculator()
        {
            InitializeComponent();
        }

        private void DifferenceCalculatorForm_Resize(object sender, EventArgs e)
        {
            if (this.Width <= 826)
                return;
            if (this.Height <= 200)
                return;

            ChangesTree.Height = this.Height - 64 - 6 - DatabaseFilter.Height;
            DatabaseFilter.Location = new Point(ChangesTree.Location.X - 3, ChangesTree.Location.Y + ChangesTree.Height + 6);
            PanelChangeList.Width = this.Width - 1108 + ChangesTree.Width + RuneDescription.Width - 64 - 84;
            PanelChangeList.Height = this.Height - 200;
            if (PanelChangeList.Width > (ButtonPrevious.Width + ButtonCurrent.Width + 16))
            {
                ButtonPrevious.Location = new Point(PanelChangeList.Location.X, PanelChangeList.Location.Y + PanelChangeList.Height);
                ButtonCurrent.Location = new Point(PanelChangeList.Location.X + PanelChangeList.Width - ButtonCurrent.Width, PanelChangeList.Location.Y + PanelChangeList.Height);
            }
            else
            {
                ButtonPrevious.Location = new Point(PanelChangeList.Location.X, PanelChangeList.Location.Y + PanelChangeList.Height);//PanelChangeList.Location.X + PanelChangeList.Width - ButtonCurrent.Width, PanelChangeList.Location.Y + PanelChangeList.Height
                ButtonCurrent.Location = new Point(PanelChangeList.Location.X + PanelChangeList.Width - ButtonCurrent.Width, ButtonPrevious.Location.Y + ButtonPrevious.Height + 3);
            }
            RuneDescription.Location = new Point(this.Width - RuneDescription.Width - 16, RuneDescription.Location.Y);
            RuneDescription.SetHeight(this.Height - 64);
        }

        private bool ElementPassesFilter(Pox.DataElement elem)
        {
            return ((!DatabaseFilter.ApplyFilters) || (DatabaseFilter.SearchFilter == null) || (DatabaseFilter.SearchFilter.Satisfies(elem)));
        }

        private void PopulateChampionChangeBrowser(TreeNode tn)
        {
            foreach (var diff_elem in diff_calculator.DifferingChampions)
            {
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.CHAMPION };
                if (diff_calculator.PreviousDatabase.Champions.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Champions[diff_elem.id]))
                        passes_filter = true;

                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Champions[diff_elem.id];
                }
                if (diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter)&&(ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Champions[diff_elem.id])))
                        passes_filter = true;

                    diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Champions[diff_elem.id];
                }

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }
            tn.Text = string.Format("Champions ({0} changes)", tn.Nodes.Count);
        }

        private void PopulateAbilityChangeBrowser(TreeNode tn)
        {
            foreach (var diff_elem in diff_calculator.DifferingAbilities)
            {
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.ABILITY };
                if (diff_calculator.PreviousDatabase.Abilities.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Abilities[diff_elem.id]))
                        passes_filter = true;

                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Abilities[diff_elem.id];
                }
                if (diff_calculator.CurrentDatabase_ref.Abilities.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Abilities[diff_elem.id])))
                        passes_filter = true;

                    diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Abilities[diff_elem.id];
                }

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }
            tn.Text = string.Format("Abilities ({0} changes)", tn.Nodes.Count);
        }

        private void PopulateSpellChangeBrowser(TreeNode tn)
        {
            foreach (var diff_elem in diff_calculator.DifferingSpells)
            {
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.SPELL };
                if (diff_calculator.PreviousDatabase.Spells.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Spells[diff_elem.id]))
                        passes_filter = true;

                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Spells[diff_elem.id];
                }
                if (diff_calculator.CurrentDatabase_ref.Spells.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Spells[diff_elem.id])))
                        passes_filter = true;

                    diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Spells[diff_elem.id];
                }

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }
            tn.Text = string.Format("Spells ({0} changes)", tn.Nodes.Count);
        }

        private void PopulateRelicChangeBrowser(TreeNode tn)
        {
            foreach (var diff_elem in diff_calculator.DifferingRelics)
            {
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.RELIC };
                if (diff_calculator.PreviousDatabase.Relics.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Relics[diff_elem.id]))
                        passes_filter = true;

                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Relics[diff_elem.id];
                }
                if (diff_calculator.CurrentDatabase_ref.Relics.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Relics[diff_elem.id])))
                        passes_filter = true;

                    diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Relics[diff_elem.id];
                }

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }
            tn.Text = string.Format("Relics ({0} changes)", tn.Nodes.Count);
        }

        private void PopulateEquipmentChangeBrowser(TreeNode tn)
        {
            foreach (var diff_elem in diff_calculator.DifferingEquipments)
            {
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.EQUIPMENT };
                if (diff_calculator.PreviousDatabase.Equipments.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Equipments[diff_elem.id]))
                        passes_filter = true;

                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Equipments[diff_elem.id];
                }
                if (diff_calculator.CurrentDatabase_ref.Equipments.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Equipments[diff_elem.id])))
                        passes_filter = true;

                    diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Equipments[diff_elem.id];
                }

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }
            tn.Text = string.Format("Equipments ({0} changes)", tn.Nodes.Count);
        }

        private void PopulateChangeBrowserPerCategory()
        {
            ChangesTree.Nodes.Clear();

            ChangesTree.Nodes.Add(new TreeNode() { Name = "Champions", Text = "Champions" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Abilities", Text = "Abilities" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Spells", Text = "Spells" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Relics", Text = "Relics" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Equipments", Text = "Equipments" });

            PopulateChampionChangeBrowser(ChangesTree.Nodes["Champions"]);
            PopulateAbilityChangeBrowser(ChangesTree.Nodes["Abilities"]);
            PopulateSpellChangeBrowser(ChangesTree.Nodes["Spells"]);
            PopulateRelicChangeBrowser(ChangesTree.Nodes["Relics"]);
            PopulateEquipmentChangeBrowser(ChangesTree.Nodes["Equipments"]);
        }

        private void PopulateFactionChangeBrowser(TreeNode tn)
        {
            // champions
            foreach (var diff_elem in diff_calculator.DifferingChampions)
            {
                bool contains_faction = false;
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.CHAMPION };

                if (diff_calculator.PreviousDatabase.Champions.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Champions[diff_elem.id]))
                        passes_filter = true;

                    if (diff_calculator.PreviousDatabase.Champions[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.PreviousElement = diff_calculator.PreviousDatabase.Champions[diff_elem.id];
                        contains_faction = true;
                    }
                }
                if (diff_calculator.CurrentDatabase_ref.Champions.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Champions[diff_elem.id])))
                        passes_filter = true;

                    if (diff_calculator.CurrentDatabase_ref.Champions[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Champions[diff_elem.id];
                        contains_faction = true;
                    }
                }

                if (!contains_faction)
                    continue;

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }

            // spells
            foreach (var diff_elem in diff_calculator.DifferingSpells)
            {
                bool contains_faction = false;
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.SPELL };

                if (diff_calculator.PreviousDatabase.Spells.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Spells[diff_elem.id]))
                        passes_filter = true;

                    if (diff_calculator.PreviousDatabase.Spells[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.PreviousElement = diff_calculator.PreviousDatabase.Spells[diff_elem.id];
                        contains_faction = true;
                    }
                }
                if (diff_calculator.CurrentDatabase_ref.Spells.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Spells[diff_elem.id])))
                        passes_filter = true;

                    if (diff_calculator.CurrentDatabase_ref.Spells[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Spells[diff_elem.id];
                        contains_faction = true;
                    }
                }

                if (!contains_faction)
                    continue;

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }

            // relics
            foreach (var diff_elem in diff_calculator.DifferingRelics)
            {
                bool contains_faction = false;
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.RELIC };

                if (diff_calculator.PreviousDatabase.Relics.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Relics[diff_elem.id]))
                        passes_filter = true;

                    if (diff_calculator.PreviousDatabase.Relics[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.PreviousElement = diff_calculator.PreviousDatabase.Relics[diff_elem.id];
                        contains_faction = true;
                    }
                }
                if (diff_calculator.CurrentDatabase_ref.Relics.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Relics[diff_elem.id])))
                        passes_filter = true;

                    if (diff_calculator.CurrentDatabase_ref.Relics[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Relics[diff_elem.id];
                        contains_faction = true;
                    }
                }

                if (!contains_faction)
                    continue;

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }

            // equipments
            foreach (var diff_elem in diff_calculator.DifferingEquipments)
            {
                bool contains_faction = false;
                bool passes_filter = false;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.EQUIPMENT };

                if (diff_calculator.PreviousDatabase.Equipments.ContainsKey(diff_elem.id))
                {
                    if (ElementPassesFilter(diff_calculator.PreviousDatabase.Equipments[diff_elem.id]))
                        passes_filter = true;

                    if (diff_calculator.PreviousDatabase.Equipments[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.PreviousElement = diff_calculator.PreviousDatabase.Equipments[diff_elem.id];
                        contains_faction = true;
                    }
                }
                if (diff_calculator.CurrentDatabase_ref.Equipments.ContainsKey(diff_elem.id))
                {
                    if ((!passes_filter) && (ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Equipments[diff_elem.id])))
                        passes_filter = true;

                    if (diff_calculator.CurrentDatabase_ref.Equipments[diff_elem.id].Faction.Contains(tn.Name))
                    {
                        diff_link.CurrentElement = diff_calculator.CurrentDatabase_ref.Equipments[diff_elem.id];
                        contains_faction = true;
                    }
                }

                if (!contains_faction)
                    continue;

                if (!passes_filter)
                    continue;

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }

            tn.Text = string.Format("{0} ({1} changes)", tn.Name, tn.Nodes.Count);
        }

        private void PopulateChangeBrowserPerFaction()
        {
            ChangesTree.Nodes.Clear();

            ChangesTree.Nodes.Add(new TreeNode() { Name = "Forglar Swamp", Text = "Forglar Swamp" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Forsaken Wastes", Text = "Forsaken Wastes" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "K'thir Forest", Text = "K'thir Forest" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Ironfist Stronghold", Text = "Ironfist Stronghold" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Savage Tundra", Text = "Savage Tundra" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Shattered Peaks", Text = "Shattered Peaks" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Sundered Lands", Text = "Sundered Lands" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Underdepths", Text = "Underdepths" });

            foreach (TreeNode n in ChangesTree.Nodes)
                PopulateFactionChangeBrowser(n);

            ChangesTree.Nodes.Add(new TreeNode() { Name = "Abilities", Text = "Abilities" });
            PopulateAbilityChangeBrowser(ChangesTree.Nodes["Abilities"]);
        }

        private void PopulateAbilityAffectedChampionBrowser(TreeNode tn)
        {
            // 1. find all abilities that changed
            HashSet<int> AbilityList = new HashSet<int>();
            foreach (var diff_elem in diff_calculator.DifferingAbilities)
            {
                if ((diff_calculator.PreviousDatabase.Abilities.ContainsKey(diff_elem.id)) && (diff_calculator.CurrentDatabase_ref.Abilities.ContainsKey(diff_elem.id)))
                    AbilityList.Add(diff_elem.id);
            }

            // 2. find all champions that now have abilities that changed cost
            List<Pox.Diff.DifferenceLink> ChampionList = new List<Pox.Diff.DifferenceLink>();
            foreach (var kv in diff_calculator.CurrentDatabase_ref.Champions)
            {
                if (!ElementPassesFilter(diff_calculator.CurrentDatabase_ref.Champions[kv.Key]))
                    continue;

                bool contains_changed_ability = false;
                foreach (var ab in kv.Value.AllAbilities_refs)
                {
                    if (AbilityList.Contains(ab.ID))
                    {
                        contains_changed_ability = true;
                        break;
                    }
                }

                if (!contains_changed_ability)
                    continue;

                Pox.Diff.DifferenceLink diff_link = new Pox.Diff.DifferenceLink() { ElemType = Pox.DataElement.ElementType.CHAMPION };
                diff_link.CurrentElement = kv.Value;
                if (diff_calculator.PreviousDatabase.Champions.ContainsKey(kv.Key))
                {
                    diff_link.PreviousElement = diff_calculator.PreviousDatabase.Champions[kv.Key];
                }

                tn.Nodes.Add(new TreeNode() { Text = diff_link.ToString(), Tag = diff_link });
            }

            tn.Text = string.Format("{0} ({1} changes)", tn.Name, tn.Nodes.Count);
        }

        private void PopulateChangeBrowserPerAbility()
        {
            ChangesTree.Nodes.Clear();

            ChangesTree.Nodes.Add(new TreeNode() { Name = "Champions", Text = "Champions" });
            ChangesTree.Nodes.Add(new TreeNode() { Name = "Abilities", Text = "Abilities" });

            PopulateAbilityAffectedChampionBrowser(ChangesTree.Nodes["Champions"]);
            PopulateAbilityChangeBrowser(ChangesTree.Nodes["Abilities"]);
        }

        private void PopulateChangeBrowser()
        {
            switch(changelist_mode)
            {
                case ChangeListMode.CATEGORY:
                    PopulateChangeBrowserPerCategory();
                    break;
                case ChangeListMode.FACTION:
                    PopulateChangeBrowserPerFaction();
                    break;
                case ChangeListMode.ABILITY:
                    PopulateChangeBrowserPerAbility();
                    break;
            }
        }

        private void DifferenceCalculatorForm_Load(object sender, EventArgs e)
        {
            if (LoadOldDatabaseDialog.ShowDialog() != DialogResult.OK)
            {
                this.Close();
                return;
            }

            diff_calculator.LoadDatabases(LoadOldDatabaseDialog.FileName);
            if(!diff_calculator.ready)
            {
                MessageBox.Show("Could not load databases...");
                this.Close();
                return;
            }

            diff_calculator.Calculate();
            PopulateChangeBrowser();

            DatabaseFilter.ApplyFilters_callback = ApplyFilters;

            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
        }

        private void DifferenceCalculator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
        }

        private void ClearChangeInfo()
        {
            PanelChangeList.Controls.Clear();
        }

        private void PushChangeInfo(string info)
        {
            Point previous_location = new Point(3, 3);
            int previous_height = 0;
            if (PanelChangeList.Controls.Count != 0)
            {
                previous_location = PanelChangeList.Controls[PanelChangeList.Controls.Count - 1].Location;
                previous_height = PanelChangeList.Controls[PanelChangeList.Controls.Count - 1].Size.Height;
            }

            PanelChangeList.Controls.Add(new Label()
            {
                Location = new Point(previous_location.X, previous_location.Y + previous_height + 5),
                Text = info,
                MaximumSize = new Size(PanelChangeList.Width - 32, 0),
                AutoSize = true,
                Font = ChangeInfoFont
            });
        }

        private void LoadDataElementChangeInfo(Pox.DataElement prev_elem, Pox.DataElement curr_elem)
        {
            int prev_changes = PanelChangeList.Controls.Count;

            if (prev_elem.Name != curr_elem.Name)
                PushChangeInfo(string.Format("Name changed from {0} to {1}", prev_elem.Name, curr_elem.Name));
            if (prev_elem.Description != curr_elem.Description)
                PushChangeInfo(string.Format("Description changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_elem.Description, curr_elem.Description));
            if (!(prev_elem is Pox.Champion))
            {
                if (prev_elem.NoraCost < curr_elem.NoraCost)
                    PushChangeInfo(string.Format("Nora cost increased by {0}", curr_elem.NoraCost - prev_elem.NoraCost));
                if (prev_elem.NoraCost > curr_elem.NoraCost)
                    PushChangeInfo(string.Format("Nora cost decreased by {0}", prev_elem.NoraCost - curr_elem.NoraCost));
            }
            else
            {
                if (((Pox.Champion)prev_elem).BaseNoraCost < ((Pox.Champion)curr_elem).BaseNoraCost)
                    PushChangeInfo(string.Format("Nora cost increased by {0}", ((Pox.Champion)curr_elem).BaseNoraCost - ((Pox.Champion)prev_elem).BaseNoraCost));
                if (((Pox.Champion)prev_elem).BaseNoraCost > ((Pox.Champion)curr_elem).BaseNoraCost)
                    PushChangeInfo(string.Format("Nora cost decreased by {0}", ((Pox.Champion)prev_elem).BaseNoraCost - ((Pox.Champion)curr_elem).BaseNoraCost));
            }

            if (prev_changes != PanelChangeList.Controls.Count)
                PushChangeInfo("");
        }

        private void LoadRuneChangeInfo(Pox.Rune prev_rune, Pox.Rune curr_rune)
        {
            LoadDataElementChangeInfo(prev_rune, curr_rune);

            int prev_changes = PanelChangeList.Controls.Count;

            if (prev_rune.Rarity != curr_rune.Rarity)
                PushChangeInfo(string.Format("Rarity changed from {0} to {1}", prev_rune.Rarity, curr_rune.Rarity));
            foreach (string f in prev_rune.Faction)
                if (!(curr_rune.Faction.Contains(f)))
                    PushChangeInfo(string.Format("Faction removed: {0}", f));
            foreach (string f in curr_rune.Faction)
                if (!(prev_rune.Faction.Contains(f)))
                    PushChangeInfo(string.Format("Faction added: {0}", f));
            if (prev_rune.ForSale != curr_rune.ForSale)
                PushChangeInfo(prev_rune.ForSale ? "No longer for sale" : "Now for sale");
            if (prev_rune.Tradeable != curr_rune.Tradeable)
                PushChangeInfo(prev_rune.Tradeable ? "No longer tradeable" : "Now tradeable");
            if (prev_rune.AllowRanked != curr_rune.AllowRanked)
                PushChangeInfo(prev_rune.AllowRanked ? "Now banned from ranked play" : "No longer banned from ranked play");
            if (prev_rune.DeckLimit != curr_rune.DeckLimit)
                PushChangeInfo(string.Format("Deck limit changed from {0} to {1}", prev_rune.DeckLimit, curr_rune.DeckLimit));
            if (prev_rune.Cooldown != curr_rune.Cooldown)
                PushChangeInfo(string.Format("Cooldown changed from {0} to {1}", prev_rune.Cooldown, curr_rune.Cooldown));

            if (prev_changes != PanelChangeList.Controls.Count)
                PushChangeInfo("");
        }

        private void LoadChampionChangeInfo(Pox.Champion prev_champion, Pox.Champion curr_champion)
        {
            LoadRuneChangeInfo(prev_champion, curr_champion);

            if ((prev_champion.MinRNG != curr_champion.MinRNG) || (prev_champion.MaxRNG != curr_champion.MaxRNG))
                PushChangeInfo(string.Format("Range changed from {0}-{1} to {2}-{3}", prev_champion.MinRNG, prev_champion.MaxRNG, curr_champion.MinRNG, curr_champion.MaxRNG));
            if (prev_champion.Defense != curr_champion.Defense)
                PushChangeInfo(string.Format("Defense changed from {0} to {1}", prev_champion.Defense, curr_champion.Defense));
            if (prev_champion.HitPoints != curr_champion.HitPoints)
                PushChangeInfo(string.Format("Hit points changed from {0} to {1}", prev_champion.HitPoints, curr_champion.HitPoints));
            if (prev_champion.Damage != curr_champion.Damage)
                PushChangeInfo(string.Format("Damage changed from {0} to {1}", prev_champion.Damage, curr_champion.Damage));
            if (prev_champion.Speed != curr_champion.Speed)
                PushChangeInfo(string.Format("Speed changed from {0} to {1}", prev_champion.Speed, curr_champion.Speed));
            if (prev_champion.Size != curr_champion.Size)
                PushChangeInfo(string.Format("Size changed from {0}x{1} to {2}x{3}", prev_champion.Size, prev_champion.Size, curr_champion.Size, curr_champion.Size));
            foreach (string c in prev_champion.Class)
                if (!(curr_champion.Class.Contains(c)))
                    PushChangeInfo(string.Format("Class removed: {0}", c));
            foreach (string c in curr_champion.Class)
                if (!(prev_champion.Class.Contains(c)))
                    PushChangeInfo(string.Format("Class added: {0}", c));
            foreach (string r in prev_champion.Race)
                if (!(curr_champion.Race.Contains(r)))
                    PushChangeInfo(string.Format("Race removed: {0}", r));
            foreach (string r in curr_champion.Race)
                if (!(prev_champion.Race.Contains(r)))
                    PushChangeInfo(string.Format("Race added: {0}", r));
            foreach (int a in prev_champion.Abilities)
                if (!(curr_champion.Abilities.Contains(a)))
                    PushChangeInfo(string.Format("Ability removed: {0}", diff_calculator.PreviousDatabase.Abilities[a].ToString()));
            foreach (int a in curr_champion.Abilities)
                if (!(prev_champion.Abilities.Contains(a)))
                    PushChangeInfo(string.Format("Ability added: {0}", diff_calculator.CurrentDatabase_ref.Abilities[a].ToString()));

            bool upgrade1_changed = false;
            foreach (int a in prev_champion.Upgrade1)
                if (!(curr_champion.Upgrade1.Contains(a)))
                {
                    upgrade1_changed = true;
                    break;
                }
            foreach (int a in curr_champion.Upgrade1)
                if (!(prev_champion.Upgrade1.Contains(a)))
                {
                    upgrade1_changed = true;
                    break;
                }
            if (upgrade1_changed)
            {
                StringBuilder sb = new StringBuilder("Upgrade 1 changed\r\nOld:\r\n");
                List<int> upg1_sorted = new List<int>(prev_champion.Upgrade1); upg1_sorted.Sort();
                foreach (int a in upg1_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", diff_calculator.PreviousDatabase.Abilities[a].ToString(), prev_champion.IsUpgrade1Default(a) ? " (default)" : ""));
                sb.Append("New:\r\n");
                upg1_sorted = new List<int>(curr_champion.Upgrade1); upg1_sorted.Sort();
                foreach (int a in upg1_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", diff_calculator.CurrentDatabase_ref.Abilities[a].ToString(), curr_champion.IsUpgrade1Default(a) ? " (default)" : ""));

                PushChangeInfo(sb.ToString());
            }
            else
            {
                if (prev_champion.Upgrade1[prev_champion.DefaultUpgrade1Index] != curr_champion.Upgrade1[curr_champion.DefaultUpgrade1Index])
                    PushChangeInfo(string.Format("Default upgrade 1 changed from {0} to {1}",
                        diff_calculator.PreviousDatabase.Abilities[prev_champion.Upgrade1[prev_champion.DefaultUpgrade1Index]].ToString(),
                        diff_calculator.CurrentDatabase_ref.Abilities[curr_champion.Upgrade1[curr_champion.DefaultUpgrade1Index]].ToString()));
            }

            bool upgrade2_changed = false;
            foreach (int a in prev_champion.Upgrade2)
                if (!(curr_champion.Upgrade2.Contains(a)))
                {
                    upgrade2_changed = true;
                    break;
                }
            foreach (int a in curr_champion.Upgrade2)
                if (!(prev_champion.Upgrade2.Contains(a)))
                {
                    upgrade2_changed = true;
                    break;
                }
            if (upgrade2_changed)
            {
                StringBuilder sb = new StringBuilder("Upgrade 2 changed\r\nOld:\r\n");
                List<int> upg2_sorted = new List<int>(prev_champion.Upgrade2); upg2_sorted.Sort();
                foreach (int a in upg2_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", diff_calculator.PreviousDatabase.Abilities[a].ToString(), prev_champion.IsUpgrade2Default(a) ? " (default)" : ""));
                sb.Append("New:\r\n");
                upg2_sorted = new List<int>(curr_champion.Upgrade2); upg2_sorted.Sort();
                foreach (int a in upg2_sorted)
                    sb.Append(string.Format("{0}{1}\r\n", diff_calculator.CurrentDatabase_ref.Abilities[a].ToString(), curr_champion.IsUpgrade2Default(a) ? " (default)" : ""));

                PushChangeInfo(sb.ToString());
            }
            else
            {
                if (prev_champion.Upgrade2[prev_champion.DefaultUpgrade2Index] != curr_champion.Upgrade2[curr_champion.DefaultUpgrade2Index])
                    PushChangeInfo(string.Format("Default upgrade 2 changed from {0} to {1}",
                        diff_calculator.PreviousDatabase.Abilities[prev_champion.Upgrade2[prev_champion.DefaultUpgrade2Index]].ToString(),
                        diff_calculator.CurrentDatabase_ref.Abilities[curr_champion.Upgrade2[curr_champion.DefaultUpgrade2Index]].ToString()));
            }

            // special: calculate new prognosed nora cost difference
            if (prev_champion.PrognosedBaseNoraCostDifference != curr_champion.PrognosedBaseNoraCostDifference)
                PushChangeInfo(string.Format("Prognosed nora cost difference changed from {0} to {1}",
                    prev_champion.PrognosedBaseNoraCostDifference, curr_champion.PrognosedBaseNoraCostDifference));
        }

        private void LoadAbilityChangeInfo(Pox.Ability prev_ability, Pox.Ability curr_ability)
        {
            LoadDataElementChangeInfo(prev_ability, curr_ability);

            if (prev_ability.APCost != curr_ability.APCost)
                PushChangeInfo(string.Format("AP cost changed from {0} to {1}", prev_ability.APCost, curr_ability.APCost));
            if (prev_ability.Cooldown != curr_ability.Cooldown)
                PushChangeInfo(string.Format("Cooldown changed from {0} to {1}", prev_ability.Cooldown, curr_ability.Cooldown));
        }

        private void LoadSpellChangeInfo(Pox.Spell prev_spell, Pox.Spell curr_spell)
        {
            LoadRuneChangeInfo(prev_spell, curr_spell);

            if (prev_spell.Flavor != curr_spell.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_spell.Flavor, curr_spell.Flavor));
        }

        private void LoadEquipmentChangeInfo(Pox.Equipment prev_equipment, Pox.Equipment curr_equipment)
        {
            LoadRuneChangeInfo(prev_equipment, curr_equipment);

            if (prev_equipment.Flavor != curr_equipment.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_equipment.Flavor, curr_equipment.Flavor));
        }

        private void LoadRelicChangeInfo(Pox.Relic prev_relic, Pox.Relic curr_relic)
        {
            LoadRuneChangeInfo(prev_relic, curr_relic);

            if (prev_relic.Defense != curr_relic.Defense)
                PushChangeInfo(string.Format("Defense changed from {0} to {1}", prev_relic.Defense, curr_relic.Defense));
            if (prev_relic.HitPoints != curr_relic.HitPoints)
                PushChangeInfo(string.Format("Hit points changed from {0} to {1}", prev_relic.HitPoints, curr_relic.HitPoints));
            if (prev_relic.Size != curr_relic.Size)
                PushChangeInfo(string.Format("Size changed from {0}x{1} to {2}x{2}", prev_relic.Size, prev_relic.Size, curr_relic.Size, curr_relic.Size));
            if (prev_relic.Flavor != curr_relic.Flavor)
                PushChangeInfo(string.Format("Flavor text changed\r\nOld:\r\n    {0}\r\nNew:\r\n    {1}", prev_relic.Flavor, curr_relic.Flavor));
        }


        private void LoadChangeInfo(Pox.Diff.DifferenceLink diff_link)
        {
            ButtonPrevious.Show();
            ButtonCurrent.Show();

            Pox.DataElement prev = diff_link.PreviousElement;
            Pox.DataElement curr = diff_link.CurrentElement;

            if (prev == null)
            {
                if (curr == null)
                {
                    PanelChangeList.Controls.Add(new Label() { Text = "<!!!INVALID!!!>" });
                    ButtonCurrent.Hide();
                }
                else
                {
                    PanelChangeList.Controls.Add(new Label() { Text = "Added" });
                }

                ButtonPrevious.Hide();
                return;
            }
            else if (curr == null)
            {
                PanelChangeList.Controls.Add(new Label() { Text = "Removed" });
                ButtonCurrent.Hide();
                return;
            }

            // load data element change info
            switch(diff_link.ElemType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    LoadChampionChangeInfo((Pox.Champion)prev, (Pox.Champion)curr);
                    break;
                case Pox.DataElement.ElementType.ABILITY:
                    LoadAbilityChangeInfo((Pox.Ability)prev, (Pox.Ability)curr);
                    break;
                case Pox.DataElement.ElementType.SPELL:
                    LoadSpellChangeInfo((Pox.Spell)prev, (Pox.Spell)curr);
                    break;
                case Pox.DataElement.ElementType.EQUIPMENT:
                    LoadEquipmentChangeInfo((Pox.Equipment)prev, (Pox.Equipment)curr);
                    break;
                case Pox.DataElement.ElementType.RELIC:
                    LoadRelicChangeInfo((Pox.Relic)prev, (Pox.Relic)curr);
                    break;
                default: 
                    break;
            }
        }

        private bool GetElementTypeFromNode(TreeNode tn, out Pox.DataElement.ElementType elem_type)
        {
            elem_type = Pox.DataElement.ElementType.CHAMPION;

            if (tn.Tag == null)
                return false;

            elem_type = ((Pox.Diff.DifferenceLink)tn.Tag).ElemType;
            return true;
        }

        private void ShowDataElementDescription(Pox.Diff.DifferenceLink link, Pox.Diff.DifferenceLink.ItemType it)
        {
            RuneDescription.TracerClear();

            if (link == null)
                return;

            Pox.DataElement elem = null;

            if (it == Pox.Diff.DifferenceLink.ItemType.PREVIOUS)
            {
                if (link.PreviousElement == null)
                    return;

                RuneDescription.database_ref = diff_calculator.PreviousDatabase;
                elem = link.PreviousElement;
            }
            else
            {
                if (link.CurrentElement == null)
                    return;

                RuneDescription.database_ref = diff_calculator.CurrentDatabase_ref;
                elem = link.CurrentElement;
            }

            switch (link.ElemType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    RuneDescription.SetChampionRune(RuneDescription.database_ref.Champions[elem.ID],
                        RuneDescription.database_ref.Champions[elem.ID].DefaultUpgrade1Index,
                        RuneDescription.database_ref.Champions[elem.ID].DefaultUpgrade2Index);
                    break;
                case Pox.DataElement.ElementType.ABILITY:
                    RuneDescription.SetAbility(RuneDescription.database_ref.Abilities[elem.ID]);
                    break;
                case Pox.DataElement.ElementType.SPELL:
                    RuneDescription.SetSpellRune(RuneDescription.database_ref.Spells[elem.ID]);
                    break;
                case Pox.DataElement.ElementType.RELIC:
                    RuneDescription.SetRelicRune(RuneDescription.database_ref.Relics[elem.ID]);
                    break;
                case Pox.DataElement.ElementType.EQUIPMENT:
                    RuneDescription.SetEquipmentRune(RuneDescription.database_ref.Equipments[elem.ID]);
                    break;
                default:
                    RuneDescription.ClearDescription();
                    break;
            }
        }

        private void ChangesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selected_treenode = e.Node;

            if (e.Node == null)
                return;
            if (e.Node.Tag == null)
                return;

            Pox.DataElement.ElementType elem_type;
            if (!GetElementTypeFromNode(e.Node, out elem_type))
                return;

            ClearChangeInfo();

            LoadChangeInfo((Pox.Diff.DifferenceLink)(e.Node.Tag));

            ButtonPrevious.Tag = (Pox.Diff.DifferenceLink)(e.Node.Tag);
            ButtonCurrent.Tag = (Pox.Diff.DifferenceLink)(e.Node.Tag);

            if (((Pox.Diff.DifferenceLink)(e.Node.Tag)).CurrentElement != null)
                ShowDataElementDescription((Pox.Diff.DifferenceLink)(e.Node.Tag), Pox.Diff.DifferenceLink.ItemType.CURRENT);
            else
                ShowDataElementDescription((Pox.Diff.DifferenceLink)(e.Node.Tag), Pox.Diff.DifferenceLink.ItemType.PREVIOUS);
        }

        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            if (ButtonPrevious.Tag == null)
                return;

            if (((Pox.Diff.DifferenceLink)(ButtonPrevious.Tag)).PreviousElement != null)
                ShowDataElementDescription((Pox.Diff.DifferenceLink)(ButtonPrevious.Tag), Pox.Diff.DifferenceLink.ItemType.PREVIOUS);
        }

        private void ButtonCurrent_Click(object sender, EventArgs e)
        {
            if (ButtonCurrent.Tag == null)
                return;

            if (((Pox.Diff.DifferenceLink)(ButtonPrevious.Tag)).CurrentElement != null)
                ShowDataElementDescription((Pox.Diff.DifferenceLink)(ButtonCurrent.Tag), Pox.Diff.DifferenceLink.ItemType.CURRENT);
        }

        private void DifferenceCalculator_Deactivate(object sender, EventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
        }

        private void DifferenceCalculator_Activated(object sender, EventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
        }

        private void showChangesPerCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changelist_mode == ChangeListMode.CATEGORY)
                return;

            changelist_mode = ChangeListMode.CATEGORY;
            PopulateChangeBrowser();
        }

        private void showChangesPerFactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changelist_mode == ChangeListMode.FACTION)
                return;
            
            changelist_mode = ChangeListMode.FACTION;
            PopulateChangeBrowser();
        }

        private void showChampionsAffectedByAbilityChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changelist_mode == ChangeListMode.ABILITY)
                return;

            changelist_mode = ChangeListMode.ABILITY;
            PopulateChangeBrowser();
        }

        private void ApplyFilters()
        {
            PopulateChangeBrowser();
        }
    }
}
