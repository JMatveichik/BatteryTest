using System;
using System.Windows.Controls;

namespace NanoBatteryTest.Content
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : UserControl
    {
        public WelcomePage()
        {
            InitializeComponent();
            this.DataContext = ((App)App.Current).MainModel;
        }
    }
}
