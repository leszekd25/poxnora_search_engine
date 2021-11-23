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
using poxnora_search_engine.Pox;
using poxnora_search_engine.Pox.Filters;
using poxnora_search_engine.Pox.FilterControls;
using System.Security.Cryptography;

namespace poxnora_search_engine
{
    public enum FilterType { AND, OR, BOOLEAN, INT, STRING, RARITY, EXPANSION, ABILITY_LIST, CLASS_LIST, FACTION_LIST, RACE_LIST }
    public partial class MainForm : Form
    {
        ViewModeEnum ViewMode = ViewModeEnum.NONE;
        Pox.DataElement.ElementType ViewType = Pox.DataElement.ElementType.NONE;

        List<RuneListInfo> Cards = new List<RuneListInfo>();
        int HighestCard = 0;
        int SelectedCard = Utility.NO_INDEX;
        const int MaxCard = 6;

        Updater app_updater = new Updater();

        CardRandomizer CardRandomizer_form = null;
        ChampionBuilder ChampionBuilder_form = null;
        DifferenceCalculator DifferenceCalculator_form = null;
        BattlegroupBuilder BattlegroupBuilder_form = null;

        public MainForm()
        {
            InitializeComponent();
            ButtonAddCard.Tag = -1;
            Log.OnLog = ShowLogMessageOnStatusBar;
        }

        // when a message is logged, it shows on status bar (unless its a mundane info message)
        private void ShowLogMessageOnStatusBar(Log.LogOption o, Log.LogSource s, string d)
        {
            System.Diagnostics.Debug.WriteLine("[" + o.ToString() + "] " + s.ToString() + ": " + d);
            if (o != Log.LogOption.INFO)
                LastLogMessage.Text = s.ToString()+": "+d;
        }

        // called to update progress bar on downloading database and updated archive, respectively
        void OnDBDownloadProgressChanged(int percentage)
        {
            DBDownloadProgress.Value = Math.Min(100, Math.Max(0, percentage));
        }

        void OnArchiveProgressChanged(int percentage)
        {
            ArchiveDownloadProgress.Value = Math.Min(100, Math.Max(0, percentage));
        }

        // called when latest version number is retrieved from the server
        void OnFetchCurrentVersion(bool is_current_outdated, string new_version)
        {
            if (is_current_outdated)
            {
                StatusNewVersionAvailable.Visible = true;
                Log.Info(Log.LogSource.Net, "MainForm.OnFetchCurrentVersion(): New version available: " + new_version);
            }
            else
                Log.Info(Log.LogSource.Net, "MainForm.OnFetchCurrentVersion(): No new version available");
        }

        // called when could not download or extract latest version archive
        void OnFetchCurrentArchiveFailed()
        {
            Log.Error(Log.LogSource.Net, "MainForm.OnFetchCurrentArchiveFailed(): Could not install latest version");

            StatusNewVersionAvailable.IsLink = true;
            StatusNewVersionAvailable.Text = "New version available";

            ArchiveDownloadProgress.Visible = false;
        }

        // called when latest version was installed, it will shut down current instance and start a new one
        void OnFetchCurrentArchiveSuccess()
        {
            Log.Info(Log.LogSource.Net, "MainForm.OnFetchCurrentArchiveSuccess(): New version successfully installed, restarting...");

            string dir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.Diagnostics.Process.Start(dir+"\\onupdate.bat");
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.Info(Log.LogSource.UI, "MainForm.Form1_Load() called");

            Program.database.ready_trigger = OnDatabaseReady;
            Program.database.progress_trigger = OnDBDownloadProgressChanged;

            Database.DatabaseLoadInfo dli = Program.database.Load("database.json", Pox.Database.POXNORA_JSON_MAIN, false);
            DBDownloadProgress.Visible = !(dli.NoDownload);

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
            foreach (ToolStripMenuItem column_vis in conditionsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnConditionColumnVisibilityClick);
            foreach (ToolStripMenuItem column_vis in mechanicsToolStripMenuItem.DropDownItems)
                column_vis.Click += new EventHandler(OnMechanicColumnVisibilityClick);

            PanelRunePreviews.MouseWheel += PanelRunePreviews_MouseWheel;

            // this makes datagridview performance bearable :)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                GridDataElements,
                new object[] { true });

           typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                PanelRunePreviews,
                new object[] { true });

            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);

            // retrieve latest version number from the server
            app_updater._OnGetVersion = OnFetchCurrentVersion;
            app_updater._OnGetArchiveFailed = OnFetchCurrentArchiveFailed;
            app_updater._OnGetArchiveSuccess = OnFetchCurrentArchiveSuccess;
            app_updater._OnArchiveProgressChanged = OnArchiveProgressChanged;
            app_updater.GetLatestVersion();

            PanelRunePreviews.Location = GridDataElements.Location;
            PanelRunePreviews.Size = GridDataElements.Size;

            DatabaseFilter.ApplyFilters_callback = ApplyFilter;

            Log.Info(Log.LogSource.UI, "MainForm.Form1_Load() finished");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Info(Log.LogSource.UI, "MainForm.MainForm_FormClosed() called");

            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
            Program.image_cache.BreakRunePreviewDownload();

            Log.Info(Log.LogSource.UI, "MainForm.MainForm_FormClosed() finished");
        }

        // called when database was loaded
        private void OnDatabaseReady()
        {
            Log.Info(Log.LogSource.PoxDB, "MainForm.OnDatabaseReady() called");

            RuneDescription.database_ref = Program.database;

            DBDownloadProgress.Visible = false;

            ViewMode = ViewModeEnum.NONE;
            ViewType = Pox.DataElement.ElementType.NONE;

            if (Cards.Count == 0)
                SelectedCard = AddCard(new RuneListInfo() { Filter = null, ApplyFilter = false, ViewMode = ViewModeEnum.GRID, ViewType = DataElement.ElementType.CHAMPION });

            ReloadCard(SelectedCard);
        }

        private void OnCardButtonClick(object sender, EventArgs e)
        {
            SelectCard((int)((Button)sender).Tag);
        }

        private int AddCard(RuneListInfo card)
        {
            if (Cards.Count >= MaxCard)
                return -1;

            Cards.Add(card);
            HighestCard += 1;

            Button b = new Button();
            b.Tag = Cards.Count - 1;
            b.Text = String.Format("Card #{0}", HighestCard);
            b.MouseDown += new MouseEventHandler(ButtonCard_MouseDown);
            b.Size = new Size(80, 23);
            b.Location = new Point(ButtonAddCard.Location.X, ButtonAddCard.Location.Y);
            ButtonAddCard.Location = new Point(ButtonAddCard.Location.X + 86, ButtonAddCard.Location.Y);
            PanelCards.Controls.Add(b);

            return Cards.Count - 1;
        }

        private void SelectCard(int card_index)
        {
            if (card_index == SelectedCard)
                return;

            if(SelectedCard != Utility.NO_INDEX)
                SaveCard(SelectedCard);

            SelectedCard = card_index;
            ReloadCard(card_index);
        }

        private void ReloadCard(int card_index)
        {
            if ((ViewMode != Cards[card_index].ViewMode) || (ViewType != Cards[card_index].ViewType))
            {
                ViewMode = Cards[card_index].ViewMode;
                ViewType = Cards[card_index].ViewType;
                PrepareView();
            }

            UpdateCard(card_index);
        }

        private void SaveCard(int card_index)
        {
            if (card_index == Utility.NO_INDEX)
                return;

            Cards[card_index].Filter = DatabaseFilter.SearchFilter;
            Cards[card_index].ApplyFilter = DatabaseFilter.ApplyFilters;
            Cards[card_index].ViewMode = ViewMode;
            Cards[card_index].ViewType = ViewType;
        }

        private void UpdateCard(int card_index)
        {
            DatabaseFilter.RebuildFilter(Cards[card_index].Filter);
            DatabaseFilter.ApplyFilters = Cards[card_index].ApplyFilter;
            ApplyFilter();
        }

        private bool RemoveCard(int card_index)
        {
            if (Cards.Count == 1)
                return false;

            Cards.RemoveAt(card_index);

            Control rem_control = null;
            foreach(Control c in PanelCards.Controls)
            {
                int cur_card_index = (int)(c.Tag);
                if(cur_card_index == -1)
                {
                    c.Location = new Point(c.Location.X - 86, c.Location.Y);
                }
                else if(cur_card_index == card_index)
                {
                    rem_control = c;
                }
                else if(cur_card_index > card_index)
                {
                    c.Tag = cur_card_index - 1;
                    c.Location = new Point(c.Location.X - 86, c.Location.Y);
                }
            }
            if(rem_control != null)
            {
                rem_control.MouseDown -= new MouseEventHandler(ButtonCard_MouseDown);
                PanelCards.Controls.Remove(rem_control);
            }
            if (SelectedCard == Cards.Count)
                SelectedCard -= 1;

            return true;
        }

        private void ClearCards()
        {
            if (Cards.Count == 0)
                AddCard(new RuneListInfo() { Filter = null, ViewMode = ViewModeEnum.GRID, ViewType = DataElement.ElementType.CHAMPION, ApplyFilter = false });
            else
            {
                while (Cards.Count > 1)
                    RemoveCard(SelectedCard);
                Cards[0] = new RuneListInfo() { Filter = null, ViewMode = ViewMode, ViewType = ViewType };
            }
            SelectedCard = -1;
            SelectCard(0);
        }

        public RuneListInfo GetSelectedCard()
        {
            SaveCard(SelectedCard);
            // refresh current card
            return Cards[SelectedCard];
        }

        // refreshes the database elements view
        private void PrepareView()
        {
            if (ViewMode == ViewModeEnum.GRID)
                PrepareGridView();
            else if (ViewMode == ViewModeEnum.IMAGES)
                PrepareImageView();
            else
            {
                Log.Error(Log.LogSource.Main, "PrepareView(): Unknown view mode!");
            }
        }

        // refreshes the grid view
        public void PrepareGridView()
        {
            Log.Info(Log.LogSource.UI, "MainForm.PrepareGridView() called");

            GridDataElements.Visible = true;
            PanelRunePreviews.Visible = false;
            PreviewScrollBar.Hide();
            Program.image_cache.BreakRunePreviewDownload();


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
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Default nora cost", Name = "DefaultNoraCost", ValueType = typeof(int), Visible = defaultNoraCostToolStripMenuItem1.Checked });
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
                case Pox.DataElement.ElementType.CONDITION:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Key", Name = "Key", ValueType = typeof(string), Visible = keyToolStripMenuItem.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem5.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem5.Checked });
                        break;
                    }
                case Pox.DataElement.ElementType.MECHANIC:
                    {
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Key", Name = "Key", ValueType = typeof(string), Visible = keyToolStripMenuItem1.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Name", Name = "Name", ValueType = typeof(string), Visible = nameToolStripMenuItem6.Checked });
                        GridDataElements.Columns.Add(new DataGridViewColumn() { HeaderText = "Description", Name = "Description", ValueType = typeof(string), Visible = descriptionToolStripMenuItem6.Checked });
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

            Log.Info(Log.LogSource.UI, "MainForm.PrepareGridView() finished");
        }

        // adds elements to the grid
        void ReloadGrid()
        {
            Log.Info(Log.LogSource.UI, "MainForm.ReloadGrid() called");

            switch (ViewType)
            {
                case Pox.DataElement.ElementType.CHAMPION:
                    {
                        List<Champion> champs = new List<Champion>();
                        foreach (var kv in Program.database.Champions)
                            champs.Add(kv.Value);

                        foreach (var c in champs)
                            GridAddChampion(c);

                        break;
                    }
                case Pox.DataElement.ElementType.ABILITY:
                    {
                        List<Ability> abs = new List<Ability>();
                        foreach (var kv in Program.database.Abilities)
                            abs.Add(kv.Value);

                        foreach (var a in abs)
                            GridAddAbility(a);

                        break;
                    }
                case Pox.DataElement.ElementType.SPELL:
                    {
                        List<Spell> sps = new List<Spell>();
                        foreach (var kv in Program.database.Spells)
                            sps.Add(kv.Value);

                        foreach (var s in sps)
                            GridAddSpell(s);

                        break;
                    }
                case Pox.DataElement.ElementType.RELIC:
                    {
                        List<Relic> rls = new List<Relic>();
                        foreach (var kv in Program.database.Relics)
                            rls.Add(kv.Value);

                        foreach (var r in rls)
                            GridAddRelic(r);

                        break;
                    }
                case Pox.DataElement.ElementType.EQUIPMENT:
                    {
                        List<Equipment> eqs = new List<Equipment>();
                        foreach (var kv in Program.database.Equipments)
                            eqs.Add(kv.Value);

                        foreach (var e in eqs)
                            GridAddEquipment(e);

                        break;
                    }
                case Pox.DataElement.ElementType.CONDITION:
                    {
                        List<FlavorElement> cons = new List<FlavorElement>();
                        foreach (var kv in Program.database.Conditions)
                            cons.Add(kv.Value);

                        foreach (var c in cons)
                            GridAddCondition(c);

                        break;
                    }
                case Pox.DataElement.ElementType.MECHANIC:
                    {
                        List<FlavorElement> mecs = new List<FlavorElement>();
                        foreach (var kv in Program.database.Mechanics)
                            mecs.Add(kv.Value);

                        foreach (var m in mecs)
                            GridAddMechanic(m);

                        break;
                    }
                default:
                    break;
            }

            Log.Info(Log.LogSource.UI, "MainForm.ReloadGrid() finished");
        }

        public void GridAddChampion(Pox.Champion c)
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


        public void GridAddAbility(Pox.Ability a)
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

        public void GridAddSpell(Pox.Spell s)
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

        public void GridAddRelic(Pox.Relic r)
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

        public void GridAddEquipment(Pox.Equipment e)
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

        public void GridAddCondition(Pox.FlavorElement c)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = c;

            GridDataElements.Rows[row].Cells["Key"].Value = c.Key;
            GridDataElements.Rows[row].Cells["Name"].Value = c.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = c.Description;
        }

        public void GridAddMechanic(Pox.FlavorElement m)
        {
            int row = GridDataElements.Rows.Add();
            GridDataElements.Rows[row].Tag = m;

            GridDataElements.Rows[row].Cells["Key"].Value = m.Key;
            GridDataElements.Rows[row].Cells["Name"].Value = m.Name;
            GridDataElements.Rows[row].Cells["Description"].Value = m.Description;
        }

        // refreshes image view
        private void PrepareImageView()
        {
            Log.Info(Log.LogSource.UI, "MainForm.PrepareImageView() called");

            GridDataElements.Visible = false;
            PanelRunePreviews.Visible = true;

            ApplyFilter();

            Log.Info(Log.LogSource.UI, "MainForm.PrepareImageView() finished");
        }

        // resets image positions in image view
        private void ReadjustPreviewImages()
        {
            Log.Info(Log.LogSource.UI, "MainForm.ReadjustPreviewImages() called");

            PanelRunePreviews.SuspendLayout();

            int items_per_row = (PanelRunePreviews.Width - 6) / 82;
            for (int i = 0; i < PanelRunePreviews.Controls.Count; i++)
                PanelRunePreviews.Controls[i].Location = new Point(3 + ((i % items_per_row) * 82), 3 + ((i / items_per_row) * 113));

            PanelRunePreviews.ResumeLayout();

            Log.Info(Log.LogSource.UI, "MainForm.ReadjustPreviewImages() finished");
        }

        // limits elements in the view to the ones that meet filter criteria
        private void ApplyFilter()
        {
            Log.Info(Log.LogSource.UI, "MainForm.ApplyFilter() called");

            if (ViewMode == ViewModeEnum.GRID)
            {
                GridDataElements.Hide();

                if ((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters))
                {
                    foreach (DataGridViewRow row in GridDataElements.Rows)
                        row.Visible = true;
                    LastLogMessage.Text = "Items on grid: " + GridDataElements.Rows.Count.ToString();
                }
                else
                {
                    int total = 0;
                    foreach (DataGridViewRow row in GridDataElements.Rows)
                    {
                        row.Visible = DatabaseFilter.SearchFilter.Satisfies((DataElement)row.Tag);
                        if (row.Visible)
                            total += 1;
                    }
                    LastLogMessage.Text = "Items on grid: " + total.ToString();
                }

                GridDataElements.Show();
            }
            else if (ViewMode == ViewModeEnum.IMAGES)
            {
                PanelRunePreviews.SuspendLayout();
                PanelRunePreviews.Hide();

                Program.image_cache.BreakRunePreviewDownload();

                PanelRunePreviews.VerticalScroll.Value = 0;
                for(int i = 0; i < PanelRunePreviews.Controls.Count; i++)
                    PanelRunePreviews.Controls[i].Hide();

                int total = 0;
                // item size: 76, 107
                int items_per_row = (PanelRunePreviews.Width - 6) / 82;

                List<string> elems = new List<string>();

                RunePreviewControl rpc;

                switch (ViewType)
                {
                    case DataElement.ElementType.CHAMPION:

                        foreach (var champ in Program.database.Champions.Keys)
                        {
                            if (((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters)) || (DatabaseFilter.SearchFilter.Satisfies(Program.database.Champions[champ])))
                            {
                                if (PanelRunePreviews.Controls.Count > total)
                                {
                                    rpc = (RunePreviewControl)(PanelRunePreviews.Controls[total]);
                                    rpc.LabelText.Text = "";
                                    rpc.RunePreviewImage.Image = null;
                                    rpc.Show();
                                }
                                else
                                {
                                    rpc = new RunePreviewControl();
                                    rpc.RunePreviewImage.MouseClick += PreviewImage_MouseClick;
                                    PanelRunePreviews.Controls.Add(rpc);
                                }
                                rpc.ElemID = Program.database.Champions[champ].ID;
                                rpc.LabelText.Text = Program.database.Champions[champ].Name;
                                elems.Add(Program.database.Champions[champ].Hash);
                                total += 1;
                            }
                        }
                        break;
                    case DataElement.ElementType.ABILITY:

                        foreach (var ability in Program.database.Abilities.Keys)
                        {
                            if (((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters)) || (DatabaseFilter.SearchFilter.Satisfies(Program.database.Abilities[ability])))
                            {
                                if (PanelRunePreviews.Controls.Count > total)
                                {
                                    rpc = (RunePreviewControl)(PanelRunePreviews.Controls[total]);
                                    rpc.LabelText.Text = "";
                                    rpc.RunePreviewImage.Image = null;
                                    rpc.Show();
                                }
                                else
                                {
                                    rpc = new RunePreviewControl();
                                    rpc.RunePreviewImage.MouseClick += PreviewImage_MouseClick;
                                    PanelRunePreviews.Controls.Add(rpc);
                                }
                                rpc.ElemID = Program.database.Abilities[ability].ID;
                                rpc.LabelText.Text = Program.database.Abilities[ability].Name;
                                elems.Add(Program.database.Abilities[ability].IconName);
                                total += 1;
                            }
                        }
                        break;
                    case DataElement.ElementType.SPELL:

                        foreach (var champ in Program.database.Spells.Keys)
                        {
                            if (((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters)) || (DatabaseFilter.SearchFilter.Satisfies(Program.database.Spells[champ])))
                            {
                                if (PanelRunePreviews.Controls.Count > total)
                                {
                                    rpc = (RunePreviewControl)(PanelRunePreviews.Controls[total]);
                                    rpc.LabelText.Text = "";
                                    rpc.RunePreviewImage.Image = null;
                                    rpc.Show();
                                }
                                else
                                {
                                    rpc = new RunePreviewControl();
                                    rpc.RunePreviewImage.MouseClick += PreviewImage_MouseClick;
                                    PanelRunePreviews.Controls.Add(rpc);
                                }
                                rpc.ElemID = Program.database.Spells[champ].ID;
                                rpc.LabelText.Text = Program.database.Spells[champ].Name;
                                elems.Add(Program.database.Spells[champ].Hash);
                                total += 1;
                            }
                        }
                        break;
                    case DataElement.ElementType.RELIC:

                        foreach (var champ in Program.database.Relics.Keys)
                        {
                            if (((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters)) || (DatabaseFilter.SearchFilter.Satisfies(Program.database.Relics[champ])))
                            {
                                if (PanelRunePreviews.Controls.Count > total)
                                {
                                    rpc = (RunePreviewControl)(PanelRunePreviews.Controls[total]);
                                    rpc.LabelText.Text = "";
                                    rpc.RunePreviewImage.Image = null;
                                    rpc.Show();
                                }
                                else
                                {
                                    rpc = new RunePreviewControl();
                                    rpc.RunePreviewImage.MouseClick += PreviewImage_MouseClick;
                                    PanelRunePreviews.Controls.Add(rpc);
                                }
                                rpc.ElemID = Program.database.Relics[champ].ID;
                                rpc.LabelText.Text = Program.database.Relics[champ].Name;
                                elems.Add(Program.database.Relics[champ].Hash);
                                total += 1;
                            }
                        }
                        break;
                    case DataElement.ElementType.EQUIPMENT:

                        foreach (var champ in Program.database.Equipments.Keys)
                        {
                            if (((DatabaseFilter.SearchFilter == null) || (!DatabaseFilter.ApplyFilters)) || (DatabaseFilter.SearchFilter.Satisfies(Program.database.Equipments[champ])))
                            {
                                if (PanelRunePreviews.Controls.Count > total)
                                {
                                    rpc = (RunePreviewControl)(PanelRunePreviews.Controls[total]);
                                    rpc.LabelText.Text = "";
                                    rpc.RunePreviewImage.Image = null;
                                    rpc.Show();
                                }
                                else
                                {
                                    rpc = new RunePreviewControl();
                                    rpc.RunePreviewImage.MouseClick += PreviewImage_MouseClick;
                                    PanelRunePreviews.Controls.Add(rpc);
                                }
                                rpc.ElemID = Program.database.Equipments[champ].ID;
                                rpc.LabelText.Text = Program.database.Equipments[champ].Name;
                                elems.Add(Program.database.Equipments[champ].Hash);
                                total += 1;
                            }
                        }
                        break;
                    default:
                        break;
                }

                // set up panel for viewing and scrolling
                int h = 6 + (((total) / items_per_row) * 113) + 113;
                PanelRunePreviews.Height = h;

                PreviewScrollBar.Show();
                PreviewScrollBar.Minimum = 0;
                PreviewScrollBar.Maximum = Math.Max(0, h - GridDataElements.Height);
                PreviewScrollBar.Value = 0;

                ReadjustPreviewImages();

                PanelRunePreviews.Show();

                if (ViewType == DataElement.ElementType.ABILITY)
                {
                    for (int i = 0; i < total; i++)
                        Program.image_cache.AddAbilityImageSubscriber(elems[i], ((RunePreviewControl)(PanelRunePreviews.Controls[i])));
                }
                else
                {
                    for (int i = 0; i < total; i++)
                        Program.image_cache.AddRunePreviewSubscriber(elems[i], ((RunePreviewControl)(PanelRunePreviews.Controls[i])));
                }

                LastLogMessage.Text = "Items displayed: " + total.ToString();
            }

            Log.Info(Log.LogSource.UI, "MainForm.ApplyFilter() finished");
        }
        

        private void RadioChampions_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.CHAMPION;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioAbilities_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.ABILITY;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioSpells_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.SPELL;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioRelics_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.RELIC;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioEquips_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.EQUIPMENT;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioConditions_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.CONDITION;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void RadioMechanics_CheckedChanged(object sender, EventArgs e)
        {
            ViewType = Pox.DataElement.ElementType.MECHANIC;
            Cards[SelectedCard].ViewType = ViewType;
            PrepareView();
        }

        private void GridDataElements_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            // text keys
            if ((ViewType == DataElement.ElementType.CONDITION) || (ViewType == DataElement.ElementType.MECHANIC))
            {
                if (GridDataElements.Rows[row].Cells["Key"].Value == null)
                    return;

                string id = (string)GridDataElements.Rows[row].Cells["Key"].Value;

                RuneDescription.TracerClear();
                switch (ViewType)
                {
                    case DataElement.ElementType.CONDITION:
                        RuneDescription.SetCondition(Program.database.Conditions[id]);
                        break;
                    case DataElement.ElementType.MECHANIC:
                        RuneDescription.SetMechanic(Program.database.Mechanics[id]);
                        break;
                    default:
                        RuneDescription.ClearDescription();
                        break;
                }
            }
            else  // number keys
            {
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
        }

        private void PreviewImage_MouseClick(object sender, MouseEventArgs e)
        {
            int id = ((RunePreviewControl)(((PictureBox)sender).Parent)).ElemID;

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
            Log.Info(Log.LogSource.UI, "MainForm.Form1_Resize() called, new size: "+Size.ToString());

            if (this.Width <= 1410 - 764)
                return;
            if (this.Height <= 686 - 565)
                return;

            GridDataElements.Width = this.Width - 1410 + 764;
            GridDataElements.Height = this.Height - 682 + 534;

            RuneDescription.Location = new Point(GridDataElements.Location.X + GridDataElements.Width + 3, PanelCards.Location.Y);
            RuneDescription.SetHeight(GridDataElements.Height + 53);
            PanelRunePreviews.Location = new Point(GridDataElements.Location.X, PreviewScrollBar.Value);
            PanelRunePreviews.Size = GridDataElements.Size;
            PanelCards.Width = GridDataElements.Width;
            PanelDataMode.Width = GridDataElements.Width;
            ButtonSetViewMode.Location = new Point(PanelDataMode.Width - ButtonSetViewMode.Width, ButtonSetViewMode.Location.Y);
            PreviewScrollBar.Location = new Point(RuneDescription.Location.X - PreviewScrollBar.Width, GridDataElements.Location.Y);
            PreviewScrollBar.Height = GridDataElements.Height;
            PreviewScrollBar.Minimum = 0;
            PreviewScrollBar.Maximum = Math.Max(0, PanelRunePreviews.Height - GridDataElements.Height);
            PreviewScrollBar.Value = 0;
            PanelRunePreviews.Location = new Point(PanelRunePreviews.Location.X, GridDataElements.Location.Y);
            PreviewScrollBar.SmallChange = 113;
            if (PanelRunePreviews.Visible)
                ReadjustPreviewImages();
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

        private void OnConditionColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.CONDITION)
                return;

            if (GridDataElements.Columns.Contains(((ToolStripMenuItem)sender).Tag.ToString()))
                GridDataElements.Columns[((ToolStripMenuItem)sender).Tag.ToString()].Visible = ((ToolStripMenuItem)sender).Checked;
        }

        private void OnMechanicColumnVisibilityClick(object sender, EventArgs e)
        {
            if (ViewType != Pox.DataElement.ElementType.MECHANIC)
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

            Log.Info(Log.LogSource.UI, "MainForm.deckRandomizerToolStripMenuItem_Click() called");

            CardRandomizer_form = new CardRandomizer();
            CardRandomizer_form.ShowDialog();
            CardRandomizer_form = null;

            Log.Info(Log.LogSource.UI, "MainForm.deckRandomizerToolStripMenuItem_Click() finished");
        }

        private void championBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.database.ready)
                return;

            if (ChampionBuilder_form != null)
            {
                ChampionBuilder_form.BringToFront();
                return;
            }

            Log.Info(Log.LogSource.UI, "MainForm.championBuilderToolStripMenuItem_Click(): Creating new form");
            ChampionBuilder_form = new ChampionBuilder();
            ChampionBuilder_form.FormClosed += new FormClosedEventHandler(ChampionBuilder_form_FormClosed);

            ChampionBuilder_form.Show();
        }

        private void ChampionBuilder_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChampionBuilder_form.FormClosed -= new FormClosedEventHandler(ChampionBuilder_form_FormClosed);
            ChampionBuilder_form = null;

            Log.Info(Log.LogSource.UI, "MainForm.ChampionBuilder_form_FormClosed(): Form closed");
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

            Log.Info(Log.LogSource.UI, "MainForm.differenceCalculatorToolStripMenuItem_Click(): Creating new form");
            DifferenceCalculator_form = new DifferenceCalculator();
            DifferenceCalculator_form.FormClosed += new FormClosedEventHandler(DifferenceCalculator_form_FormClosed);

            DifferenceCalculator_form.Show();
        }

        private void DifferenceCalculator_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            DifferenceCalculator_form.FormClosed -= new FormClosedEventHandler(DifferenceCalculator_form_FormClosed);
            DifferenceCalculator_form = null;

            Log.Info(Log.LogSource.UI, "MainForm.DifferenceCalculator_form_FormClosed(): Form closed");
        }

        private void battlegroundBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.database.ready)
                return;

            if (BattlegroupBuilder_form != null)
            {
                BattlegroupBuilder_form.BringToFront();
                return;
            }

            Log.Info(Log.LogSource.UI, "MainForm.battlegroundBuilderToolStripMenuItem_Click(): Creating new form");
            BattlegroupBuilder_form = new BattlegroupBuilder();
            BattlegroupBuilder_form.FormClosed += new FormClosedEventHandler(BattlegroupBuilder_form_FormClosed);

            BattlegroupBuilder_form.Show();
        }

        private void BattlegroupBuilder_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            BattlegroupBuilder_form.FormClosed -= new FormClosedEventHandler(BattlegroupBuilder_form_FormClosed);
            BattlegroupBuilder_form = null;

            Log.Info(Log.LogSource.UI, "MainForm.BattlegroupBuilder_form_FormClosed(): Form closed");
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
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

            ArchiveDownloadProgress.Visible = true;
            app_updater.ForceInstallLatestVersion();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ViewMode == ViewModeEnum.GRID)
            {
                ViewMode = ViewModeEnum.IMAGES;
                ButtonSetViewMode.Text = "Switch to grid";
            }
            else
            {
                ViewMode = ViewModeEnum.GRID;
                ButtonSetViewMode.Text = "Switch to images";
            }

            Cards[SelectedCard].ViewMode = ViewMode;
            PrepareView();
        }

        private void PreviewScrollBar_ValueChanged(object sender, EventArgs e)
        {
            PanelRunePreviews.SuspendLayout();
            PanelRunePreviews.Location = new Point(PanelRunePreviews.Location.X, GridDataElements.Location.Y - PreviewScrollBar.Value);
            PanelRunePreviews.ResumeLayout();
        }

        private void PanelRunePreviews_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!PanelRunePreviews.Visible)
                return;

            int val = e.Delta * 113 / 120;
            PreviewScrollBar.Value = Math.Max(PreviewScrollBar.Minimum, Math.Min(PreviewScrollBar.Maximum, PreviewScrollBar.Value - val));
        }

        private void loadOlderDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // cant do it while database is being downloaded
            if (Program.database.loading)
                return;

            if (DBLoadDialog.ShowDialog() != DialogResult.OK)
                return;


            // unload current database
            if(CardRandomizer_form != null)
                CardRandomizer_form.Close();
            CardRandomizer_form = null;

            if (ChampionBuilder_form != null)
                ChampionBuilder_form.Close();
            ChampionBuilder_form = null;

            if (DifferenceCalculator_form != null)
                DifferenceCalculator_form.Close();
            DifferenceCalculator_form = null;

            if (BattlegroupBuilder_form != null)
                BattlegroupBuilder_form.Close();
            BattlegroupBuilder_form = null;

            RuneDescription.ClearDescription();
            Program.image_cache.RuneImageSubscribers.Remove(RuneDescription);
            Program.image_cache.BreakRunePreviewDownload();
            RuneDescription.database_ref = null;

            SaveCard(SelectedCard);
            DatabaseFilter.Clear();

            GridDataElements.Rows.Clear();
            PanelRunePreviews.Controls.Clear();

            Program.database.Unload();

            // load new database from file
            Program.database.Load(DBLoadDialog.FileName, "", true);
            if (!Program.database.ready)
                return;

            RuneDescription.database_ref = Program.database;
            Program.image_cache.RuneImageSubscribers.Add(RuneDescription);
        }

        private void ButtonCard_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectCard((int)(((Button)sender).Tag));
            else if (e.Button == MouseButtons.Right)
            {
                if (RemoveCard((int)(((Button)sender).Tag)))
                    ReloadCard(SelectedCard);
            }
        }

        private void ButtonAddCard_MouseDown(object sender, MouseEventArgs e)
        {
            AddCard(new RuneListInfo() { Filter = null, ViewMode = ViewMode, ViewType = ViewType, ApplyFilter = false });
        }

        private void ButtonQuickFilter_Click(object sender, EventArgs e)
        {
            DatabaseFilter.InvokeQuickFilter();
        }
    }
}
