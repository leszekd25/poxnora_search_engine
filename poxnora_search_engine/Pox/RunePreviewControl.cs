using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine.Pox
{
    public partial class RunePreviewControl : UserControl, IImageCacheSubscriber
    {
        public int ElemID = 0;
        public bool MiniMode = false;

        public RunePreviewControl()
        {
            InitializeComponent();
        }

        public void OnImageLoad(Bitmap b)
        {
            if (b == null)
                RunePreviewImage.Image = RunePreviewImage.ErrorImage;
            else
                RunePreviewImage.Image = b;
        }

        private void RunePreviewControl_Resize(object sender, EventArgs e)
        {
            if (MiniMode)
            {
                RunePreviewImage.Width = this.Width - 6;
                RunePreviewImage.Height = this.Height - 6;
                RunePreviewImage.Location = new Point(3, 3);
                LabelText.Hide();
            }
            else
            {
                RunePreviewImage.Width = Width - 6;
                RunePreviewImage.Height = Height - 6 - 30;
                RunePreviewImage.Location = new Point((this.Width - RunePreviewImage.Width) / 2, 3);
                LabelText.Show();
                LabelText.Width = Width - 6;
                LabelText.Height = 30;
                LabelText.Location = new Point((this.Width - LabelText.Width) / 2, RunePreviewImage.Location.Y + RunePreviewImage.Height + 3);
            }
        }

        public void SetMiniMode(bool mini_mode)
        {
            if(MiniMode == mini_mode)
            {
                return;
            }

            MiniMode = mini_mode;

            RunePreviewControl_Resize(null, null);
        }

        public void SetBGColor(Color bg_color)
        {
            BackColor = bg_color;
        }
    }
}
