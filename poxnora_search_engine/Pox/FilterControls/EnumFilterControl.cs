using poxnora_search_engine.Pox.Filters;
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
    public partial class EnumFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public EnumFilterControl()
        {
            InitializeComponent();
            foreach (var s in Enum.GetNames(typeof(Filters.EnumFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if (Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((Filters.EnumFilter)Filter_ref).FilterType);

                List<string> sorted_strings = ((EnumFilter)Filter_ref).Options_ref.AllowedStrings.ToList();
                sorted_strings.Sort();
                foreach (var s in sorted_strings)
                    FilterValue.Items.Add(s);
                FilterValue.SelectedIndex = sorted_strings.IndexOf(((Filters.EnumFilter)Filter_ref).RefValue);
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterType.SelectedIndex != -1)
                    ((Filters.EnumFilter)Filter_ref).FilterType = (Filters.EnumFilterType)(FilterType.SelectedIndex);

                UpdateFilterDescription();
            }
        }

        private void FilterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterValue.SelectedIndex != -1)
                {
                    ((Filters.EnumFilter)Filter_ref).RefValue = FilterValue.SelectedItem.ToString();
                }

                UpdateFilterDescription();
            }
        }
    }
}
