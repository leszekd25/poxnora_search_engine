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

namespace poxnora_search_engine
{
    public partial class SynergyComparerForm : Form
    {
        Database old_db;
        Database new_db;

        List<string> allowed_races;
        List<string> allowed_classes;
        List<string> allowed_factions;

        int fac1;
        int fac2;

        public SynergyComparerForm()
        {
            InitializeComponent();
        }

        public void Init(Database db1, Database db2)
        {
            old_db = db1;
            old_db.Plugin_SynergyComparer.Load(old_db);
            new_db = db2;
            new_db.Plugin_SynergyComparer.Load(new_db);

            // generate faction combos
            List<string> factions1 = db1.Factions.GetStringsSortedDefault();
            List<string> factions2 = db2.Factions.GetStringsSortedDefault();
            HashSet<string> allowed_factions_set = new HashSet<string>();
            foreach (var s in factions1) allowed_factions_set.Add(s);
            foreach (var s in factions2) allowed_factions_set.Add(s);
            allowed_factions = allowed_factions_set.ToList();
            allowed_factions.Sort();

            ComboFaction2.Items.Add("<empty>");
            foreach(var af in allowed_factions)
            {
                ComboFaction1.Items.Add(af);
                ComboFaction2.Items.Add(af);
            }
            ComboFaction1.SelectedIndexChanged -= ComboFaction1_SelectedIndexChanged;
            ComboFaction1.SelectedIndex = 0;
            ComboFaction1.SelectedIndexChanged += ComboFaction1_SelectedIndexChanged;
            fac1 = 0;
            ComboFaction2.SelectedIndexChanged -= ComboFaction2_SelectedIndexChanged;
            ComboFaction2.SelectedIndex = 0;
            ComboFaction2.SelectedIndexChanged += ComboFaction2_SelectedIndexChanged;
            fac2 = -1;

            // generate race combos
            List<string> races1 = db1.Races.GetStringsSortedDefault();
            List<string> races2 = db2.Races.GetStringsSortedDefault();
            HashSet<string> allowed_races_set = new HashSet<string>();
            foreach (var s in races1) allowed_races_set.Add(s);
            foreach (var s in races2) allowed_races_set.Add(s);
            allowed_races = allowed_races_set.ToList();
            allowed_races.Sort();

            // generate class combos
            List<string> classes1 = db1.Classes.GetStringsSortedDefault();
            List<string> classes2 = db2.Classes.GetStringsSortedDefault();
            HashSet<string> allowed_classes_set = new HashSet<string>();
            foreach (var s in classes1) allowed_classes_set.Add(s);
            foreach (var s in classes2) allowed_classes_set.Add(s);
            allowed_classes = allowed_classes_set.ToList();
            allowed_classes.Sort();

            ShowControls();
            LoadGrid();
        }

        void ClearGrid()
        {
            GridSynergies.Rows.Clear();
        }

        void LoadFactionRaces()
        {
            ClearGrid();

            GridSynergies.Columns[0].HeaderText = "Race";

            string f1 = ComboFaction1.SelectedItem.ToString();
            string f2 = "";
            if (ComboFaction2.SelectedIndex != 0)
            {
                f2 = ComboFaction2.SelectedItem.ToString();
            }
            SynergyContextKey sck = new SynergyContextKey(f1, f2);

            SynergyContext old_ctx = old_db.Plugin_SynergyComparer.Synergies[sck];
            SynergyContext new_ctx = new_db.Plugin_SynergyComparer.Synergies[sck];

            foreach(var s in allowed_races)
            {
                int row_index = GridSynergies.Rows.Add();
                GridSynergies.Rows[row_index].Cells[0].Value = s;
                if (old_ctx.RacePower.ContainsKey(s))
                {
                    GridSynergies.Rows[row_index].Cells[1].Value = old_ctx.RacePower[s].AveragePower;
                    GridSynergies.Rows[row_index].Cells[3].Value = old_ctx.RacePower[s].MaxPower;
                    if (old_ctx.RacePower[s].ChampCount < 10)
                    {
                        GridSynergies.Rows[row_index].Cells[1].Style.BackColor = Color.LightSkyBlue;
                        GridSynergies.Rows[row_index].Cells[3].Style.BackColor = Color.LightSkyBlue;
                    }
                }
                if (new_ctx.RacePower.ContainsKey(s))
                {
                    GridSynergies.Rows[row_index].Cells[2].Value = new_ctx.RacePower[s].AveragePower;
                    GridSynergies.Rows[row_index].Cells[4].Value = new_ctx.RacePower[s].MaxPower;
                    if (new_ctx.RacePower[s].ChampCount < 10)
                    {
                        GridSynergies.Rows[row_index].Cells[2].Style.BackColor = Color.LightSkyBlue;
                        GridSynergies.Rows[row_index].Cells[4].Style.BackColor = Color.LightSkyBlue;
                    }
                }
            }
        }

