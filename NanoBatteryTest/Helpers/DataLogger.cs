using System;
using System.IO;
using System.Threading;
using NanoBatteryTest.Models;

namespace NanoBatteryTest.Helpers
{
    public abstract class DataLoggerBase
    {
        public DataLoggerBase(string pref)
        {
            //Подготовка выходного файла
            prepareOutStream(pref);

            //формирование заголовка
            makeHeader();

            //интервал записи по умолчанию
            WriteInterval = TimeSpan.FromSeconds(10.0);
        }

        //private static DataLogger inst = null;
        //public static DataLogger Instance => inst ?? (inst = new DataLogger());

        protected StreamWriter OutputStream
        {
            get;
            set;
        }

        public TimeSpan WriteInterval
        {
            get;
            set;
        }


        private Thread collectThread = null;

        private ManualResetEvent stopCollectEvent = new ManualResetEvent(false);

        private void collectFunction()
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                TimeSpan t = DateTime.Now - start;

                //Делаем запись
                makeRecord(t.TotalMinutes);

                DateTime recTime = DateTime.Now;

                //ожидаем заданный интервал времени
                TimeSpan tsCur = TimeSpan.FromMilliseconds(0);
                while (tsCur < WriteInterval)
                {
                    Thread.Sleep(50);
                    tsCur = DateTime.Now - recTime;

                    //проверяем событие остановки сбора данных
                    if (stopCollectEvent.WaitOne(50))
                        break;
                }

                //проверяем событие остановки сбора данных
                if (stopCollectEvent.WaitOne(50))
                    break;

            }
        }

        public void Start()
        {
            collectThread = new Thread(new ThreadStart(collectFunction));
            collectThread.Start();
        }

        public void Stop()
        {
            stopCollectEvent.Set();

            OutputStream.Flush();

            Thread.Sleep(500);
            stopCollectEvent.Reset();
        }

        public void Start(TimeSpan interval)
        {
            WriteInterval = interval;
            Start();
        }

        private void prepareOutStream(string pref)
        {
            DateTime dt = DateTime.Now;
            string logdir = App.GetAppSubDirectoryName("data");
            string fn = string.Format("{0}{1}_{2}.csv", logdir, dt.ToString("ddHHmm"), pref);
            FileStream fs = File.Open(fn, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            OutputStream = new StreamWriter(fs);
        }

        protected abstract void makeHeader();

        protected abstract void makeRecord(double minutes);
    }

    public class FullDataLogger : DataLoggerBase
    {
        private FullDataLogger() : base("data")
        {

        }

        private static FullDataLogger inst = null;
        public static FullDataLogger Instance => inst ?? (inst = new FullDataLogger());

        protected override void makeHeader()
        {
            string header = "Time; Minutes;";

            header += "UBAT;";  // banch.BatteryVoltage;
            header += "IBAT;";  // banch.RinsingFilter1;
            header += "UPSS;";  // banch.RinsingFilterIsClosed;
            header += "UPSG;";  // banch.RinsingFilter2;            
            header += "IPSS;";  // banch.RinsingFilterIsOpen;
            header += "IPSG;";  // banch.RinsingFilterIsOpen;
            header += "PSW;";   // banch.RinsingFilterIsOpen;


            OutputStream.WriteLine(header);

        }

        protected override void makeRecord(double minutes)
        {
            string rec = DateTime.Now.ToString("HH:mm:ss");
            BanchModel banch = ((App)(App.Current)).TestBanch;

            rec += string.Format(";{0:F5}", minutes);

            rec += string.Format(";{0:F3}", banch.BatteryVoltage);
            rec += string.Format(";{0:F3}", banch.BatteryCurrent);
            rec += string.Format(";{0:F3}", banch.PowerSourceVoltageSet);
            rec += string.Format(";{0:F3}", banch.PowerSourceVoltageGet);
            rec += string.Format(";{0:F3}", banch.PowerSourceCurrentSet);
            rec += string.Format(";{0:F3}", banch.PowerSourceCurrentGet);
            rec += string.Format(";{0}",    banch.PowerSourceWorkMode ? 1 : 0);

            OutputStream.WriteLine(rec);
            OutputStream.Flush();

        }
    }
  

}
