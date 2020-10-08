using poxnora_search_engine.Pox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Management.Instrumentation;
using poxnora_search_engine.Pox.Filters;
using poxnora_search_engine.Pox.FilterControls;
using System.Security.Cryptography;

namespace poxnora_search_engine
{
    public enum FilterType { AND, OR, BOOLEAN, INT, STRING, RARITY, EXPANSION, ABILITY_LIST, CLASS_LIST, FACTION_LIST, RACE_LIST }
    public partial class MainForm : Form
    {
        Pox.DataElement.ElementType ViewType = Pox.DataElement.ElementType.CHAMPION;
        Pox.Filters.BaseFilter SearchFilter = null;
        bool ApplyFilters = false;

        Updater app_updater = new Updater();

        CardRandomizer CardRandomizer_form = null;
        ChampionBuilder ChampionBuilder_form = null;
        DifferenceCalculator DifferenceCalculator_form = null;

        BaseFilterControl FilterProperties = null;
        public MainForm()
        {
            InitializeComponent();
            Log.OnLog = ShowLogMessageOnStatusBar;
        }

        private void ShowLogMessageOnStatusBar(string s)
        {
            LastLogMessage.Text = s;
        }

        void OnFetchCurrentVersion(bool is_current_outdated, string new_version)
        {
            if (is_current_outdated)
            {
                StatusNewVersionAvailable.Visible = true;
            }
        }

        void OnFetchCurrentArchiveFailed()
        {
            Log.Error("MainForm.OnFetchCurrentArchiveFailed(): Could not install latest version");

            StatusNewVersionAvailable.IsLink = true;
            StatusNewVersionAvailable.Text = "New version available";
        }

        void OnFetchCurrentArchiveSuccess()
        {
            string dir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.Diagnostics.Process.Start(dir+"\\onupdate.bat");
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.database.ready_trigger = OnDatabaseReady;
            Program.database.LoadJSON("database.json", Pox.Database.POXNORA_JSON_SITE);

            foreach (ToolStripMenuItem column_vis in championsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnChampionColumnVisibilityClick);
            foreach (ToolStripMenuItem column_vis in abilitiesToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnAbilityColumnVisibilityClick);
            foreach (ToolStripMenuItem column_vis in spellsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnSpellColumnVisibilityClick);
            foreach (ToolStripMenuItem column_vis in relicsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnRelicColumnVisibilityClick);
            foreach (ToolStripMenuItem column_vis in equipmentsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnEquipmentColumnVisibilityClick);

            // this makes datagridview performance bearable :)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                GridDataElements,
                new object[] { true });

            Program.image_cache.Subscribers.Add(RuneDescription);


            app_updater._OnGetVersion = OnFetchCurrentVersion;
            app_updater._OnGetArchiveFailed = OnFetchCurrentArchiveFailed;
            app_updater._OnGetArchiveSuccess = OnFetchCurrentArchiveSuccess;
            app_updater.GetLatestVersion();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.image_cache.Subscribers.Remove(RuneDescription);
        }

        private void OnDatabaseReady()
        {
            RuneDescription.database_ref = Program.database;
            PrepareGrid();
        }

