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
            AbilityID = a.ID;
            LabelAbilityName.Text = a.ToString();
            LabelAbilityNoraCost.Text = string.Format("{0} nora", a.NoraCost);
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
