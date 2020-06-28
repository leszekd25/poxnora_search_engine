using poxnora_search_engine.Pox.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox.FilterControls
{
    public partial class IntFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public IntFilterControl()
        {
            InitializeComponent();

            foreach (var s in Enum.GetNames(typeof(IntFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if(Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((IntFilter)Filter_ref).FilterType);
                FilterValue.Text = ((IntFilter)Filter_ref).RefValue.ToString();
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Filter_ref != null)
            {
                if(FilterType.SelectedIndex != -1)
                    ((IntFilter)Filter_ref).FilterType = (IntFilterType)(FilterType.SelectedIndex);
                
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
            if(Filter_ref != null)
            {
                int refval;
                if (int.TryParse(FilterValue.Text, out refval))
                    ((IntFilter)Filter_ref).RefValue = refval;
                else
                    FilterValue.Text = ((IntFilter)Filter_ref).RefValue.ToString();

                UpdateFilterDescription();
            }
        }
    }
}
