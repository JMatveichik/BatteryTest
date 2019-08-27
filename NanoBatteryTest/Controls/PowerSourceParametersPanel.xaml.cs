using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NanoBatteryTest.Models;

namespace NanoBatteryTest.Controls
{
    /// <summary>
    /// Interaction logic for BatteryParametersPanel.xaml
    /// </summary>
    public partial class PowerSourceParametersPanel : UserControl
    {
        public PowerSourceParametersPanel()
        {
            InitializeComponent();
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            BanchModel mvm = (BanchModel)DataContext;
            mvm.ApplyValuesToPowerSource();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            BanchModel mvm = (BanchModel)DataContext;
            mvm.TogglePowerSourceState();
        }
    }
}
