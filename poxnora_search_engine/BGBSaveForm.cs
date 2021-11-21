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
    public partial class BGBSaveForm : Form
    {
        public struct BGCodeSort : IEquatable<BGCodeSort>, IComparable<BGCodeSort>
        {
            public string BGName;
            public string BGCode;

            public bool Equals(BGCodeSort other)
            {
                return (BGName == other.BGName);
            }

            public int CompareTo(BGCodeSort comparePart)
            {
                return BGName.CompareTo(comparePart.BGName);
            }
        }

        List<BGCodeSort> BGEntries = new List<BGCodeSort>();

        public string SelectedName = "";
        public string SelectedCode = "";

        public BGBSaveForm()
        {
            InitializeComponent();
        }

        private void BGBSaveForm_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (var str in File.ReadAllLines("BG codes.txt"))
                {
                    string[] split_str = str.Split(' ');
                    if (split_str.Length != 2)
                    {
                        continue;
                    }

                    BGEntries.Add(new BGCodeSort() { BGName = split_str[0], BGCode = split_str[1] });
                }
            }
            catch (Exception ex)
            {
                return;
            }

            BGEntries.Sort();

            foreach (var bge in BGEntries)
            {
                ListboxExistingBGs.Items.Add(bge.BGName);
            }

            TextboxBGName.Text = SelectedName;
        }

        private void ListboxExistingBGs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListboxExistingBGs.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            TextboxBGName.Text = BGEntries[ListboxExistingBGs.SelectedIndex].BGName;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (SelectedName == "")
                return;

            SelectedName = TextboxBGName.Text;
            SelectedName = SelectedName.Replace(' ', '_');

            BGCodeSort bge = new BGCodeSort() { BGName = SelectedName, BGCode = SelectedCode };
            bool bg_exists = false;
            for (int i = 0; i < BGEntries.Count; i++)
            {
                if (BGEntries[i].BGName == SelectedName)
                {
                    BGEntries[i] = bge;
                    bg_exists = true;
                    break;
                }
            }
            if (!bg_exists)
            {
                BGEntries.Add(bge);
                BGEntries.Sort();
            }

            try
            {
                List<string> lines = new List<string>();
                foreach (var bge2 in BGEntries)
                    lines.Add(bge2.BGName + " " + bge2.BGCode);

                File.WriteAllLines("BG codes.txt", lines.ToArray());
            }
            catch (Exception ex)
            {

            }

            this.Close();
        }
    }
}
