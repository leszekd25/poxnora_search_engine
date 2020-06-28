using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox.FilterControls
{
    public partial class AbilityListFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public AbilityListFilterControl()
        {
            InitializeComponent();
            foreach (var s in Enum.GetNames(typeof(Filters.AbilityListFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if (Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((Filters.AbilityListFilter)Filter_ref).FilterType);
                FilterValue.Text = Program.database.Abilities[((Filters.AbilityListFilter)Filter_ref).RefValue].ToString();

                AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
                acsc.AddRange(Program.database.AbilityNames.AllowedStrings.ToArray());
                FilterValue.AutoCompleteCustomSource = acsc;
                FilterValue.AutoCompleteMode = AutoCompleteMode.Suggest;
                FilterValue.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterType.SelectedIndex != -1)
                    ((Filters.AbilityListFilter)Filter_ref).FilterType = (Filters.AbilityListFilterType)(FilterType.SelectedIndex);

                UpdateFilterDescription();
            }
        }

        private void FilterValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Invalidate();
        }

        private void FilterValue_Validated(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                ((Filters.AbilityListFilter)Filter_ref).RefValue = Program.database.GetAbilityIDByName(FilterValue.Text);
                UpdateFilterDescription();
            }
        }
    }
}
