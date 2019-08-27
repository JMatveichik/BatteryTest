using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace NanoBatteryTest.Helpers
{
    public class ResultsPoint : ViewModelBase
    {

        private int cycleNo;

        public int CycleNo
        {
            get { return cycleNo; }
            set
            {
                if (cycleNo == value)
                    return;

                cycleNo = value;
                OnPropertyChanged("CycleNo");
            }
        }

        public double ChargeVolumeCNM
        {
            get { return chargeVolCNM; }
            set
            {
                if (chargeVolCNM == value)
                    return;

                chargeVolCNM = value;
                OnPropertyChanged("ChargeVolCNM");
            }
        }
        private double chargeVolCNM = 0.0;

        public double DischargeVolumeCNM
        {
            get { return dischargeVolCNM; }
            set
            {
                if (dischargeVolCNM == value)
                    return;

                dischargeVolCNM = value;
                OnPropertyChanged("DischargeVolumeCNM");
            }
        }
        private double dischargeVolCNM = 0.0;

        public TimeSpan ChargeDurationCNM
        {
            get { return chargeDurationCNM; }
            set
            {
                if (chargeDurationCNM == value)
                    return;

                chargeDurationCNM = value;
                OnPropertyChanged("ChargeDurationCNM");
            }
        }
        private TimeSpan chargeDurationCNM = TimeSpan.FromSeconds(0.0);

        public TimeSpan DischargeDurationCNM
        {
            get { return dischargeDurationCNM; }
            set
            {
                if (dischargeDurationCNM == value)
                    return;

                dischargeDurationCNM = value;
                OnPropertyChanged("DischargeDurationCNM");
            }
        }
        private TimeSpan dischargeDurationCNM = TimeSpan.FromSeconds(0.0);

        public double ChargeVolumeSingle
        {
            get { return chargeVolSingle; }
            set
            {
                if (chargeVolSingle == value)
                    return;

                chargeVolSingle = value;
                OnPropertyChanged("ChargeVolSingle");
            }
        }
        private double chargeVolSingle = 0.0;

        public double DischargeVolumeSingle
        {
            get { return dischargeVolSingle; }
            set
            {
                if (dischargeVolSingle == value)
                    return;

                dischargeVolSingle = value;
                OnPropertyChanged("DischargeVolumeSingle");
            }
        }
        private double dischargeVolSingle = 0.0;

        public TimeSpan ChargeDurationSingle
        {
            get { return chargeDurationSingle; }
            set
            {
                if (chargeDurationSingle == value)
                    return;

                chargeDurationSingle = value;
                OnPropertyChanged("ChargeDurationSingle");
            }
        }
        private TimeSpan chargeDurationSingle = TimeSpan.FromSeconds(0.0);

        public TimeSpan DischargeDurationSingle
        {
            get { return dischargeDurationSingle; }
            set
            {
                if (dischargeDurationSingle == value)
                    return;

                dischargeDurationSingle = value;
                OnPropertyChanged("DischargeDurationSingle");
            }
        }
        private TimeSpan dischargeDurationSingle = TimeSpan.FromSeconds(0.0);


        public static ResultsPoint Load(StreamReader sr)
        {
            string data = sr.ReadLine();
            string[] parts = data.Split(';', ',');

            ResultsPoint pt = new ResultsPoint();

            try
            {
                pt.CycleNo = int.Parse(parts[0]);
                pt.ChargeVolumeCNM = double.Parse(parts[1]);
                pt.DischargeVolumeCNM = double.Parse(parts[2]);
                pt.ChargeVolumeSingle = double.Parse(parts[3]);
                pt.DischargeVolumeSingle = double.Parse(parts[4]);
                pt.ChargeDurationCNM = TimeSpan.FromMinutes(double.Parse(parts[5]));
                pt.DischargeDurationCNM = TimeSpan.FromMinutes(double.Parse(parts[6]));
                pt.ChargeDurationSingle = TimeSpan.FromMinutes(double.Parse(parts[7]));
                pt.DischargeDurationSingle = TimeSpan.FromMinutes(double.Parse(parts[8]));
            }
            catch (Exception ex)
            {
                return null;
            }
            return pt;
        } 
        
        public void Save  (StreamWriter sw)
        {
            string line = string.Format("{0},", CycleNo);
            line += string.Format("{0:F4},", ChargeVolumeCNM);
            line += string.Format("{0:F4},", DischargeVolumeCNM);
            line += string.Format("{0:F4},", ChargeVolumeSingle);
            line += string.Format("{0:F4},", DischargeVolumeSingle);
            line += string.Format("{0:F1},", ChargeDurationCNM.TotalMinutes);
            line += string.Format("{0:F1},", DischargeDurationCNM.TotalMinutes);
            line += string.Format("{0:F1},", ChargeDurationSingle.TotalMinutes);
            line += string.Format("{0:F1}", DischargeDurationSingle.TotalMinutes);
            sw.WriteLine(line);
        }
    }
}
