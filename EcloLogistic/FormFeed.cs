using System;
using System.Windows.Forms;

namespace EcloLogistic
{
    public partial class FormFeed : Form
    {
        // Properties
        public double RealQantity;
        public string Quantity
        {
            set { label1Quantity.Text = value; }
        }
        public string FeedBack
        {
            get { return textBoxFeedback.Text; }
        }

        // Construcor
        public FormFeed()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Setter
        /// </summary>
        /// <param name="tray_id"></param>
        public void SetTrayId(string tray_id)
        {
            labelDescription.Text += " " + tray_id;
        }

        // Events
        private void textBoxRealQuantity_TextChanged(object sender, EventArgs e)
        {
            string s_quantity = textBoxRealQuantity.Text.Replace(".", ",");
            if(s_quantity != "")
            {
                buttonValidate.Enabled = double.TryParse(s_quantity, out RealQantity);
            }
        }
    }
}
