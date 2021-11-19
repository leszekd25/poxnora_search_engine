using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using poxnora_search_engine.Pox;
using poxnora_search_engine.Pox.Filters;
using poxnora_search_engine.Pox.FilterControls;

namespace poxnora_search_engine.Pox
{
    public partial class DatabaseFilterControl : UserControl
    {
        public delegate void OnApplyFilters();

        public Pox.Filters.BaseFilter SearchFilter = null;
        public OnApplyFilters ApplyFilters_callback = null;
        public bool ApplyFilters = false;

        BaseFilterControl FilterProperties = null;

        public DatabaseFilterControl()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            if(FilterTree.Nodes.Count != 0)
                RemoveFilter(FilterTree.Nodes[0]);
        }


        private void AddFilter(string fname, FilterType ftype, DataPath dpath = DataPath.None)
        {
            Log.Info(Log.LogSource.UI, "MainForm.AddFilter() called");

            Pox.Filters.BaseFilter bf;

            switch (ftype)
            {
                case FilterType.AND:
                    bf = new Pox.Filters.AndFilter();
                    break;
                case FilterType.OR:
                    bf = new Pox.Filters.OrFilter();
                    break;
                case FilterType.INT:
                    bf = new Pox.Filters.IntFilter() { dpath = dpath, FilterType = Pox.Filters.IntFilterType.EQUAL, RefValue = 0 };
                    break;
                case FilterType.STRING:
                    bf = new Pox.Filters.StringFilter() { dpath = dpath, IgnoreCase = true, FilterType = Pox.Filters.StringFilterType.CONTAINS, RefValue = "", RefValueLowerCase = "" };
                    break;
                case FilterType.BOOLEAN:
                    bf = new Pox.Filters.BooleanFilter() { dpath = dpath, FilterType = Pox.Filters.BooleanFilterType.EQUAL, RefValue = true };
                    break;
                case FilterType.EXPANSION:
                    bf = new Pox.Filters.EnumFilter() { dpath = DataPath.Expansion, Options_ref = Program.database.Expansions, FilterType = Pox.Filters.EnumFilterType.EQUAL, RefValue = "" };
                    break;
                case FilterType.RARITY:
                    bf = new Pox.Filters.EnumFilter() { dpath = DataPath.Rarity, Options_ref = Program.database.Rarities, FilterType = Pox.Filters.EnumFilterType.EQUAL, RefValue = "" };
                    break;
                case FilterType.ABILITY_LIST:
                    bf = new Pox.Filters.AbilityListFilter() { dpath = dpath, FilterType = Pox.Filters.AbilityListFilterType.CONTAINS, RefValue = 0 };
                    break;
                case FilterType.CLASS_LIST:
                    bf = new Pox.Filters.EnumListFilter() { dpath = DataPath.Class, Options_ref = Program.database.Classes, FilterType = Pox.Filters.EnumListFilterType.CONTAINS, RefValue = "" };
                    break;
                case FilterType.FACTION_LIST:
                    bf = new Pox.Filters.EnumListFilter() { dpath = DataPath.Faction, Options_ref = Program.database.Factions, FilterType = Pox.Filters.EnumListFilterType.CONTAINS, RefValue = "" };
                    break;
                case FilterType.RACE_LIST:
                    bf = new Pox.Filters.EnumListFilter() { dpath = DataPath.Race, Options_ref = Program.database.Races, FilterType = Pox.Filters.EnumListFilterType.CONTAINS, RefValue = "" };
                    break;

                default:
                    Log.Error(Log.LogSource.UI, "Form1.AddFilter(): Unknown filter type");
                    throw new Exception("Form1.AddFilter(): Error while adding filter");
            }
            bf.Name = fname;

            // determine which filter is selected right now
            if (FilterTree.Nodes.Count == 0)
            {
                FilterTree.Nodes.Add(new TreeNode() { Text = bf.ToString(), Tag = bf });
                SearchFilter = null;
                SearchFilter = bf;
            }
            else
            {
                TreeNode selected_node = FilterTree.SelectedNode;
                if (selected_node == null)
                    selected_node = FilterTree.Nodes[0];
                if (selected_node.Tag is Pox.Filters.AndFilter)
                {
                    selected_node.Nodes.Add(new TreeNode() { Text = bf.ToString(), Tag = bf });
                    ((Pox.Filters.AndFilter)selected_node.Tag).Filters.Add(bf);
                }
                else if (selected_node.Tag is Pox.Filters.OrFilter)
                {
                    selected_node.Nodes.Add(new TreeNode() { Text = bf.ToString(), Tag = bf });
                    ((Pox.Filters.OrFilter)selected_node.Tag).Filters.Add(bf);
                }
                else if (selected_node.Parent != null)
                {
                    if (selected_node.Parent.Tag is Pox.Filters.AndFilter)
                    {
                        selected_node.Parent.Nodes.Add(new TreeNode() { Text = bf.ToString(), Tag = bf });
                        ((Pox.Filters.AndFilter)selected_node.Parent.Tag).Filters.Add(bf);
                    }
                    else if (selected_node.Parent.Tag is Pox.Filters.OrFilter)
                    {
                        selected_node.Parent.Nodes.Add(new TreeNode() { Text = bf.ToString(), Tag = bf });
                        ((Pox.Filters.OrFilter)selected_node.Parent.Tag).Filters.Add(bf);
                    }
                }
                else
                {
                    Log.Error(Log.LogSource.UI, "Form1.AddFilter(): Could not add new filter. Select a valid filter to add new filter to.");
                    return;
                }
            }

            ShowFilterProperties(bf);

            Log.Info(Log.LogSource.UI, "MainForm.AddFilter() finished");
        }

