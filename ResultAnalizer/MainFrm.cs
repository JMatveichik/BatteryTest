using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResultAnalizer
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }


        ExperimentalData AllData = new ExperimentalData();

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if ( ofd.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = ofd.FileName;
                AllData.Load(ofd.FileName);

                List<string> allRes = new List<string>();
                foreach(StageData st in AllData.Stages)
                    allRes.AddRange(st.GetStageInfo());

                tbxResults.Lines = allRes.ToArray();

            }
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            fsd.ShowNewFolderButton = true;

            if (fsd.ShowDialog() == DialogResult.OK)
            {
                textDir.Text = fsd.SelectedPath;

                int stageNum = 1;

                foreach (StageData st in AllData.Stages)
                {
                    string fn = string.Format("{1:000}_{0}.csv", st.TypeOfStage, stageNum++); 
                    fn  = Path.Combine(fsd.SelectedPath, fn);
                    st.Save(fn);                    
                }
            }
        }
    }
}
