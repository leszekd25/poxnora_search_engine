using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox
{
    public partial class QuickFilterForm : Form
    {
        public List<string> AllowedFactions = new List<string>();
        public bool NegateFactions = false;
        public List<string> AllowedClasses = new List<string>();
        public bool NegateClasses = false;
        public List<string> AllowedRaces = new List<string>();
        public bool NegateRaces = false;
        public List<string> AllowedRarities = new List<string>();
        public bool NegateRarities = false;
        public List<string> AllowedExpansions = new List<string>();
        public bool NegateExpansions = false;

        public QuickFilterForm()
        {
            InitializeComponent();
        }

        public void FillFilterLists()
        {
            if (!Program.database.ready)
                return;

            foreach (var faction in Program.database.Factions.GetStringsSortedDefault())
                ListFactions.Items.Add(faction, true);

            foreach (var cl in Program.database.Classes.GetStringsSortedDefault())
                ListClasses.Items.Add(cl, true);

            foreach (var race in Program.database.Races.GetStringsSortedDefault())
                ListRaces.Items.Add(race, true);

            foreach (var rarity in Program.database.Rarities.GetStringsSortedDefault())
                ListRarities.Items.Add(rarity, true);

            foreach (var expansion in Program.database.Expansions.GetStringsSortedDefault())
                ListExpansions.Items.Add(expansion, true);
        }

        private void QuickFilterForm_Load(object sender, EventArgs e)
        {
            FillFilterLists();
        }

        private void ClearFactions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListFactions.Items.Count; i++)
                ListFactions.SetItemChecked(i, false);
        }

        private void ToggleFactions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListFactions.Items.Count; i++)
                ListFactions.SetItemChecked(i, !ListFactions.GetItemChecked(i));
        }

        private void ClearClasses_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListClasses.Items.Count; i++)
                ListClasses.SetItemChecked(i, false);
        }

        private void ToggleClasses_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListClasses.Items.Count; i++)
                ListClasses.SetItemChecked(i, !ListClasses.GetItemChecked(i));
        }

        private void ClearRaces_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListRaces.Items.Count; i++)
                ListRaces.SetItemChecked(i, false);
        }

        private void ToggleRaces_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListRaces.Items.Count; i++)
                ListRaces.SetItemChecked(i, !ListRaces.GetItemChecked(i));
        }

        private void ClearRarities_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListRarities.Items.Count; i++)
                ListRarities.SetItemChecked(i, false);
        }

        private void ToggleRarities_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListRarities.Items.Count; i++)
                ListRarities.SetItemChecked(i, !ListRarities.GetItemChecked(i));
        }

        private void ClearExpansions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListExpansions.Items.Count; i++)
                ListExpansions.SetItemChecked(i, false);
        }

        private void ToggleExpansions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListExpansions.Items.Count; i++)
                ListExpansions.SetItemChecked(i, !ListExpansions.GetItemChecked(i));
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            bool negate = false;

            // factions
            negate = ((ListFactions.CheckedItems.Count * 2) > ListFactions.Items.Count);
            for (int i = 0; i < ListFactions.Items.Count; i++)
                if (ListFactions.GetItemChecked(i) != negate)
                    AllowedFactions.Add(ListFactions.Items[i].ToString());
            NegateFactions = negate;


            // classes
            negate = ((ListClasses.CheckedItems.Count * 2) > ListClasses.Items.Count);
            for (int i = 0; i < ListClasses.Items.Count; i++)
                if (ListClasses.GetItemChecked(i) != negate)
                    AllowedClasses.Add(ListClasses.Items[i].ToString());
            NegateClasses = negate;


            // races
            negate = ((ListRaces.CheckedItems.Count * 2) > ListRaces.Items.Count);
            for (int i = 0; i < ListRaces.Items.Count; i++)
                if (ListRaces.GetItemChecked(i) != negate)
                    AllowedRaces.Add(ListRaces.Items[i].ToString());
            NegateRaces = negate;


            // rarities
            negate = ((ListRarities.CheckedItems.Count * 2) > ListRarities.Items.Count);
            for (int i = 0; i < ListRarities.Items.Count; i++)
                if (ListRarities.GetItemChecked(i) != negate)
                    AllowedRarities.Add(ListRarities.Items[i].ToString());
            NegateRarities = negate;


            // expansions
            negate = ((ListExpansions.CheckedItems.Count * 2) > ListExpansions.Items.Count);
            for (int i = 0; i < ListExpansions.Items.Count; i++)
                if (ListExpansions.GetItemChecked(i) != negate)
                    AllowedExpansions.Add(ListExpansions.Items[i].ToString());
            NegateExpansions = negate;



            Close();
        }
    }
}
