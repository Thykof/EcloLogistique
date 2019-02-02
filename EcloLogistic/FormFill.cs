using System.Globalization;
using System.Windows.Forms;

namespace EcloLogistic
{
    /// <summary>
    /// Form Fill.
    /// Need form validation only for Quantity.
    /// </summary>
    public partial class FormFill : Form
    {
        public double Quantity;
        public int BreedingTime
        {
            get { return (int) numericUpDownBT.Value; }
        }
        public int Clutch
        {
            get { return (int) numericUpDownClutch.Value; }
        }
        public int FeedFrequency
        {
            get { return (int)numericUpDownFF.Value; }
        }
        public string Feedback
        {
            get { return textBoxFeedback.Text; }
        }

        public FormFill()
        {
            InitializeComponent();
            Quantity = double.Parse(textBoxQuantity.Text);
        }

        private void textBoxQuantity_TextChanged(object sender, System.EventArgs e)
        {
            string s_quantity = textBoxQuantity.Text.Replace(".", ",");
            if (s_quantity != "")
            {
                buttonValidate.Enabled = double.TryParse(s_quantity, out Quantity);
            }
            else
            {
                buttonValidate.Enabled = false;
            }
        }
    }
}
