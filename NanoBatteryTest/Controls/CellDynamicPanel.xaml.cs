using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace NanoBatteryTest.Controls
{
    /// <summary>
    /// Interaction logic for DynamicCurrentPage.xaml
    /// </summary>
    public partial class CellDynamicPanel : UserControl
    {
        public CellDynamicPanel()
        {
            InitializeComponent();
//             App a = (App)App.Current;
//             this.DataContext = a.ExperimentModel;
        }

        private void OnCurrentPlotTrackUpdated(object sender, TrackBallInfoEventArgs e)
        {
           if (e.Context.DataPointInfos.Count == 2)
            {
                CategoricalDataPoint dp = e.Context.DataPointInfos[0].DataPoint as CategoricalDataPoint;                
                this.txtTime.Text = ((DateTime)dp.Category).ToString("HH:mm:ss");
                this.txtBatCurrent.Text = ((double)dp.Value).ToString("F3");

                dp = e.Context.DataPointInfos[1].DataPoint as CategoricalDataPoint;
                this.txtVoltage.Text = ((double)dp.Value).ToString("F2");

            }            
        }

       
    }
}
