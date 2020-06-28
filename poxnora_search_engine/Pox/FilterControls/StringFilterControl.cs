using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox.FilterControls
{
    public partial class StringFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public StringFilterControl()
        {
            InitializeComponent();
            foreach (var s in Enum.GetNames(typeof(Filters.StringFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if (Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((Filters.StringFilter)Filter_ref).FilterType);
                FilterValue.Text = ((Filters.StringFilter)Filter_ref).RefValue;
                IgnoreCase.Checked = ((Filters.StringFilter)Filter_ref).IgnoreCase;
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterType.SelectedIndex != -1)
                    ((Filters.StringFilter)Filter_ref).FilterType = (Filters.StringFilterType)(FilterType.SelectedIndex);

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
                ((Filters.StringFilter)Filter_ref).RefValue = FilterValue.Text;
                UpdateFilterDescription();
            }
        }

        private void IgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            if(Filter_ref != null)
            {
                ((Filters.StringFilter)Filter_ref).IgnoreCase = IgnoreCase.Checked;
                UpdateFilterDescription();
            }
        }
    }
}