        public void PrepareGrid()
        {
            GridDataElements.Hide();
            GridDataElements.Rows.Clear();
            GridDataElements.Columns.Clear();

            switch (ViewType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "ID", Name = "ID", ValueType = typeof(int), Visible = iDToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Nora cost", Name = "NoraCost", ValueType = typeof(int), Visible = noraCostToolStripMenuItem.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Rarity", Name = "Rarity", ValueType = typeof(string), Visible = rarityToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Artist", Name = "Artist", ValueType = typeof(string), Visible = artistToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Faction", Name = "Faction", ValueType = typeof(string), Visible = factionToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Expansion", Name = "Expansion", ValueType = typeof(string), Visible = expansionToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "For sale", Name = "ForSale", ValueType = typeof(bool), Visible = forSaleToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Tradeable", Name = "Tradeable", ValueType = typeof(bool), Visible = tradeableToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Allowed in ranked", Name = "AllowRanked", ValueType = typeof(bool), Visible = allowedInRankedToolStripMenuItem.Visible });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Deck limit", Name = "DeckLimit", ValueType = typeof(int), Visible = deckLimitToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Cooldown", Name = "Cooldown", ValueType = typeof(int), Visible = cooldownToolStripMenuItem2.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Prognosed base nora cost", Name = "PrognosedBaseNoraCost", ValueType = typeof(int), Visible = prognosedBaseNoraCostToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Base nora cost", Name = "BaseNoraCost", ValueType = typeof(int), Visible = baseNoraCostToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Prognosed base nora cost difference", Name = "PrognosedBaseNoraCostDifference", ValueType = typeof(int), Visible = prognosedBaseNoraCostDifferenceToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Default nora cost", Name = "DefaultNoraCost", ValueType = typeof(int), Visible = defaultNoraCostToolStripMenuItem1.Checked});
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Minimum nora cost", Name = "MinNoraCost", ValueType = typeof(int), Visible = minimumNoraCostToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Maximum nora cost", Name = "MaxNoraCost", ValueType = typeof(int), Visible = maximumNoraCostToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Minimum range", Name = "MinRNG", ValueType = typeof(int), Visible = minimumRangeToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Maximum range", Name = "MaxRNG", ValueType = typeof(int), Visible = maximumRangeToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Defense", Name = "Defense", ValueType = typeof(int), Visible = defenseToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Speed", Name = "Speed", ValueType = typeof(int), Visible = speedToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Damage", Name = "Damage", ValueType = typeof(int), Visible = damageToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Hit points", Name = "HitPoints", ValueType = typeof(int), Visible = hitPointsToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Size", Name = "Size", ValueType = typeof(int), Visible = sizeToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Class", Name = "Class", ValueType = typeof(string), Visible = classToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Race", Name = "Race", ValueType = typeof(string), Visible = raceToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Base abilities", Name = "BaseAbilities", ValueType = typeof(string), Visible = baseAbilitiesToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Upgrade 1", Name = "Upgrade1", ValueType = typeof(string), Visible = upgrade1ToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Upgrade 2", Name = "Upgrade2", ValueType = typeof(string), Visible = upgrade2ToolStripMenuItem.Checked });

