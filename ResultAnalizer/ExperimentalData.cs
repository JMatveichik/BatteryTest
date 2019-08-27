using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResultAnalizer
{
    public class MeassurePoint
    {
        public DateTime Time
        {
            get;
            set;
        }

        public double TotalMinutes
        {
            get;
            set;
        }

        public double BatteryVoltage
        {
            get;
            set;
        }

        public double BatteryCurrent
        {
            get;
            set;
        }

        public double PowerSourceVoltage
        {
            get;
            set;
        }

        public double PowerSourceCurrent
        {
            get;
            set;
        }

        static public MeassurePoint Load(StreamReader sr)
        {
            string[] vals = null;
            try
            {
                vals = sr.ReadLine().Split(',', ';');
            }
            catch (Exception e)
            {
                return null;
            }

            MeassurePoint mp = new MeassurePoint()
            {
                Time = DateTime.ParseExact(vals[0], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture),
                TotalMinutes = double.Parse(vals[1]),
                BatteryVoltage = double.Parse(vals[2]),
                BatteryCurrent = double.Parse(vals[3]),
                PowerSourceVoltage = double.Parse(vals[5]),
                PowerSourceCurrent = double.Parse(vals[7])
            };

            return mp;
        }

        public void Save(StreamWriter sw)
        {
            string line = string.Format("{0},{1},{2},{3},{4},{5}", Time.ToString("HH:mm:ss"), TotalMinutes, BatteryVoltage, BatteryCurrent, PowerSourceVoltage, PowerSourceCurrent);
            sw.WriteLine(line);
        }

    }

    public enum StageType
    {
        Charge = 0, 
        Discharge = 1, 
        Relax = 2
    }
    public class StageData : List<MeassurePoint>
    {    
        public void ProcessData()
        {
            if (this.Count == 0)
                return;            

            double Ips = 0.0;
            double Ibat = 0.0;
            
            for (int i = 0; i < 100; i ++)
            {
                MeassurePoint mp = this[i];
                Ips  += mp.PowerSourceCurrent;
                Ibat += mp.BatteryCurrent;
                Is += (mp.BatteryCurrent + mp.PowerSourceCurrent); 
            }

            Is /= 100;
            bool isChargeStage = (Ips > Ibat);

            if (Is < 0.05)
                TypeOfStage = StageType.Relax;
            else
                TypeOfStage = isChargeStage ? StageType.Charge : StageType.Discharge;


            TotalCharge = 0.0;
            MeassurePoint mpc = this.First();

            foreach (MeassurePoint mp in this)
            {
                double I = isChargeStage ? mp.PowerSourceCurrent : mp.BatteryCurrent;
                double dt = TimeSpan.FromMinutes(mp.TotalMinutes - mpc.TotalMinutes).TotalHours;

                mpc = mp;
                TotalCharge += I * dt;
            }

            Duration = TimeSpan.FromMinutes(this.Last().TotalMinutes);
        }

        public List<string> GetStageInfo()
        {
            List<string> info = new List<string>();
            info.Add("___________________________________________");
            info.Add(string.Format("{0:20} {1} {2:F5}", "STAGE ", TypeOfStage, Is));
            info.Add(string.Format("{0:20} {1:F1} minutes", "DURATION", Duration.TotalMinutes));
            info.Add(string.Format("{0:20} {1:F4} Ah", "CHARGE", TotalCharge));

            return info;
        }

        public void Save(string path)
        {
            StreamWriter sw = new StreamWriter(path);
            List<string> info = GetStageInfo();
            foreach (string line in info)
                sw.WriteLine(line);

            sw.WriteLine("Time, Minutes, UB, IB, UPS, IPS");
            foreach (MeassurePoint mp in this)
                mp.Save(sw);

            sw.Close();
        }

        private double Is = 0.0;
        public StageType TypeOfStage
        {
            get;
            private set;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        public double TotalCharge
        {
            get;
            private set;
        }
    }

    public class ExperimentalData
    {
        List<StageData> stagesData = new List<StageData>();

        public List<StageData> Stages
        {
            get { return stagesData; }
        }

        public bool Load(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            string line = sr.ReadLine();
            if (!line.Contains("Time"))
                return false;


            List<MeassurePoint> allData = new List<MeassurePoint>();

            while (!sr.EndOfStream)
            {
                MeassurePoint mp = MeassurePoint.Load(sr);
                allData.Add(mp);
            }


            List<MeassurePoint> startPoints = (from mp in allData where mp.TotalMinutes  == 0 select mp).ToList();

            ///split stages
            StageData currentStage = null;
            foreach (MeassurePoint mp in allData)
            {
                if (startPoints.Contains(mp))
                {
                    currentStage = new StageData();
                    stagesData.Add(currentStage);
                }

                currentStage.Add(mp);
            }


            foreach (StageData stage in stagesData)
                stage.ProcessData();


            return true;
        }

    }
}