        public void RebuildFilter(BaseFilter bf)
        {
            Clear();

            SearchFilter = bf;
            if(bf == null)
                return;

            FilterTree.Nodes.Add(new TreeNode());
            AddSubNode(FilterTree.Nodes[0], bf);
        }

        private void AddSubNode(TreeNode tn, BaseFilter bf)
        {
            tn.Text = bf.ToString();
            tn.Tag = bf;

            if (bf is OrFilter)
            {
                foreach (var bff in ((OrFilter)bf).Filters)
                {
                    TreeNode tn2 = new TreeNode();
                    tn.Nodes.Add(tn2);
                    AddSubNode(tn2, bff);
                }
            }
            else if (bf is AndFilter)
            {
                foreach (var bff in ((AndFilter)bf).Filters)
                {
                    TreeNode tn2 = new TreeNode();
                    tn.Nodes.Add(tn2);
                    AddSubNode(tn2, bff); 
                }
            }
        }

        private void RemoveFilter(TreeNode f)
        {
            Log.Info(Log.LogSource.UI, "MainForm.RemoveFilter() called");

            if (f == null)
                return;
            if (f.Parent == null)
            {
                SearchFilter = null;
                FilterTree.Nodes.Clear();
            }
            else
            {
                if (f.Parent.Tag is Pox.Filters.AndFilter)
                    ((Pox.Filters.AndFilter)f.Parent.Tag).Filters.Remove((Pox.Filters.BaseFilter)f.Tag);
                else if (f.Parent.Tag is Pox.Filters.OrFilter)
                    ((Pox.Filters.OrFilter)f.Parent.Tag).Filters.Remove((Pox.Filters.BaseFilter)f.Tag);
                f.Parent.Nodes.Remove(f);
            }

            ShowFilterProperties(null);

            Log.Info(Log.LogSource.UI, "MainForm.RemoveFilter() called");
        }

