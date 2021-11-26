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
    public partial class AbilityControl_Histogram : UserControl, IImageCacheSubscriber
    {
        public AbilityControl_Histogram()
        {
            InitializeComponent();
        }

        public void OnImageLoad(Bitmap bmp)
        {
            PictureBoxAbility.Image = bmp;
        }
    }
}
