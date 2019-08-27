using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using NanoBatteryTest.Models;
using NanoBatteryTest.Resources;
using System.IO;
using NanoBatteryTest.Helpers;
using NLog.Targets;
using NLog;
using NLog.Config;

namespace NanoBatteryTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            
            Log = App.PrepareLogger("app");

            ArtWPFHelpers.Localization.ResxLocalizationProvider rlp = new ArtWPFHelpers.Localization.ResxLocalizationProvider(Strings.ResourceManager);
            ArtWPFHelpers.Localization.LocalizationManager.Instance.LocalizationProvider = rlp;

            rlp.AddCulture("ru-RU");
            rlp.AddCulture("en-US");
            
            ArtWPFHelpers.Localization.LocalizationManager.Instance.CultureChanged += OnCultureChanged;
            Telerik.Windows.Controls.StyleManager.ApplicationTheme = new Telerik.Windows.Controls.Office2016TouchTheme();


            MainModel = new MainViewModel();
            ParametersModel = new ProcessParameters();
            TestBanch = new BanchModel();

            
        }

        public static NLog.Logger PrepareLogger(string name)
        {
            FileTarget fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName(name);
            if (fileTarget == null)
            {
                fileTarget = new FileTarget(name);
                fileTarget.FileName = @"${basedir}/logs/app/${shortdate}_" + name + @".log";

                FileTarget template = (FileTarget)LogManager.Configuration.FindTargetByName("flog");
                if (template != null)
                {
                    fileTarget.Layout = template.Layout;
                }

                LogManager.Configuration.AddTarget(fileTarget);
                var rule = new LoggingRule(name, LogLevel.Debug, fileTarget) { Final = true };

                LogManager.Configuration.LoggingRules.Add(rule);
                LogManager.ReconfigExistingLoggers();
            }

            return LogManager.GetLogger(name);
        }

        static public NLog.Logger Log
        {
            get;
            private set;
        }
        public MainViewModel MainModel
        {
            get;
            private set;
        }

        public ProcessParameters ParametersModel
        {
            get;
            private set;
        }

        public BanchModel TestBanch
        {
            get;
            private set;
        }


        public static string GetExecutingDirectoryName()
        {
            //var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetAppSubDirectoryName(string subdir, bool splitByMonth)
        {
            string dir = App.GetExecutingDirectoryName();
            DateTime dt = DateTime.Now;
            string appSubDir = splitByMonth ? string.Format("{0}{1}\\{2}\\{3}\\", dir, subdir, dt.Year, dt.Month) : string.Format("{0}{1}\\", dir, subdir);

            Directory.CreateDirectory(appSubDir);
            return appSubDir;
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            LocalizeAll();
        }



        void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            try
            {
                Exception ex = args.Exception;

                string msg = string.Format(Localize("UnhandledExceptionMessage"), ex.Message);
                string title = "ERROR";

                MessageBox.Show(msg, title, MessageBoxButton.OK);
            }
            catch
            {
                // swallow silently... nothing we can do.
            }
        }

        public static string Localize(string key)
        {
            string str = key;
            try
            {
                str = (string)ArtWPFHelpers.Localization.LocalizationManager.Instance.Localize(key);
            }
            catch
            {
                str = key;
            }
            if (str == null)
                return string.Format("[{0}]", key);
            return str;
        }

        static private List<ArtWPFHelpers.Localization.ILocalizableObject> localizableObjects = new List<ArtWPFHelpers.Localization.ILocalizableObject>();

        static public void AddLocalizableObject(ArtWPFHelpers.Localization.ILocalizableObject lo)
        {
            localizableObjects.Add(lo);
        }
        static public void LocalizeAll()
        {
            foreach (ArtWPFHelpers.Localization.ILocalizableObject lo in localizableObjects)
                lo.Localize();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Info("Application started.");
            ParametersModel.Load();

            TestBanch.ConnectAll();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Info("Application closed.");
            ParametersModel.Save();
            
            TestBanch.DisconnectAll();
            base.OnExit(e);
        }
    }
}