        private void ShowFilterProperties(BaseFilter bf)
        {
            if (FilterProperties != null)
            {
                PanelFilterProperties.Controls.Clear();
                FilterProperties.Hide();
                FilterProperties = null;
            }
            if (bf is IntFilter)
                FilterProperties = new IntFilterControl();
            else if (bf is StringFilter)
                FilterProperties = new StringFilterControl();
            else if (bf is BooleanFilter)
                FilterProperties = new BooleanFilterControl();
            else if (bf is EnumFilter)
                FilterProperties = new EnumFilterControl();
            else if (bf is EnumListFilter)
                FilterProperties = new EnumListFilterControl();
            else if (bf is AbilityListFilter)
                FilterProperties = new AbilityListFilterControl();
            else
                FilterProperties = new BaseFilterControl();

            FilterProperties.FilterUpdate_action = FilterProperties_PropertyValueChanged;
            FilterProperties.SetFilter(bf);
            FilterProperties.Location = new Point(3, 3);

            PanelFilterProperties.Controls.Add(FilterProperties);
            FilterProperties.BringToFront();
            FilterProperties.Show();
        }

        private void FilterTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            FilterTreeContextMenu.Show(new Point(e.X + Parent.Location.X + Location.X, e.Y + Parent.Location.Y + Location.Y));
        }

        private void ButtonApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters = true;
            ButtonApplyFilter.BackColor = Color.Orange;
            ButtonClearFilter.BackColor = System.Drawing.SystemColors.Control;

            ApplyFilters_callback?.Invoke();
        }


        private void ButtonClearFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters = false;
            ButtonApplyFilter.BackColor = System.Drawing.SystemColors.Control;
            ButtonClearFilter.BackColor = Color.Orange;

            ApplyFilters_callback?.Invoke();
        }

        private void FilterProperties_PropertyValueChanged(BaseFilter bf)
        {
            // find filter on filter tree list
            if (FilterTree.Nodes.Count == 0)
                return;

            TreeNode tn = FilterTree.Nodes[0];
            int c_index = -1;
            while (true)
            {
                if (c_index >= tn.Nodes.Count)
                {
                    if (tn.Parent == null)
                        break;

                    c_index = tn.Parent.Nodes.IndexOf(tn) + 1;
                    tn = tn.Parent;

                    continue;
                }

                if (c_index != -1)
                {
                    tn = tn.Nodes[c_index];
                    c_index = -1;
                }

                if (tn.Tag == bf)
                {
                    tn.Text = bf.ToString();
                    break;
                }

                c_index += 1;
            }
        }

        private void FilterTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ShowFilterProperties((BaseFilter)e.Node.Tag);
        }

        private void matchAnyFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("Any subfilters", FilterType.OR);
        }

        private void matchAllFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("All subfilters", FilterType.AND);
        }

        private void iDToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddFilter("ID", FilterType.INT, DataPath.ID);
        }

        private void noraCostToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddFilter("Nora cost", FilterType.INT, DataPath.NoraCost);
        }

        private void deckLimitToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Deck limit", FilterType.INT, DataPath.DeckLimit);
        }

        private void prognosedBaseNoraCostToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Prognosed base nora cost", FilterType.INT, DataPath.PrognosedBaseNoraCost);
        }

        private void baseNoraCostToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Base nora cost", FilterType.INT, DataPath.BaseNoraCost);
        }

        private void defaultNoraCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("Default nora cost", FilterType.INT, DataPath.DefaultNoraCost);
        }

        private void minimumNoraCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("Minimum nora cost", FilterType.INT, DataPath.MinimumNoraCost);
        }

        private void maximumNoraCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("Maximum nora cost", FilterType.INT, DataPath.MaximumNoraCost);
        }

        private void minimumRangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Minimum range", FilterType.INT, DataPath.MinRNG);
        }

        private void maximumRangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Maximum range", FilterType.INT, DataPath.MaxRNG);
        }

        private void defenseToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddFilter("Defense", FilterType.INT, DataPath.Defense);
        }

        private void speedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Speed", FilterType.INT, DataPath.Speed);
        }

        private void damageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Damage", FilterType.INT, DataPath.Damage);
        }

        private void hitPointsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddFilter("Hit points", FilterType.INT, DataPath.HitPoints);
        }

        private void sizeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddFilter("Size", FilterType.INT, DataPath.Size);
        }

        private void aPCostToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("AP cost", FilterType.INT, DataPath.APCost);
        }

        private void levelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Level", FilterType.INT, DataPath.Level);
        }

        private void cooldownToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Cooldown", FilterType.INT, DataPath.Cooldown);
        }

        private void nameToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddFilter("Name", FilterType.STRING, DataPath.Name);
        }

        private void descriptionToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddFilter("Description", FilterType.STRING, DataPath.Description);
        }

        private void artistToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Artist", FilterType.STRING, DataPath.Artist);
        }

        private void flavorTextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Flavor text", FilterType.STRING, DataPath.FlavorText);
        }

        private void rarityToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Rarity", FilterType.RARITY);
        }

        private void expansionToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Expansion", FilterType.EXPANSION);
        }

        private void forSaleToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("For sale", FilterType.BOOLEAN, DataPath.ForSale);
        }

        private void tradeableToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Tradeable", FilterType.BOOLEAN, DataPath.Tradeable);
        }

        private void allowedInRankedToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Allowed in ranked", FilterType.BOOLEAN, DataPath.AllowRanked);
        }

        private void factionToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddFilter("Faction", FilterType.FACTION_LIST);
        }

        private void raceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Race", FilterType.RACE_LIST);
        }

        private void classToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Class", FilterType.CLASS_LIST);
        }

        private void allAbilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("All abilities", FilterType.ABILITY_LIST, DataPath.AllAbilities);
        }

        private void baseAbilitiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFilter("Base abiities", FilterType.ABILITY_LIST, DataPath.BaseAbilities);
        }

        private void upgradeAbilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter("Upgrade abilities", FilterType.ABILITY_LIST, DataPath.UpgradeAbilities);
        }

        private void removeFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveFilter(FilterTree.SelectedNode);
        }

        private void wrapIntoAnySubfiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilterTree.Nodes.Count == 0)
                return;

            TreeNode selected_node = FilterTree.SelectedNode;
            if (selected_node == null)
                return;

            OrFilter fl = new OrFilter();
            TreeNode or_node = new TreeNode() { Text = fl.ToString(), Tag = fl };
            // check if the node is the root
            if (selected_node.Parent == null)
            {
                FilterTree.Nodes.Clear();
                FilterTree.Nodes.Add(or_node);
                SearchFilter = fl;
            }
            else
            {
                TreeNode pt = selected_node.Parent;
                pt.Nodes.Insert(pt.Nodes.IndexOf(selected_node), or_node);
                pt.Nodes.Remove(selected_node);

                if (pt.Tag is OrFilter)
                {
                    ((OrFilter)pt.Tag).Filters.Add(fl);
                    ((OrFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);
                }
                else if (pt.Tag is AndFilter)
                {
                    ((AndFilter)pt.Tag).Filters.Add(fl);
                    ((AndFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);
                }
            }
            fl.Filters.Add((BaseFilter)selected_node.Tag);
            or_node.Nodes.Add(selected_node);
            FilterTree.SelectedNode = selected_node;

            ShowFilterProperties((BaseFilter)selected_node.Tag);
        }

        private void wrapIntoAllSubfiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilterTree.Nodes.Count == 0)
                return;

            TreeNode selected_node = FilterTree.SelectedNode;
            if (selected_node == null)
                return;

            AndFilter fl = new AndFilter();
            TreeNode and_node = new TreeNode() { Text = fl.ToString(), Tag = fl };
            // check if the node is the root
            if (selected_node.Parent == null)
            {
                FilterTree.Nodes.Clear();
                FilterTree.Nodes.Add(and_node);
                SearchFilter = fl;
            }
            else
            {
                TreeNode pt = selected_node.Parent;
                pt.Nodes.Insert(pt.Nodes.IndexOf(selected_node), and_node);
                pt.Nodes.Remove(selected_node);

                if (pt.Tag is OrFilter)
                {
                    ((OrFilter)pt.Tag).Filters.Add(fl);
                    ((OrFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);
                }
                else if (pt.Tag is AndFilter)
                {
                    ((AndFilter)pt.Tag).Filters.Add(fl);
                    ((AndFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);
                }
            }
            fl.Filters.Add((BaseFilter)selected_node.Tag);
            and_node.Nodes.Add(selected_node);
            FilterTree.SelectedNode = selected_node;

            ShowFilterProperties((BaseFilter)selected_node.Tag);
        }

        private void popOutOfAnyAllSubfiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilterTree.Nodes.Count == 0)
                return;

            TreeNode selected_node = FilterTree.SelectedNode;
            if (selected_node == null)
                return;

            TreeNode pt = selected_node.Parent;
            if (pt == null)
                return;

            TreeNode pt2 = pt.Parent;
            // if pt is top of hierarchy
            if (pt2 == null)
            {
                if ((pt.Tag is AndFilter) || (pt.Tag is OrFilter))
                {
                    if (pt.Nodes.Count == 1)
                    {
                        FilterTree.Nodes.Clear();
                        FilterTree.Nodes.Add(selected_node);
                        SearchFilter = (BaseFilter)selected_node.Tag;
                    }
                }
            }
            else
            {
                if ((pt.Tag is AndFilter) || (pt.Tag is OrFilter))
                {
                    pt.Nodes.Remove(selected_node);
                    if (pt.Tag is AndFilter)
                        ((AndFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);
                    else if (pt.Tag is OrFilter)
                        ((OrFilter)pt.Tag).Filters.Remove((BaseFilter)selected_node.Tag);

                    pt2.Nodes.Insert(pt2.Nodes.IndexOf(pt), selected_node);
                    if (pt2.Tag is AndFilter)
                        ((AndFilter)pt2.Tag).Filters.Insert(((AndFilter)pt2.Tag).Filters.IndexOf((BaseFilter)pt.Tag), (BaseFilter)selected_node.Tag);
                    else if (pt2.Tag is OrFilter)
                        ((OrFilter)pt2.Tag).Filters.Insert(((OrFilter)pt2.Tag).Filters.IndexOf((BaseFilter)pt.Tag), (BaseFilter)selected_node.Tag);

                    if (pt.Nodes.Count == 0)
                    {
                        pt2.Nodes.Remove(pt);
                        if (pt2.Tag is AndFilter)
                            ((AndFilter)pt2.Tag).Filters.Remove((BaseFilter)pt.Tag);
                        else if (pt2.Tag is OrFilter)
                            ((OrFilter)pt2.Tag).Filters.Remove((BaseFilter)pt.Tag);
                    }
                }
            }

            ShowFilterProperties((BaseFilter)selected_node.Tag);
        }

        private void quickFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickFilterForm qff = new QuickFilterForm();
            if (qff.ShowDialog() != DialogResult.OK)
                return;


            // remove current filter
            if (FilterTree.Nodes.Count != 0)
                RemoveFilter(FilterTree.Nodes[0]);

            // add all subfilter
            AddFilter("All subfilters", FilterType.AND);

            int cur_subf = 0;
            // add faction filter
            FilterTree.SelectedNode = FilterTree.Nodes[0];
            if ((!qff.NegateFactions)||(qff.AllowedFactions.Count != 0))
            {
                AddFilter("Any subfilters", FilterType.OR);
                FilterTree.SelectedNode = FilterTree.Nodes[0].Nodes[cur_subf];
                ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).NegateResult = qff.NegateFactions;
                FilterTree.SelectedNode.Text = ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).ToString();

                for (int i = 0; i < qff.AllowedFactions.Count; i++)
                {
                    AddFilter("Faction", FilterType.FACTION_LIST);
                    ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).RefValue = qff.AllowedFactions[i];
                    FilterTree.SelectedNode.Nodes[i].Text = ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).ToString();
                }

                cur_subf += 1;
            }

            // add class filter
            FilterTree.SelectedNode = FilterTree.Nodes[0];
            if ((!qff.NegateClasses) || (qff.AllowedClasses.Count != 0))
            {
                AddFilter("Any subfilters", FilterType.OR);
                FilterTree.SelectedNode = FilterTree.Nodes[0].Nodes[cur_subf];
                ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).NegateResult = qff.NegateClasses;
                FilterTree.SelectedNode.Text = ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).ToString();

                for (int i = 0; i < qff.AllowedClasses.Count; i++)
                {
                    AddFilter("Class", FilterType.CLASS_LIST);
                    ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).RefValue = qff.AllowedClasses[i];
                    FilterTree.SelectedNode.Nodes[i].Text = ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).ToString();
                }

                cur_subf += 1;
            }

            // add race filter
            FilterTree.SelectedNode = FilterTree.Nodes[0];
            if ((!qff.NegateRaces) || (qff.AllowedRaces.Count != 0))
            {
                AddFilter("Any subfilters", FilterType.OR);
                FilterTree.SelectedNode = FilterTree.Nodes[0].Nodes[cur_subf];
                ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).NegateResult = qff.NegateRaces;
                FilterTree.SelectedNode.Text = ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).ToString();

                for (int i = 0; i < qff.AllowedRaces.Count; i++)
                {
                    AddFilter("Race", FilterType.RACE_LIST);
                    ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).RefValue = qff.AllowedRaces[i];
                    FilterTree.SelectedNode.Nodes[i].Text = ((Pox.Filters.EnumListFilter)FilterTree.SelectedNode.Nodes[i].Tag).ToString();
                }

                cur_subf += 1;
            }

            // add rarity filter
            FilterTree.SelectedNode = FilterTree.Nodes[0];
            if ((!qff.NegateRarities) || (qff.AllowedRarities.Count != 0))
            {
                AddFilter("Any subfilters", FilterType.OR);
                FilterTree.SelectedNode = FilterTree.Nodes[0].Nodes[cur_subf];
                ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).NegateResult = qff.NegateRarities;
                FilterTree.SelectedNode.Text = ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).ToString();

                for (int i = 0; i < qff.AllowedRarities.Count; i++)
                {
                    AddFilter("Rarity", FilterType.RARITY);
                    ((Pox.Filters.EnumFilter)FilterTree.SelectedNode.Nodes[i].Tag).RefValue = qff.AllowedRarities[i];
                    FilterTree.SelectedNode.Nodes[i].Text = ((Pox.Filters.EnumFilter)FilterTree.SelectedNode.Nodes[i].Tag).ToString();
                }

                cur_subf += 1;
            }

            // add expansion filter
            FilterTree.SelectedNode = FilterTree.Nodes[0];
            if ((!qff.NegateExpansions) || (qff.AllowedExpansions.Count != 0))
            {
                AddFilter("Any subfilters", FilterType.OR);
                FilterTree.SelectedNode = FilterTree.Nodes[0].Nodes[cur_subf];
                ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).NegateResult = qff.NegateExpansions;
                FilterTree.SelectedNode.Text = ((Pox.Filters.OrFilter)FilterTree.SelectedNode.Tag).ToString();

                for (int i = 0; i < qff.AllowedExpansions.Count; i++)
                {
                    AddFilter("Expansion", FilterType.EXPANSION);
                    ((Pox.Filters.EnumFilter)FilterTree.SelectedNode.Nodes[i].Tag).RefValue = qff.AllowedExpansions[i];
                    FilterTree.SelectedNode.Nodes[i].Text = ((Pox.Filters.EnumFilter)FilterTree.SelectedNode.Nodes[i].Tag).ToString();
                }

                cur_subf += 1;
            }

            ButtonApplyFilter_Click(null, null);
        }
    }
}
