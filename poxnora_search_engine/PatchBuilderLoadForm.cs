using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace poxnora_search_engine
{
    public partial class PatchBuilderLoadForm : Form
    {
        public string patch_to_load = "";

        public PatchBuilderLoadForm()
        {
            InitializeComponent();
        }

        private void ButtonDeletePatch_Click(object sender, EventArgs e)
        {
            if(ListPatches.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            try
            {
                File.Delete(ListPatches.Items[ListPatches.SelectedIndex].ToString() + ".ptc");
                ListPatches.Items.RemoveAt(ListPatches.SelectedIndex);
            }
            catch(Exception ex)
            {
                return;
            }
        }

        private void ButtonLoadPatch_Click(object sender, EventArgs e)
        {
            if (ListPatches.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            patch_to_load = "patches\\" + ListPatches.Items[ListPatches.SelectedIndex].ToString() + ".ptc";
            if(!File.Exists(patch_to_load))
            {
                patch_to_load = "";
                return;
            }

            Close();
        }

        private void PatchBuilderLoadForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists("patches"))
                {
                    Directory.CreateDirectory("patches");
                }
                foreach (var path in Directory.EnumerateFiles("patches"))
                {
                    if(Path.GetExtension(path) != ".ptc")
                    {
                        continue;
                    }
                    ListPatches.Items.Add(Path.GetFileNameWithoutExtension(path));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
