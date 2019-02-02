using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EcloLogistic
{
    public partial class FormNewLot : Form
    {
        public FormNewLot()
        {
            InitializeComponent();
        }
        
        public string Clutch
        {
            get { return textBox1.Text; }
        }

        public string FeedFrequency
        {
            get { return textBox2.Text; }
        }

    }
}
