using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            LabelCurrentVersion.Text = "current version: " + Utility.APP_VERSION;

            LinkGithub.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LinkGithub_Clicked);
            LinkGithub.Links.Add(new LinkLabel.Link() { LinkData = "https://github.com/leszekd25/poxnora_search_engine" });
        }

        private void LinkGithub_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void AboutBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            LinkGithub.LinkClicked -= new LinkLabelLinkClickedEventHandler(this.LinkGithub_Clicked);
        }
    }
}
