using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox.FilterControls
{
    public partial class BooleanFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public BooleanFilterControl()
        {
            InitializeComponent();
            foreach (var s in Enum.GetNames(typeof(Filters.BooleanFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if (Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((Filters.BooleanFilter)Filter_ref).FilterType);
                FilterValue.SelectedIndex = ((Filters.BooleanFilter)Filter_ref).RefValue ? 1 : 0;
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterType.SelectedIndex != -1)
                    ((Filters.BooleanFilter)Filter_ref).FilterType = (Filters.BooleanFilterType)(FilterType.SelectedIndex);

                UpdateFilterDescription();
            }
        }

        private void FilterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterValue.SelectedIndex != -1)
                {
                    ((Filters.BooleanFilter)Filter_ref).RefValue = (FilterValue.SelectedIndex == 1);
                }

                UpdateFilterDescription();
            }
        }
    }
}
