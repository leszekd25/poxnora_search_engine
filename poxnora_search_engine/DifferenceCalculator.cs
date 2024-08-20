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
        ChangeListMode changelist_mode = ChangeListMode.CATEGORY;
        Pox.Diff.DifferencePresentation diff_presentation;

        SynergyComparerForm synergy_form = null;

        public DifferenceCalculator()
        {
            InitializeComponent();

            diff_presentation = new Pox.Diff.DifferencePresentation()
            {
                PanelRef = PanelChangeList,
                ButtonPreviousRef = ButtonPrevious,
                ButtonCurrentRef = ButtonCurrent,
                DiffCalculatorRef = diff_calculator
            };
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
                if ((diff_calculator.CurrentDatabase_ref.Abilities.ContainsKey(diff_elem.id)) && (diff_calculator.CurrentDatabase_ref.AbilityUseCount.ContainsKey(diff_elem.id)) && (diff_calculator.CurrentDatabase_ref.AbilityUseCount[diff_elem.id] != 0))
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

                //sb.AppendLine(diff_link.PreviousElement.ToString());
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
            //sb.Clear();
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
            //if (sb.Length == 0)
            //    return;

           // Clipboard.SetText(sb.ToString());
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

            diff_presentation.ClearChangeInfo();

            diff_presentation.LoadChangeInfo((Pox.Diff.DifferenceLink)(e.Node.Tag));

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

        private void ButtonSynergyComparer_Click(object sender, EventArgs e)
        {
            if(synergy_form != null)
            {
                synergy_form.Focus();
                return;
            }

            synergy_form = new SynergyComparerForm();
            synergy_form.Show();
            synergy_form.Init(diff_calculator.PreviousDatabase, diff_calculator.CurrentDatabase_ref);
            synergy_form.FormClosed += OnSynergyFormClosed;
        }

        private void OnSynergyFormClosed(object sender, EventArgs e)
        {
            synergy_form.FormClosed -= OnSynergyFormClosed;
            synergy_form = null;
        }
    }
}
