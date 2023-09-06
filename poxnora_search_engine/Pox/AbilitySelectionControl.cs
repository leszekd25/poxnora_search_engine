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
    public partial class AbilitySelectionControl : UserControl
    {
        public delegate void OnAbilitySelected(int up_index, int ab_index);

        public OnAbilitySelected AbilitySelected;

        public AbilitySelectionControl()
        {
            InitializeComponent();

            TimerCheckMouseOver.Start();
        }

        public void SetAbilities(List<Ability> abs)
        {
            Controls.Clear();

            Size = new Size(Width, 35 * Math.Max(1, abs.Count));

            for(int i = 0; i < abs.Count; i++)
            {
                AbilityControl abc = new AbilityControl();
                Controls.Add(abc);
                abc.Tag = i;
                abc.Location = new Point(0, i * 35);
                abc.SetAbility(abs[i]);
                abc.SetColor(Color.LightGray);
                abc.PictureBoxAbility.MouseDown += OnAbilityClicked;
                if(abs[i] != null)
                    Program.image_cache.AddAbilityImageSubscriber(abs[i].IconName, abc);
            }
        }

        public int GetAbilityID(int index)
        {
            return (int)(Controls[index].Tag);
        }

        public void SetSelectedAbilitiy(int index)
        {
            AbilityControl abc = (AbilityControl)(Controls[index]);
            abc.SetSelected(true);
            abc.SetColor(Color.White);
        }

        public void ClearAbilities()
        {
            foreach(var c in Controls)
            {
                Program.image_cache.RemoveAbilityImageSubscriber((AbilityControl)c);
                ((AbilityControl)c).PictureBoxAbility.MouseDown -= OnAbilityClicked;
            }
            Controls.Clear();
        }

        public void OnAbilityClicked(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            AbilityControl abc = (AbilityControl)(((PictureBox)sender).Parent);

            AbilitySelected?.Invoke((int)Tag, (int)(abc.Tag));
            if (Parent != null)
            {
                ClearAbilities();
                AbilitySelected = null;
                Parent.Controls.Remove(this);
            }
        }

        private void TimerCheckMouseOver_Tick(object sender, EventArgs e)
        {
            if(!ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                TimerCheckMouseOver.Stop();
                if (Parent != null)
                {
                    ClearAbilities();
                    AbilitySelected = null;
                    Parent.Controls.Remove(this);
                }
            }
        }
    }
}
