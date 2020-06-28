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
    public partial class EnumListFilterControl : poxnora_search_engine.Pox.FilterControls.BaseFilterControl
    {
        public EnumListFilterControl()
        {
            InitializeComponent();
            foreach (var s in Enum.GetNames(typeof(Filters.EnumListFilterType)))
                FilterType.Items.Add(s);
        }

        protected override void OnFilterSet()
        {
            base.OnFilterSet();
            if (Filter_ref != null)
            {
                FilterType.SelectedIndex = (int)(((Filters.EnumListFilter)Filter_ref).FilterType);

                List<string> sorted_strings = ((Filters.EnumListFilter)Filter_ref).Options_ref.AllowedStrings.ToList();
                sorted_strings.Sort();
                foreach (var s in sorted_strings)
                    FilterValue.Items.Add(s);
                FilterValue.SelectedIndex = sorted_strings.IndexOf(((Filters.EnumListFilter)Filter_ref).RefValue);
            }
        }

        private void FilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterType.SelectedIndex != -1)
                    ((Filters.EnumListFilter)Filter_ref).FilterType = (Filters.EnumListFilterType)(FilterType.SelectedIndex);

                UpdateFilterDescription();
            }
        }

        private void FilterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter_ref != null)
            {
                if (FilterValue.SelectedIndex != -1)
                {
                    ((Filters.EnumListFilter)Filter_ref).RefValue = FilterValue.SelectedItem.ToString();
                }

                UpdateFilterDescription();
            }
        }
    }
}
