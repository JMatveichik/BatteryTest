using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArtWPFHelpers.Localization;
using System.IO;

namespace NanoBatteryTest.Models
{
    
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<CultureInfo> cultureInfos;
        private CultureInfo currentCulture;

        public MainViewModel()
        {
            
            Office2016TouchPalette.Palette.FontFamily = new System.Windows.Media.FontFamily("PF Din Text Pro");
            Office2016TouchPalette.Palette.FontSize = 16;
            Office2016TouchPalette.Palette.FontSizeL = 18;
            Office2016TouchPalette.Palette.FontSizeS = 14;

            
        }

        

        public IEnumerable<CultureInfo> CultureInfos
        {
            get
            {
                return cultureInfos ?? (cultureInfos = ArtWPFHelpers.Localization.LocalizationManager.Instance.Cultures);
            }
            set
            {
                if (Equals(value, cultureInfos))
                    return;

                cultureInfos = value;
                OnPropertyChanged("CultureInfos");
            }
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture ?? (currentCulture = ArtWPFHelpers.Localization.LocalizationManager.Instance.CurrentCulture);
            }
            set
            {
                if (Equals(value, currentCulture))
                    return;

                currentCulture = value;
                ArtWPFHelpers.Localization.LocalizationManager.Instance.CurrentCulture = value;
                OnPropertyChanged("CurrentCulture");

            }
        }

        public ThemePalette Palette
        {
            get
            {
                return Office2016TouchPalette.Palette;
            }
        }
        public string Version
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string ver = "v. " + assembly.GetName().Version.ToString();

                return ver;
            }
        }

        private UserControl currentContent = null;
        public UserControl CurrentContent
        {
            get {
                return currentContent;
            }
            set
            {
                if (currentContent == value)
                    return;

                currentContent = value;
                OnPropertyChanged("CurrentContent");
            }
        }

        private Dictionary<string, UserControl> contents = new Dictionary<string, UserControl>();

       
        public bool AddContentPage(string key, UserControl content)
        {
            if (contents.ContainsKey(key))
                return false;

            contents.Add(key, content);
            return true;
        }

        public void ShowContentPage(string key)
        {
            if (key == null)
                return;

            if (contents.ContainsKey(key))
            {
                prevContentID    = currentContentID;
                currentContentID = key;
                CurrentContent = contents[key];

                CommandManager.InvalidateRequerySuggested();
            }
            
        }
      

        public void ShowContentPages(int ms)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (string uc in contents.Keys)
                {
                    ShowContentPage(uc);
                    Thread.Sleep(ms);
                }
            });
        }



        private string currentContentID;
        private string prevContentID;

        public void Back()
        {
            ShowContentPage(prevContentID);            
        }

        private DateTime currentTime;

        public DateTime CurrentTime
        {
            get { return currentTime; }
            set
            {
                if (value != currentTime)
                {
                    currentTime = value;
                    OnPropertyChanged("CurrentTime");
                }
            }
        }



       
    }
}
