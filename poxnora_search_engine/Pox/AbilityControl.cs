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
    public partial class AbilityControl : UserControl, IImageCacheSubscriber
    {
        public int AbilityID;
        public AbilityControl()
        {
            InitializeComponent();
        }

        public void SetAbility(Ability a)
        {
            if (a == null)
            {
                AbilityID = Utility.NO_INDEX;
                LabelAbilityName.Text = "No ability";
                LabelAbilityNoraCost.Text = "";
                PictureBoxAbility.BorderStyle = BorderStyle.None;
            }
            else
            {
                AbilityID = a.ID;
                LabelAbilityName.Text = a.ToString();
                LabelAbilityNoraCost.Text = string.Format("{0} nora", a.NoraCost);
                if ((a.ActivationType == 3) || (a.Cooldown > 0) || (a.APCost > 0))
                {
                    PictureBoxAbility.BorderStyle = BorderStyle.FixedSingle;
                    LabelAbilityNoraCost.Text = string.Format("{0} nora         {1} AP         CD {2}", a.NoraCost, a.APCost, a.Cooldown);
                }
                else
                {
                    PictureBoxAbility.BorderStyle = BorderStyle.None;
                    LabelAbilityNoraCost.Text = string.Format("{0} nora", a.NoraCost);
                }
            }
        }

        public void SetSelected(bool selected)
        {
            if (selected)
                LabelAbilityName.Font = new Font(LabelAbilityName.Font, FontStyle.Bold);
            else
                LabelAbilityName.Font = new Font(LabelAbilityName.Font, FontStyle.Regular);
        }

        public void SetColor(Color c)
        {
            LabelAbilityName.ForeColor = c;
            LabelAbilityNoraCost.ForeColor = c;
        }

        public void OnImageLoad(Bitmap img)
        {
            PictureBoxAbility.Image = img;
        }
    }
}
