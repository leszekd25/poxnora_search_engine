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
    public partial class PatchBuilderSaveForm : Form
    {
        public string PatchName = "";

        public PatchBuilderSaveForm()
        {
            InitializeComponent();
        }

        private void PatchBuilderSaveForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists("patches"))
                {
                    Directory.CreateDirectory("patches");
                }
                foreach (var path in Directory.EnumerateFiles("patches"))
                {
                    if (Path.GetExtension(path) != ".ptc")
                    {
                        continue;
                    }
                    ListboxExistingPatches.Items.Add(Path.GetFileNameWithoutExtension(path));
                }
            }
            catch(Exception ex)
            {
                return;
            }

            TextboxPatchName.Text = PatchName;
        }

        private void ListboxExistingPatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListboxExistingPatches.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            TextboxPatchName.Text = ListboxExistingPatches.Items[ListboxExistingPatches.SelectedIndex].ToString();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            PatchName = TextboxPatchName.Text.Trim();

            if (PatchName == "")
                return;

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
