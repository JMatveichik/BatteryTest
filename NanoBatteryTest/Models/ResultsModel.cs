using NanoBatteryTest.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace NanoBatteryTest.Models
{
    public class ResultsModel : ViewModelBase
    {

        public ResultsModel()
        {
            LoadLast("ToLoad2.csv");
        }
        
        ~ResultsModel()
        {
            Save("ToLoad2.csv");
        }
        
        

        public ObservableCollection<ResultsPoint> Results
        {
            get;
            private set;
        } = new ObservableCollection<ResultsPoint>();

        private object resLock = new object(); 
        public void OnStageStarted(object set, StageInfoEventArgs sea)
        {
            lock(resLock)
            {            
                BatterySet bat = (BatterySet)set;
                
                if (Results.Count == 0)
                {
                    Results.Add(new ResultsPoint() { CycleNo = bat.CurrentCycle });
                    return;
                }

                ResultsPoint point = Results.Last();
                if (point.CycleNo < bat.CurrentCycle)
                    Results.Add( new ResultsPoint() { CycleNo = bat.CurrentCycle } );
            }
        }

        public void LoadLast(string fn)
        {
            StreamReader sr = new StreamReader(fn);
            string line = sr.ReadLine();
            while(!sr.EndOfStream)
            {
                Results.Add(ResultsPoint.Load(sr));
            }
            sr.Close();
        } 
        public void Save(string fn)
        {
            StreamWriter sw = new StreamWriter(fn);
            string line = "CYCLE,CNM CHARGE,CNM DISCHARGE,NO CHARGE,NO DISCHARGE,CNM CHARGE DURATION,CNM DISCHARGE DURATION,NO CHARGE DURATION,NO DISCHARGE DURATION";
            sw.WriteLine(line);
            foreach (ResultsPoint rp in Results)
                rp.Save(sw);

            sw.Flush();
            sw.Close();
        }

      

        public void OnStageCompleted(object set, StageInfoEventArgs sea)
        {
            BatterySet bat = (BatterySet)set;
            ResultsPoint point = Results[bat.CurrentCycle - 1];

            ExperimentStageBase stage = sea.Stage;

            switch (bat.TypeOfBattery)
            {
                case BatteryType.SingleBattery:
                    {
                        if (stage is ChargeStage)
                        {
                            point.ChargeDurationSingle = stage.Duration;
                            point.ChargeVolumeSingle = stage.TotalCharge;
                        }

                        if (stage is DischargeStage)
                        {
                            point.DischargeDurationSingle = stage.Duration;
                            point.DischargeVolumeSingle = stage.TotalCharge;
                        }
                    }    
                    break;

                case BatteryType.BatteryUsingCNM:
                    {
                        if (stage is ChargeStage)
                        {
                            point.ChargeDurationCNM = stage.Duration;
                            point.ChargeVolumeCNM = stage.TotalCharge;
                        }

                        if (stage is DischargeStage)
                        {
                            point.DischargeDurationCNM = stage.Duration;
                            point.DischargeVolumeCNM = stage.TotalCharge;
                        }
                    }
                    break;

                default:
                    break;
            }

            Save("ToLoad2.csv");

        }
    }
}
