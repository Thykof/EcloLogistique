
using System.Windows.Forms;

namespace EcloLogistic
{
    public partial class About : Form
    {
        /// <summary>
        /// Define About dialog.
        /// </summary>
        public About()
        {
            InitializeComponent();
            linkLabel1.Links.Add(0, 30, "https://facebook.com/eclofood");
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkedLabel1Clicked);
            
            linkLabel2.Links.Add(0, 30, "mailto:nathan.seva@outlook.fr");
            linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkedLabel2Clicked);
        }

        private void LinkedLabel1Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://facebook.com/eclofood");
        }

        private void LinkedLabel2Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:nathan.seva@outlook.fr");
        }
    }
}
