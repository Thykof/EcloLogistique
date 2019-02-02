using System;
using System.Windows.Forms;

namespace EcloLogistic
{
    public partial class FormCollect : Form
    {
        /// <summary>
        /// Number of insects when collecting.
        /// </summary>
        public int ProductivityUnit;

        /// <summary>
        /// total lot's weight.
        /// </summary>
        public double ProductivityWeight;

        /// <summary>
        /// Comment about the task.
        /// </summary>
        public string Feedback
        {
            get { return textBoxFeedback.Text; }
        }

        /// <summary>
        /// Form constructor.
        /// </summary>
        public FormCollect()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Validate ProductivityUnit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        /// <summary>
        /// Validate ProductivityWeight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            button1.Enabled = Helpers.ValidateCollectForm(textBox1.Text, textBox2.Text, out ProductivityWeight, out ProductivityUnit);
        }
    }
}
