using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResultsAnalizer
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if ( ofd.ShowDialog() == DialogResult.OK)
            {
                txtbFilePath.Text = ofd.FileName;
            }
        }
    }
}
