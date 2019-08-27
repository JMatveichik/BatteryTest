using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoBatteryTest.Helpers
{
    public class MeassurePoint
    {
        public DateTime Time
        {
            get;
            set;
        }

        public double  TotalMinutes
        {
            get;
            set;
        }

        public double Voltage
        {
            get;
            set;
        }
       
        public double Current
        {
            get;
            set;
        }
        

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("{0},{1:F8},{2:F3},{3:F3}", Time.ToString("HH:mm:ss"), TotalMinutes, Current, Voltage);
        }

        static MeassurePoint Load(StreamReader sr)
        {
            string[] vals = null;
            try
            { 
              vals = sr.ReadLine().Split(',', ';');
            }
            catch(Exception e)
            {
                return null;
            }

            MeassurePoint mp = new MeassurePoint()
            {
                Time = DateTime.ParseExact(vals[0], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture),
                TotalMinutes = double.Parse(vals[1]),
                Current = double.Parse(vals[2]),
                Voltage = double.Parse(vals[3])
            };

            return mp;
        }
    }
}
