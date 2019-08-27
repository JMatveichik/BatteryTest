using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using System.Threading;
using System.Collections.ObjectModel;

using ArtAuto.Devices;
using ArtAuto.Devices.Various;
using ArtAuto.Devices.ADAM6000;
using System.IO;
using NanoBatteryTest.Helpers;
using ArtAuto.Converters;

namespace NanoBatteryTest.Models
{

    public class BanchModel : ViewModelBase
    {

        public enum ChannelsNumbers{

            SingleBatteryVoltageMeassure = 0,
            SingleBatteryCurrentMeassure = 1,

            BatteryWithCMNVoltageMeassure = 2,
            BatteryWithCMNCurrentMeassure = 3,


            SingleBatteryChargeRelay    = 0,
            SingleBatteryDischargeRelay = 1,

            BatteryWithCMNChargeRelay = 2,
            BatteryWithCMNDischargeRelay = 3
        }

        public BanchModel()
        {
        
            A01 = new Adam6017("A01", "Battery parameters measure", 1000);
            A02 = new Adam6050("A02", "Switch relay for charge and discharge", 1000);

            SingleBatterySet = new BatterySet(BatteryType.SingleBattery, A01, A02,
                                                            (int)ChannelsNumbers.SingleBatteryVoltageMeassure,
                                                            (int)ChannelsNumbers.SingleBatteryCurrentMeassure,
                                                            (int)ChannelsNumbers.SingleBatteryChargeRelay,
                                                            (int)ChannelsNumbers.SingleBatteryDischargeRelay);

            SingleBatterySet.VoltageCoverter = new LinearConverter(2.99, 0.0, "VCONV", "Конвертирование входного напряжения в напряжение батареии");
            SingleBatterySet.CurrentCoverter = new LinearConverter(0.605, 0.0, "CCONV", "Конвертирование входного напряжения в ток батареии");


            BatteryWithCNMSet = new BatterySet(BatteryType.BatteryUsingCNM, A01, A02,
                                                            (int)ChannelsNumbers.BatteryWithCMNVoltageMeassure,
                                                            (int)ChannelsNumbers.BatteryWithCMNCurrentMeassure,
                                                            (int)ChannelsNumbers.BatteryWithCMNChargeRelay,
                                                            (int)ChannelsNumbers.BatteryWithCMNDischargeRelay);

            BatteryWithCNMSet.VoltageCoverter = new LinearConverter(3.0, 0.0, "VCONV", "Конвертирование входного напряжения в напряжение батареии");
            BatteryWithCNMSet.CurrentCoverter = new LinearConverter(0.59, -0.0124, "CCONV", "Конвертирование входного напряжения в ток батареии");


            SingleBatterySet.StageStart  += ResultsModel.OnStageStarted;
            BatteryWithCNMSet.StageStart += ResultsModel.OnStageStarted;

            SingleBatterySet.StageComplete += ResultsModel.OnStageCompleted;
            BatteryWithCNMSet.StageComplete += ResultsModel.OnStageCompleted;

            SingleBatterySet.StageTick += ResultsModel.OnStageCompleted;
            BatteryWithCNMSet.StageTick += ResultsModel.OnStageCompleted;


            StopAnyWork = new ManualResetEvent(false);            
        }

        private void SingleBatterySet_StageComplete(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private NLog.Logger Log = NLog.LogManager.GetLogger("app");        

        public Adam6017 A01
        {
            get;
            private set;
        }

        public Adam6050 A02
        {
            get;
            private set;
        }

        public ResultsModel ResultsModel
        {
            get;
            private set;
        } =    new ResultsModel();

        public BatterySet SingleBatterySet
        {
            get;
            private set;
        }
        public BatterySet BatteryWithCNMSet
        {
            get;
            private set;
        }

        public void ConnectAll()
        {

            if (A01.Connect("192.168.10.121:502", 1000))
                A01.Start(1000);

            if (A02.Connect("192.168.10.111:502", 1000))
                A02.Start(1000);
        }

        public void DisconnectAll()
        {
            A01.Stop();
            A01.Disconnect();

            A02.Stop();
            A02.Disconnect();
            
        }

        public ManualResetEvent StopAnyWork
        {
            get;
            set;
        }
        

        private string stageInfo = App.Localize("WaitingUser");
        public string CurrentStageInfo
        {
            get
            {
                return stageInfo;
            }
            set
            {
                if (value == stageInfo)
                    return;

                stageInfo = value;
                OnPropertyChanged("CurrentStageInfo");
            }
        }

        public void Start()
        {
            int cycleNo = ResultsModel.Results.Count + 1;
            SingleBatterySet.Start(StopAnyWork, cycleNo);
            BatteryWithCNMSet.Start(StopAnyWork, cycleNo);
        }

        public void Stop()
        {
            StopAnyWork.Set();            
        }

    }

}