        void LoadFactionClasses()
        {
            ClearGrid();

            GridSynergies.Columns[0].HeaderText = "Class";

            string f1 = ComboFaction1.SelectedItem.ToString();
            string f2 = "";
            if (ComboFaction2.SelectedIndex != 0)
            {
                f2 = ComboFaction2.SelectedItem.ToString();
            }
            SynergyContextKey sck = new SynergyContextKey(f1, f2);

            SynergyContext old_ctx = old_db.Plugin_SynergyComparer.Synergies[sck];
            SynergyContext new_ctx = new_db.Plugin_SynergyComparer.Synergies[sck];

            foreach (var s in allowed_classes)
            {
                int row_index = GridSynergies.Rows.Add();
                GridSynergies.Rows[row_index].Cells[0].Value = s;
                if (old_ctx.ClassPower.ContainsKey(s))
                {
                    GridSynergies.Rows[row_index].Cells[1].Value = old_ctx.ClassPower[s].AveragePower;
                    GridSynergies.Rows[row_index].Cells[3].Value = old_ctx.ClassPower[s].MaxPower;
                    if (old_ctx.ClassPower[s].ChampCount < 10)
                    {
                        GridSynergies.Rows[row_index].Cells[1].Style.BackColor = Color.LightSkyBlue;
                        GridSynergies.Rows[row_index].Cells[3].Style.BackColor = Color.LightSkyBlue;
                    }
                }
                if (new_ctx.ClassPower.ContainsKey(s))
                {
                    GridSynergies.Rows[row_index].Cells[2].Value = new_ctx.ClassPower[s].AveragePower;
                    GridSynergies.Rows[row_index].Cells[4].Value = new_ctx.ClassPower[s].MaxPower;
                    if (new_ctx.ClassPower[s].ChampCount < 10)
                    {
                        GridSynergies.Rows[row_index].Cells[2].Style.BackColor = Color.LightSkyBlue;
                        GridSynergies.Rows[row_index].Cells[4].Style.BackColor = Color.LightSkyBlue;
                    }
                }
            }
        }

        void LoadRaceFactions()
        {
            ClearGrid();

            GridSynergies.Columns[0].HeaderText = "Faction";

            string race = ComboRaceClass.SelectedItem.ToString();

            for(int i = 0; i < allowed_factions.Count; i++)
            {
                for(int j = -1; j < i; j++)
                {
                    string f1 = allowed_factions[i];
                    string f2 = "";
                    if (j != -1)
                    {
                        f2 = allowed_factions[j];
                    }
                    SynergyContextKey sck = new SynergyContextKey(f1, f2);

                    SynergyContext old_ctx = old_db.Plugin_SynergyComparer.Synergies[sck];
                    SynergyContext new_ctx = new_db.Plugin_SynergyComparer.Synergies[sck];

                    int row_index = GridSynergies.Rows.Add(new_ctx);

                    GridSynergies.Rows[row_index].Cells[0].Value = $"{f1}{(j == -1 ? "" : "/" + f2)}";
                    if (old_ctx.RacePower.ContainsKey(race))
                    {
                        GridSynergies.Rows[row_index].Cells[1].Value = old_ctx.RacePower[race].AveragePower;
                        GridSynergies.Rows[row_index].Cells[3].Value = old_ctx.RacePower[race].MaxPower;
                        if (old_ctx.RacePower[race].ChampCount < 10)
                        {
                            GridSynergies.Rows[row_index].Cells[1].Style.BackColor = Color.LightSkyBlue;
                            GridSynergies.Rows[row_index].Cells[3].Style.BackColor = Color.LightSkyBlue;
                        }
                    }
                    if (new_ctx.RacePower.ContainsKey(race))
                    {
                        GridSynergies.Rows[row_index].Cells[2].Value = new_ctx.RacePower[race].AveragePower;
                        GridSynergies.Rows[row_index].Cells[4].Value = new_ctx.RacePower[race].MaxPower;
                        if (new_ctx.RacePower[race].ChampCount < 10)
                        {
                            GridSynergies.Rows[row_index].Cells[2].Style.BackColor = Color.LightSkyBlue;
                            GridSynergies.Rows[row_index].Cells[4].Style.BackColor = Color.LightSkyBlue;
                        }
                    }
                }
            }
        }