                        break;
                    }
                case Pox.DataElement.ElementType.ABILITY:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "ID", Name = "ID", ValueType = typeof(int), Visible = iDToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Nora cost", Name = "NoraCost", ValueType = typeof(int), Visible = noraCostToolStripMenuItem1.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "AP cost", Name = "APCost", ValueType = typeof(string), Visible = aPCostToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Level", Name = "Level", ValueType = typeof(string), Visible = levelToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Cooldown", Name = "Cooldown", ValueType = typeof(string), Visible = cooldownToolStripMenuItem.Checked });

                        break;
                    }
                case Pox.DataElement.ElementType.SPELL:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "ID", Name = "ID", ValueType = typeof(int), Visible = iDToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Nora cost", Name = "NoraCost", ValueType = typeof(int), Visible = noraCostToolStripMenuItem2.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Rarity", Name = "Rarity", ValueType = typeof(string), Visible = rarityToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Artist", Name = "Artist", ValueType = typeof(string), Visible = artistToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Faction", Name = "Faction", ValueType = typeof(string), Visible = factionToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Expansion", Name = "Expansion", ValueType = typeof(string), Visible = expansionToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "For sale", Name = "ForSale", ValueType = typeof(bool), Visible = forSaleToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Tradeable", Name = "Tradeable", ValueType = typeof(bool), Visible = tradeableToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Allowed in ranked", Name = "AllowRanked", ValueType = typeof(bool), Visible = allowedInRankedToolStripMenuItem1.Visible });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Deck limit", Name = "DeckLimit", ValueType = typeof(int), Visible = deckLimitToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Cooldown", Name = "Cooldown", ValueType = typeof(int), Visible = cooldownToolStripMenuItem3.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Flavor text", Name = "FlavorText", ValueType = typeof(string), Visible = flavorToolStripMenuItem.Checked });
                        break;
                    }
                case Pox.DataElement.ElementType.RELIC:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "ID", Name = "ID", ValueType = typeof(int), Visible = iDToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Nora cost", Name = "NoraCost", ValueType = typeof(int), Visible = noraCostToolStripMenuItem3.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Rarity", Name = "Rarity", ValueType = typeof(string), Visible = rarityToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Artist", Name = "Artist", ValueType = typeof(string), Visible = artistToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Faction", Name = "Faction", ValueType = typeof(string), Visible = factionToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Expansion", Name = "Expansion", ValueType = typeof(string), Visible = expansionToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "For sale", Name = "ForSale", ValueType = typeof(bool), Visible = forSaleToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Tradeable", Name = "Tradeable", ValueType = typeof(bool), Visible = tradeableToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Allowed in ranked", Name = "AllowRanked", ValueType = typeof(bool), Visible = allowedInRankedToolStripMenuItem2.Visible });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Deck limit", Name = "DeckLimit", ValueType = typeof(int), Visible = deckLimitToolStripMenuItem2.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Cooldown", Name = "Cooldown", ValueType = typeof(int), Visible = cooldownToolStripMenuItem4.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Flavor text", Name = "FlavorText", ValueType = typeof(string), Visible = flavorToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Defense", Name = "Defense", ValueType = typeof(int), Visible = defenseToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Hit points", Name = "HitPoints", ValueType = typeof(int), Visible = hitPointsToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Size", Name = "Size", ValueType = typeof(int), Visible = sizeToolStripMenuItem1.Checked });
                        break;
                    }
                case Pox.DataElement.ElementType.EQUIPMENT:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "ID", Name = "ID", ValueType = typeof(int), Visible = iDToolStripMenuItem4.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem4.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem4.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Nora cost", Name = "NoraCost", ValueType = typeof(int), Visible = noraCostToolStripMenuItem4.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Rarity", Name = "Rarity", ValueType = typeof(string), Visible = rarityToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Artist", Name = "Artist", ValueType = typeof(string), Visible = artistToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Faction", Name = "Faction", ValueType = typeof(string), Visible = factionToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Expansion", Name = "Expansion", ValueType = typeof(string), Visible = expansionToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "For sale", Name = "ForSale", ValueType = typeof(bool), Visible = forSaleToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Tradeable", Name = "Tradeable", ValueType = typeof(bool), Visible = tradeableToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Allowed in ranked", Name = "AllowRanked", ValueType = typeof(bool), Visible = allowedInRankedToolStripMenuItem3.Visible });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Deck limit", Name = "DeckLimit", ValueType = typeof(int), Visible = deckLimitToolStripMenuItem3.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Cooldown", Name = "Cooldown", ValueType = typeof(int), Visible = cooldownToolStripMenuItem5.Checked });

                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Flavor text", Name = "FlavorText", ValueType = typeof(string), Visible = flavorTextToolStripMenuItem.Checked });
                        break;
                    }
                default:
                    break;
            }

            foreach (DataGridViewColumn column in GridDataElements.Columns)
            {
                column.CellTemplate = new DataGridViewTextBoxCell();
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            ReloadGrid();
            ApplyFilter();
            GridDataElements.Show();
        }

        void ReloadGrid()
        {

            switch (ViewType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    {
                        List<Champion> champs = new List<Champion>();
                        foreach (var kv in Program.database.Champions)
                            champs.Add(kv.Value);

                        foreach (var c in champs)
                            AddChampion(c);

                        break;
                    }
                case Pox.DataElement.ElementType.ABILITY:
                    {
                        List<Ability> abs = new List<Ability>();
                        foreach (var kv in Program.database.Abilities)
                            abs.Add(kv.Value);

                        foreach (var a in abs)
                            AddAbility(a);

                        break;
                    }
                case Pox.DataElement.ElementType.SPELL:
                    {
                        List<Spell> sps = new List<Spell>();
                        foreach (var kv in Program.database.Spells)
                            sps.Add(kv.Value);

                        foreach (var s in sps)
                            AddSpell(s);

                        break;
                    }
                case Pox.DataElement.ElementType.RELIC:
                    {
                        List<Relic> rls = new List<Relic>();
                        foreach (var kv in Program.database.Relics)
                            rls.Add(kv.Value);

                        foreach (var r in rls)
                            AddRelic(r);

                        break;
                    }
                case Pox.DataElement.ElementType.EQUIPMENT:
                    {
                        List<Equipment> eqs = new List<Equipment>();
                        foreach (var kv in Program.database.Equipments)
                            eqs.Add(kv.Value);

                        foreach (var e in eqs)
                            AddEquipment(e);

                        break;
                    }
                default:
                    break;
            }
        }

        public void AddChampion(Pox.Champion c)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = c;

            GridDataElements.Rows[row].Cells["ID"].Value = c.ID;
            GridDataElements.Rows[row].Cells["Name"].Value = c.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = c.Description;
            GridDataElements.Rows[row].Cells["NoraCost"].Value = c.NoraCost;

            GridDataElements.Rows[row].Cells["Rarity"].Value = c.Rarity;
            GridDataElements.Rows[row].Cells["Artist"].Value = c.Artist;

            string faction = "";
            for (int i = 0; i < c.Faction.Count - 1; i++)
                faction += c.Faction[i] + ", ";
            if (c.Faction.Count != 0)
                faction += c.Faction[c.Faction.Count - 1];
            GridDataElements.Rows[row].Cells["Faction"].Value = faction;

            GridDataElements.Rows[row].Cells["Expansion"].Value = c.Expansion;
            GridDataElements.Rows[row].Cells["ForSale"].Value = c.ForSale;
            GridDataElements.Rows[row].Cells["Tradeable"].Value = c.Tradeable;
            GridDataElements.Rows[row].Cells["AllowRanked"].Value = c.AllowRanked;
            GridDataElements.Rows[row].Cells["DeckLimit"].Value = c.DeckLimit;
            GridDataElements.Rows[row].Cells["Cooldown"].Value = c.Cooldown;

            GridDataElements.Rows[row].Cells["PrognosedBaseNoraCost"].Value = c.PrognosedBaseNoraCost;
            GridDataElements.Rows[row].Cells["BaseNoraCost"].Value = c.BaseNoraCost; 
            GridDataElements.Rows[row].Cells["PrognosedBaseNoraCostDifference"].Value = c.PrognosedBaseNoraCostDifference;
            GridDataElements.Rows[row].Cells["DefaultNoraCost"].Value = c.DefaultNoraCost;
            GridDataElements.Rows[row].Cells["MinNoraCost"].Value = c.MinNoraCost;
            GridDataElements.Rows[row].Cells["MaxNoraCost"].Value = c.MaxNoraCost;
            GridDataElements.Rows[row].Cells["MinRNG"].Value = c.MinRNG;
            GridDataElements.Rows[row].Cells["MaxRNG"].Value = c.MaxRNG;
            GridDataElements.Rows[row].Cells["Defense"].Value = c.Defense;
            GridDataElements.Rows[row].Cells["Speed"].Value = c.Speed;
            GridDataElements.Rows[row].Cells["Damage"].Value = c.Damage;
            GridDataElements.Rows[row].Cells["HitPoints"].Value = c.HitPoints;
            GridDataElements.Rows[row].Cells["Size"].Value = c.Size;

            string cl = "";
            for (int i = 0; i < c.Class.Count - 1; i++)
                cl += c.Class[i] + ", ";
            if (c.Class.Count != 0)
                cl += c.Class[c.Class.Count - 1];
            GridDataElements.Rows[row].Cells["Class"].Value = cl;

            string race= "";
            for (int i = 0; i < c.Race.Count - 1; i++)
                race += c.Race[i] + ", ";
            if (c.Race.Count != 0)
                race += c.Race[c.Race.Count - 1];
            GridDataElements.Rows[row].Cells["Race"].Value = race;

            string bases = "";
            for (int i = 0; i < c.BaseAbilities_refs.Count - 1; i++)
                bases += c.BaseAbilities_refs[i].ToString() + ", ";
            if (c.BaseAbilities_refs.Count != 0)
                bases += c.BaseAbilities_refs[c.BaseAbilities_refs.Count - 1].ToString();
            GridDataElements.Rows[row].Cells["BaseAbilities"].Value = bases;

            string upg1 = "";
            for (int i = 0; i < c.UpgradeAbilities1_refs.Count - 1; i++)
                upg1 += c.UpgradeAbilities1_refs[i].ToString() + ", ";
            if (c.UpgradeAbilities1_refs.Count != 0)
                upg1 += c.UpgradeAbilities1_refs[c.UpgradeAbilities1_refs.Count - 1].ToString();
            GridDataElements.Rows[row].Cells["Upgrade1"].Value = upg1;

            string upg2 = "";
            for (int i = 0; i < c.UpgradeAbilities2_refs.Count - 1; i++)
                upg2 += c.UpgradeAbilities2_refs[i].ToString() + ", ";
            if (c.UpgradeAbilities2_refs.Count != 0)
                upg2 += c.UpgradeAbilities2_refs[c.UpgradeAbilities2_refs.Count - 1].ToString();
            GridDataElements.Rows[row].Cells["Upgrade2"].Value = upg2;
        }


        public void AddAbility(Pox.Ability a)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = a;

            GridDataElements.Rows[row].Cells["ID"].Value = a.ID;
            GridDataElements.Rows[row].Cells["Name"].Value = a.ToString();
            GridDataElements.Rows[row].Cells["Description"].Value = a.Description;
            GridDataElements.Rows[row].Cells["NoraCost"].Value = a.NoraCost;

            GridDataElements.Rows[row].Cells["APCost"].Value = a.APCost;
            GridDataElements.Rows[row].Cells["Level"].Value = a.Level;
            GridDataElements.Rows[row].Cells["Cooldown"].Value = a.Cooldown;
        }

        public void AddSpell(Pox.Spell s)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = s;

            GridDataElements.Rows[row].Cells["ID"].Value = s.ID;
            GridDataElements.Rows[row].Cells["Name"].Value = s.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = s.Description;
            GridDataElements.Rows[row].Cells["NoraCost"].Value = s.NoraCost;

            GridDataElements.Rows[row].Cells["Rarity"].Value = s.Rarity;
            GridDataElements.Rows[row].Cells["Artist"].Value = s.Artist;

            string faction = "";
            for (int i = 0; i < s.Faction.Count - 1; i++)
                faction += s.Faction[i] + ", ";
            if (s.Faction.Count != 0)
                faction += s.Faction[s.Faction.Count - 1];
            GridDataElements.Rows[row].Cells["Faction"].Value = faction;

            GridDataElements.Rows[row].Cells["Expansion"].Value = s.Expansion;
            GridDataElements.Rows[row].Cells["ForSale"].Value = s.ForSale;
            GridDataElements.Rows[row].Cells["Tradeable"].Value = s.Tradeable;
            GridDataElements.Rows[row].Cells["AllowRanked"].Value = s.AllowRanked;
            GridDataElements.Rows[row].Cells["DeckLimit"].Value = s.DeckLimit;
            GridDataElements.Rows[row].Cells["Cooldown"].Value = s.Cooldown;

            GridDataElements.Rows[row].Cells["FlavorText"].Value = s.Flavor;
        }

        public void AddRelic(Pox.Relic r)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = r;

            GridDataElements.Rows[row].Cells["ID"].Value = r.ID;
            GridDataElements.Rows[row].Cells["Name"].Value = r.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = r.Description;
            GridDataElements.Rows[row].Cells["NoraCost"].Value = r.NoraCost;

            GridDataElements.Rows[row].Cells["Rarity"].Value = r.Rarity;
            GridDataElements.Rows[row].Cells["Artist"].Value = r.Artist;

            string faction = "";
            for (int i = 0; i < r.Faction.Count - 1; i++)
                faction += r.Faction[i] + ", ";
            if (r.Faction.Count != 0)
                faction += r.Faction[r.Faction.Count - 1];
            GridDataElements.Rows[row].Cells["Faction"].Value = faction;

            GridDataElements.Rows[row].Cells["Expansion"].Value = r.Expansion;
            GridDataElements.Rows[row].Cells["ForSale"].Value = r.ForSale;
            GridDataElements.Rows[row].Cells["Tradeable"].Value = r.Tradeable;
            GridDataElements.Rows[row].Cells["AllowRanked"].Value = r.AllowRanked;
            GridDataElements.Rows[row].Cells["DeckLimit"].Value = r.DeckLimit;
            GridDataElements.Rows[row].Cells["Cooldown"].Value = r.Cooldown;

            GridDataElements.Rows[row].Cells["FlavorText"].Value = r.Flavor;
            GridDataElements.Rows[row].Cells["Defense"].Value = r.Defense;
            GridDataElements.Rows[row].Cells["HitPoints"].Value = r.HitPoints;
            GridDataElements.Rows[row].Cells["Size"].Value = r.Size;
        }

        public void AddEquipment(Pox.Equipment e)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = e;

            GridDataElements.Rows[row].Cells["ID"].Value = e.ID;
            GridDataElements.Rows[row].Cells["Name"].Value = e.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = e.Description;
            GridDataElements.Rows[row].Cells["NoraCost"].Value = e.NoraCost;

            GridDataElements.Rows[row].Cells["Rarity"].Value = e.Rarity;
            GridDataElements.Rows[row].Cells["Artist"].Value = e.Artist;

            string faction = "";
            for (int i = 0; i < e.Faction.Count - 1; i++)
                faction += e.Faction[i] + ", ";
            if (e.Faction.Count != 0)
                faction += e.Faction[e.Faction.Count - 1];
            GridDataElements.Rows[row].Cells["Faction"].Value = faction;

            GridDataElements.Rows[row].Cells["Expansion"].Value = e.Expansion;
            GridDataElements.Rows[row].Cells["ForSale"].Value = e.ForSale;
            GridDataElements.Rows[row].Cells["Tradeable"].Value = e.Tradeable;
            GridDataElements.Rows[row].Cells["AllowRanked"].Value = e.AllowRanked;
            GridDataElements.Rows[row].Cells["DeckLimit"].Value = e.DeckLimit;
            GridDataElements.Rows[row].Cells["Cooldown"].Value = e.Cooldown;

            GridDataElements.Rows[row].Cells["FlavorText"].Value = e.Flavor;
        }

        private void ApplyFilter()
        {
            if((SearchFilter == null)||(!ApplyFilters))
            {
                foreach (DataGridViewRow row in GridDataElements.Rows)
                    row.Visible = true;
                Log.Info("Items on grid: " + GridDataElements.Rows.Count.ToString());
                return;
            }

            int total = 0;
            foreach (DataGridViewRow row in GridDataElements.Rows)
            {
                row.Visible = SearchFilter.Satisfies((DataElement)row.Tag);
                if (row.Visible)
                    total += 1;
            }
            Log.Info("Items on grid: " + total.ToString());
        }

        private void AddFilter(string fname, FilterType ftype, DataPath dpath = DataPath.None)
        {
            Pox.Filters.BaseFilter bf;

            switch(ftype)
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
                    Log.Error("Form1.AddFilter(): Unknown filter type");
                    throw new Exception("Form1.AddFilter(): Error while adding filter");
            }
            bf.Name = fname;

            // determine which filter is selected right now
            if(FilterTree.Nodes.Count == 0)
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
                    Log.Error("Form1.AddFilter(): Could not add new filter. Select a valid filter to add new filter to.");
                    return;
                }
            }

            ShowFilterProperties(bf);
        }

        private void RemoveFilter(TreeNode f)
        {
            if (f == null)
                return;
            if(f.Parent == null)
            {
                SearchFilter = null;
                FilterTree.Nodes.Clear();
            }
            else
            {
                if (f.Parent.Tag is Pox.Filters.AndFilter)
                    ((Pox.Filters.AndFilter)f.Parent.Tag).Filters.Remove((Pox.Filters.BaseFilter)f.Tag);
                else if(f.Parent.Tag is Pox.Filters.OrFilter)
                    ((Pox.Filters.OrFilter)f.Parent.Tag).Filters.Remove((Pox.Filters.BaseFilter)f.Tag);
                f.Parent.Nodes.Remove(f);
            }

            ShowFilterProperties(null);
        }

        private void ShowFilterProperties(BaseFilter bf)
        {
            if(FilterProperties != null)
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


        private void RadioChampions_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.CHAMPION;
            PrepareGrid();
        }

        private void RadioAbilities_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.ABILITY;
            PrepareGrid();
        }

        private void RadioSpells_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.SPELL;
            PrepareGrid();
        }

        private void RadioRelics_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.RELIC;
            PrepareGrid();
        }

        private void RadioEquips_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.EQUIPMENT;
            PrepareGrid();
        }

        // FILTERS

        private void FilterTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            FilterTreeContextMenu.Show(new Point(e.X+Location.X, e.Y+Location.Y));
        }

        private void ButtonApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters = true;
            ButtonApplyFilter.BackColor = Color.Orange;
            ButtonClearFilter.BackColor = System.Drawing.SystemColors.Control;
            GridDataElements.Hide();
            ApplyFilter();
            GridDataElements.Show();
        }


        private void ButtonClearFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters = false;
            ButtonApplyFilter.BackColor = System.Drawing.SystemColors.Control;
            ButtonClearFilter.BackColor = Color.Orange;
            GridDataElements.Hide();
            ApplyFilter();
            GridDataElements.Show();
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

                if(c_index != -1)
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
            if(selected_node.Parent == null)
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
            if(pt2 == null)
            {
                if((pt.Tag is AndFilter) || (pt.Tag is OrFilter))
                {
                    if(pt.Nodes.Count == 1)
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
                    else if(pt2.Tag is OrFilter)
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

        private void GridDataElements_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            if (GridDataElements.Rows[row].Cells["ID"].Value == null)
                return;

            int id = (int)GridDataElements.Rows[row].Cells["ID"].Value;

            RuneDescription.TracerClear();
            switch (ViewType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    RuneDescription.SetChampionRune(Program.database.Champions[id]);
                    if (ChampionBuilder_form != null)
                        ChampionBuilder_form.external_SetChampionTemplate(Program.database.Champions[id].Name);
                    break;
                case Pox.DataElement.ElementType.ABILITY:
                    RuneDescription.SetAbility(Program.database.Abilities[id]);
                    break;
                case Pox.DataElement.ElementType.SPELL:
                    RuneDescription.SetSpellRune(Program.database.Spells[id]);
                    break;
                case Pox.DataElement.ElementType.RELIC:
                    RuneDescription.SetRelicRune(Program.database.Relics[id]);
                    break;
                case Pox.DataElement.ElementType.EQUIPMENT:
                    RuneDescription.SetEquipmentRune(Program.database.Equipments[id]);
                    break;
                default:
                    RuneDescription.ClearDescription();
                    break;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.Width <= 1410 - 764)
                return;
            if (this.Height <= 686 - 565)
                return;

            GridDataElements.Width = this.Width - 1410 + 764;
            GridDataElements.Height = this.Height - 682 + 565;
            RuneDescription.Location = new Point(GridDataElements.Location.X + GridDataElements.Width + 3, FilterTree.Location.Y);
            RuneDescription.SetHeight(GridDataElements.Height + 32);
        }


        // grid columns


        private void OnChampionColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.CHAMPION)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }
        private void OnAbilityColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.ABILITY)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }
        private void OnSpellColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.SPELL)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }
        private void OnRelicColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.RELIC)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }
        private void OnEquipmentColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.EQUIPMENT)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about_box = new AboutBox();
            about_box.ShowDialog();
        }

        private void deckRandomizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.database.ready)
                return;

            CardRandomizer_form = new CardRandomizer();
            CardRandomizer_form.ShowDialog();
            CardRandomizer_form = null;
        }

        private void championBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.database.ready)
                return;

            if(ChampionBuilder_form != null)
            {
                ChampionBuilder_form.BringToFront();
                return;
            }

            ChampionBuilder_form = new ChampionBuilder();
            ChampionBuilder_form.FormClosed += new FormClosedEventHandler(ChampionBuilder_form_FormClosed);

            ChampionBuilder_form.Show();
        }

        private void ChampionBuilder_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChampionBuilder_form.FormClosed -= new FormClosedEventHandler(ChampionBuilder_form_FormClosed);
            ChampionBuilder_form = null;
        }

        public void external_SetRuneDescriptionAbility(Ability a)
        {
            if (a == null)
                return;

            RuneDescription.TracerClear();
            RuneDescription.SetAbility(a);
        }

        private void differenceCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.database.ready)
                return;

            if (DifferenceCalculator_form != null)
            {
                DifferenceCalculator_form.BringToFront();
                return;
            }

            DifferenceCalculator_form = new DifferenceCalculator();
            DifferenceCalculator_form.FormClosed += new FormClosedEventHandler(DifferenceCalculator_form_FormClosed);

            DifferenceCalculator_form.Show();
        }

        private void DifferenceCalculator_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            DifferenceCalculator_form.FormClosed -= new FormClosedEventHandler(DifferenceCalculator_form_FormClosed);
            DifferenceCalculator_form = null;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            Program.image_cache.Subscribers.Add(RuneDescription);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            Program.image_cache.Subscribers.Remove(RuneDescription);
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("manual\\index.html");
        }

        private void StatusNewVersionAvailable_Click(object sender, EventArgs e)
        {
            if (!StatusNewVersionAvailable.Visible)
                return;
            if (!StatusNewVersionAvailable.IsLink)
                return;

            StatusNewVersionAvailable.IsLink = false;
            StatusNewVersionAvailable.Text = "Downloading...";

            app_updater.ForceInstallLatestVersion();
        }
    }
}
