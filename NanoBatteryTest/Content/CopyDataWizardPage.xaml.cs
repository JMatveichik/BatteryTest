using Demineralizer.Models;
using Demineralizer.Selectors;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Demineralizer.Helpers;
using Demineralizer.Commands;

namespace Demineralizer.Content
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class CopyDataWizrdPage : UserControl
    {
        public CopyDataWizrdPage()
        {
            InitializeComponent();

            App app = App.Current as App;
            CopyDataFilesViewModel cdfvm = app.CopyDataFilesModel;
            
            this.DataContext = cdfvm;

            ExperimentDayTemplateSelector calendarDayTemplateSelector = Resources["ExpDayTemplateSelector"] as ExperimentDayTemplateSelector;
            calendarDayTemplateSelector.AvailableDates = cdfvm.ExperimentDates;

            if (cdfvm.ExperimentDates.Count != 0)
                calendar.SelectedDate = cdfvm.ExperimentDates.Last();
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = (CheckBox)sender;
            App app = App.Current as App;
            CopyDataFilesViewModel cdfvm = app.CopyDataFilesModel;

            if (chbx.Name == "allDataCheckBox")
            {

                cdfvm.CopyFullDataFiles = true;
                cdfvm.CopyLogFiles = true;
                cdfvm.CopyMeassureDataFiles = true;
                cdfvm.CopyShortDataFiles = true;
            }
            else
            {
                allDataCheckBox.IsChecked = cdfvm.CopyFullDataFiles && 
                                            cdfvm.CopyLogFiles && 
                                            cdfvm.CopyMeassureDataFiles &&
                                            cdfvm.CopyShortDataFiles;
            }            
        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            allDataCheckBox.IsChecked = false;
        }

        private void WizardPageChanged(object sender, Telerik.Windows.Controls.SelectedPageChangedEventArgs e)
        {
            App app = App.Current as App;
            CopyDataFilesViewModel cdfvm = app.CopyDataFilesModel;

            //если последняя страница
            if (e.NewPage == radWizard.WizardPages.Last() && e.OldPage != null)
            {
                cdfvm.MakeLogArchive(USBDrivesProvider.Instance.SelectedDisk);
            }
        }

        private void CancelWizard(object sender, Telerik.Windows.Controls.NavigationButtonsEventArgs e)
        {
            
            BanchCommands.Back.Execute(sender, null);
            radWizard.SelectedPageIndex = 0;
        }

        private void FinishWizard(object sender, Telerik.Windows.Controls.NavigationButtonsEventArgs e)
        {
            if (MessageBox.Show(App.Localize("USBEject"), "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string msg      = "";
                string title = App.Localize("USBOperation");

                if ( USBDrivesProvider.Instance.EjectSelected() )
                    msg = App.Localize("USBCanRemove");
                else
                    msg = App.Localize("USBCanotRemove");

                FacilityMessagesModel.ShowPopupAlert(title, msg);
            }

            BanchCommands.Back.Execute(sender, null);
            radWizard.SelectedPageIndex = 0;
        }
    }
}