        void LoadClassFactions()
        {
            ClearGrid();

            GridSynergies.Columns[0].HeaderText = "Faction";

            string klass = ComboRaceClass.SelectedItem.ToString();

            for (int i = 0; i < allowed_factions.Count; i++)
            {
                for (int j = -1; j < i; j++)
                {
                    string f1 = allowed_factions[i];
                    string f2 = "";
                    if (j != -1)
                    {
                        f2 = allowed_factions[j];
                    }
                    SynergyContextKey sck = new SynergyContextKey(f1, f2);

                    SynergyContext old_ctx = old_db.Plugin_SynergyComparer.Synergies[sck];
                    SynergyContext new_ctx = new_db.Plugin_SynergyComparer.Synergies[sck];

                    int row_index = GridSynergies.Rows.Add(new_ctx);

                    GridSynergies.Rows[row_index].Cells[0].Value = $"{f1}{(j == -1 ? "" : "/" + f2)}";
                    if (old_ctx.ClassPower.ContainsKey(klass))
                    {
                        GridSynergies.Rows[row_index].Cells[1].Value = old_ctx.ClassPower[klass].AveragePower;
                        GridSynergies.Rows[row_index].Cells[3].Value = old_ctx.ClassPower[klass].MaxPower;
                        if (old_ctx.ClassPower[klass].ChampCount < 10)
                        {
                            GridSynergies.Rows[row_index].Cells[1].Style.BackColor = Color.LightSkyBlue;
                            GridSynergies.Rows[row_index].Cells[3].Style.BackColor = Color.LightSkyBlue;
                        }
                    }
                    if (new_ctx.ClassPower.ContainsKey(klass))
                    {
                        GridSynergies.Rows[row_index].Cells[2].Value = new_ctx.ClassPower[klass].AveragePower;
                        GridSynergies.Rows[row_index].Cells[4].Value = new_ctx.ClassPower[klass].MaxPower;
                        if (new_ctx.ClassPower[klass].ChampCount < 10)
                        {
                            GridSynergies.Rows[row_index].Cells[2].Style.BackColor = Color.LightSkyBlue;
                            GridSynergies.Rows[row_index].Cells[4].Style.BackColor = Color.LightSkyBlue;
                        }
                    }
                }
            }
        }

        void LoadGlobal()
        {
            ClearGrid();

            GridSynergies.Columns[0].HeaderText = "Faction";

            for (int i = 0; i < allowed_factions.Count; i++)
            {
                for (int j = -1; j < i; j++)
                {
                    string f1 = allowed_factions[i];
                    string f2 = "";
                    if (j != -1)
                    {
                        f2 = allowed_factions[j];
                    }
                    SynergyContextKey sck = new SynergyContextKey(f1, f2);

                    SynergyContext old_ctx = old_db.Plugin_SynergyComparer.Synergies[sck];
                    SynergyContext new_ctx = new_db.Plugin_SynergyComparer.Synergies[sck];

                    int row_index = GridSynergies.Rows.Add(new_ctx);

                    GridSynergies.Rows[row_index].Cells[0].Value = $"{f1}{(j == -1 ? "" : "/" + f2)}";
                    GridSynergies.Rows[row_index].Cells[1].Value = old_ctx.GlobalPower.AveragePower;
                    GridSynergies.Rows[row_index].Cells[2].Value = new_ctx.GlobalPower.AveragePower;
                    GridSynergies.Rows[row_index].Cells[3].Value = old_ctx.GlobalPower.MaxPower;
                    GridSynergies.Rows[row_index].Cells[4].Value = new_ctx.GlobalPower.MaxPower;
                }
            }
        }

        void LoadGrid()
        {
            if (RadioFactionClasses.Checked)
            {
                LoadFactionClasses();
                return;
            }
            if (RadioFactionRaces.Checked)
            {
                LoadFactionRaces();
                return;
            }
            if (RadioClassFactions.Checked)
            {
                LoadClassFactions();
                return;
            }
            if (RadioRaceFactions.Checked)
            {
                LoadRaceFactions();
                return;
            }
            if (RadioGlobal.Checked)
            {
                LoadGlobal();
                return;
            }
        }

