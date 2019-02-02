using System.Windows.Forms;

namespace EcloLogistic
{
    public partial class FormNewTray : Form
    {
        public FormNewTray()
        {
            InitializeComponent();
        }
        
        public string id
        {
            get { return textBoxId.Text; }
        }
        public string Feedback
        {
            get { return textBoxFeedback.Text; }
        }

        private void textBoxId_TextChanged(object sender, System.EventArgs e)
        {
            buttonValidate.Enabled = (textBoxId.Text == "") ? false: true;
        }
    }
}
