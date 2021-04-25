using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using poxnora_search_engine.Pox.Filters;

namespace poxnora_search_engine.Pox.FilterControls
{
    public delegate void OnFilterUpdate(BaseFilter f);
    public partial class BaseFilterControl : UserControl
    {
        protected BaseFilter Filter_ref;
        public OnFilterUpdate FilterUpdate_action;
        public BaseFilterControl()
        {
            InitializeComponent();
        }

        public void SetFilter(BaseFilter f)
        {
            Filter_ref = f;
            OnFilterSet();
        }

        protected void UpdateFilterDescription()
        {
            FilterUpdate_action(Filter_ref);
        }

        protected virtual void OnFilterSet()
        {
            if (Filter_ref != null)
            {
                LabelName.Text = Filter_ref.Name;
                FilterNegated.Checked = Filter_ref.NegateResult;
            }
            else
            {
                LabelName.Text = "UNKNOWN_FILTER";
                FilterNegated.Checked = false;
            }
        }

        private void FilterNegated_CheckedChanged(object sender, EventArgs e)
        {
            if (Filter_ref == null)
                return;

            Filter_ref.NegateResult = FilterNegated.Checked;
            UpdateFilterDescription();
        }
    }
}
