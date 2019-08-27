using System;
using System.Collections.Generic;
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
using NanoBatteryTest.Commands;
using NanoBatteryTest.Content;
using NanoBatteryTest.Models;
using System.Windows.Threading;

namespace NanoBatteryTest
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App app = ((App)App.Current);
            this.DataContext = app.MainModel;

            
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Start, StartExecuted, StartCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Stop, StopExecuted, StopCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Connect, ConnectExecuted, ConnectCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Back, BackExecuted, BackCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Exit, ExitExecuted, ExitCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Home, HomeExecuted, HomeCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Parameters, ParamsExecuted, ParamsCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Messages, MessagesExecuted, MessagesCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Dynamic, DynamicExecuted, DynamicCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.Save, SaveExecuted, SaveCanExecute));

            this.CommandBindings.Add(new CommandBinding(BanchCommands.RessetParameters, RessetParametersExecuted, RessetParametersCanExecute));
            this.CommandBindings.Add(new CommandBinding(BanchCommands.DemoParameters, DemoParametersExecuted, DemoParametersCanExecute));

            this.CommandBindings.Add(new CommandBinding(BanchCommands.ExtendedMode, ExtendedModeExecuted, ExtendedModeCanExecute));

            app.MainModel.AddContentPage("WELCOME", new WelcomePage());
            app.MainModel.AddContentPage("SCHEMA", new FacilitySchemaPage());
            app.MainModel.AddContentPage("PARAMETERS", new ProcessParametersPage());
            app.MainModel.AddContentPage("RESULTS", new ResultsPage());
            

            app.MainModel.ShowContentPage("WELCOME");

            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Tick += OnTimerTick;
            timer.Start();
        }


        private DispatcherTimer timer = new DispatcherTimer();


        void OnTimerTick(object sender, EventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.CurrentTime = DateTime.Now;
        }

        private void OnFacilityInitFalied(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainViewModel mvm = this.DataContext as MainViewModel;
                mvm.ShowContentPage("ERROR");
            });
        }

        private void OnFacilityInitComplete(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainViewModel mvm = this.DataContext as MainViewModel;
                mvm.ShowContentPage("CONFIRM");
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        #region Команды

        void RessetParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            App app = ((App)App.Current);
            app.ParametersModel.ResetToDefault();
        }

        void RessetParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        void DemoParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            App app = ((App)App.Current);
            app.ParametersModel.ResetToDemo();
        }

        void DemoParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        void StartExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            App app = ((App)App.Current);
            app.TestBanch.Start();
            
        }

        void StartCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void ExtendedModeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("SCHEMA");
        }

        void ExtendedModeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void MessagesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("MESSAGES");
        }

        void MessagesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.Back();
        }

        void BackCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        void ConnectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("SCHEMA");

        }

        void ConnectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void ParamsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("PARAMETERS");
        }

        void ParamsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void StopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(App.Localize("MsgStopCicle"), App.Localize("StopTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                App app = ((App)App.Current);
                app.TestBanch.Stop();                
            }
        }

        void StopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void ExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(App.Localize("MsgExitApplication"), App.Localize("ExitTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                App app = ((App)App.Current);
                app.TestBanch.Stop();
                Application.Current.Shutdown();
            }
                
        }

        void ExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void HomeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("WELCOME");
        }

        void HomeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void DynamicExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("DYNAMIC");
        }

        void DynamicCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {   
            MainViewModel mvm = this.DataContext as MainViewModel;
            mvm.ShowContentPage("RESULTS");

        }

        void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;//USBDrivesProvider.Instance.IsUSBDriveSelected;
        }


        #endregion
    }

}
