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
            RunePreviewImage.Width = Width - 6;
            RunePreviewImage.Height = Height - 6 - 30;
            LabelText.Location = new Point(RunePreviewImage.Location.X, RunePreviewImage.Location.Y + RunePreviewImage.Height + 3);
            LabelText.Width = Width - 6;
            LabelText.Height = 30;
        }
    }
}