         void HideControls()
        {
            LabelFaction1.Hide();
            LabelFaction2.Hide();
            ComboFaction1.Hide();
            ComboFaction2.Hide();
            LabelRaceClass.Hide();
            ComboRaceClass.Hide();
        }

        void ShowFactionRaces()
        {
            HideControls();

            LabelFaction1.Show();
            LabelFaction2.Show();
            ComboFaction1.Show();
            ComboFaction2.Show();
        }

        void ShowFactionClasses()
        {

            HideControls();

            LabelFaction1.Show();
            LabelFaction2.Show();
            ComboFaction1.Show();
            ComboFaction2.Show();
        }

        void ShowRaceFactions()
        {
            HideControls();

            LabelRaceClass.Show();
            LabelRaceClass.Text = "Race";
            ComboRaceClass.Show();
            ComboRaceClass.SelectedIndexChanged -= ComboRaceClass_SelectedIndexChanged;
            ComboRaceClass.Items.Clear();
            foreach(var s in allowed_races)
            {
                ComboRaceClass.Items.Add(s);
            }
            ComboRaceClass.SelectedIndex = 0;
            ComboRaceClass.SelectedIndexChanged += ComboRaceClass_SelectedIndexChanged;
        }

        void ShowClassFactions()
        {
            HideControls();

            LabelRaceClass.Show();
            LabelRaceClass.Text = "Class";
            ComboRaceClass.Show();
            ComboRaceClass.SelectedIndexChanged -= ComboRaceClass_SelectedIndexChanged;
            ComboRaceClass.Items.Clear();
            foreach (var s in allowed_classes)
            {
                ComboRaceClass.Items.Add(s);
            }
            ComboRaceClass.SelectedIndex = 0;
            ComboRaceClass.SelectedIndexChanged += ComboRaceClass_SelectedIndexChanged;
        }

        void ShowGlobal()
        {
            HideControls();
        }

        void ShowControls()
        {
            if (RadioFactionClasses.Checked)
            {
                ShowFactionClasses();
                return;
            }
            if (RadioFactionRaces.Checked)
            {
                ShowFactionRaces();
                return;
            }
            if (RadioClassFactions.Checked)
            {
                ShowClassFactions();
                return;
            }
            if (RadioRaceFactions.Checked)
            {
                ShowRaceFactions();
                return;
            }
            if (RadioGlobal.Checked)
            {
                ShowGlobal();
                return;
            }
        }

        void RadioGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if(!((RadioButton)sender).Checked)
            {
                return;
            }

            ShowControls();
            LoadGrid();
        }

        private void ComboFaction1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboFaction1.SelectedIndex == -1)
            {
                return;
            }
            if(ComboFaction2.SelectedItem == ComboFaction1.SelectedItem)
            {
                ComboFaction2.SelectedIndexChanged -= ComboFaction2_SelectedIndexChanged;
                ComboFaction2.SelectedIndex = fac1 + 1;
                fac2 = fac1;
                ComboFaction2.SelectedIndexChanged += ComboFaction2_SelectedIndexChanged;
            }
            fac1 = ComboFaction1.SelectedIndex;

            LoadGrid();
        }

        private void ComboFaction2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboFaction2.SelectedIndex == -1)
            {
                return;
            }
            if (ComboFaction2.SelectedItem == ComboFaction1.SelectedItem)
            {
                if(fac2 == -1)
                {
                    ComboFaction2.SelectedIndexChanged -= ComboFaction2_SelectedIndexChanged;
                    ComboFaction2.SelectedIndex = 0;
                    fac2 = -1;
                    ComboFaction2.SelectedIndexChanged += ComboFaction2_SelectedIndexChanged;
                    return;
                }

                ComboFaction1.SelectedIndexChanged -= ComboFaction1_SelectedIndexChanged;
                ComboFaction1.SelectedIndex = fac2;
                fac1 = fac2;
                ComboFaction1.SelectedIndexChanged += ComboFaction1_SelectedIndexChanged;
            }
            fac2 = ComboFaction1.SelectedIndex - 1;

            LoadGrid();
        }

        private void ComboRaceClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboRaceClass.SelectedIndex == -1)
            {
                return;
            }

            LoadGrid();
        }
    }
}
