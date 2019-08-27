using ArtAuto.Converters;
using ArtAuto.Devices;
using ArtAuto.Devices.ADAM6000;
using ArtWPFHelpers.Converters;
using NanoBatteryTest.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace NanoBatteryTest.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BatteryType
    {
        [Description("Etalon")]
        SingleBattery   = 0,

        [Description("Modified")]
        BatteryUsingCNM = 1
    }

    public class StageInfoEventArgs : EventArgs
    {

        public StageInfoEventArgs(ExperimentStageBase s)
        {
            Stage = s;
        }

        public ExperimentStageBase Stage
        {
            get;
            private set;
        }


    }

    public class BatterySet : ViewModelBase
    {

       
        /// <summary>
        /// Конструктор объектов испытуемой батареии
        /// </summary>
        /// <param name="A01">Устройство измерения напряжения и тока батареии</param>
        /// <param name="A02">Устройство переключения реле заряда/разряда</param>
        /// <param name="aiVoltageCahnnelNo">Номер аналогового входа для измерения напряжения</param>
        /// <param name="aiCurrentCahnnelNo">Номер аналогового входа для измерения тока</param>
        /// <param name="doChargeRelayCahnnelNo">Номер дискретного выхода для переключения реле заряда</param>
        /// <param name="doDischargeRelayChannelNo">Номер дискретного выхода для переключения реле разряда</param>
        public BatterySet(BatteryType batteryType, Adam6017 A01, Adam6050 A02, int aiVoltageCahnnelNo, int aiCurrentCahnnelNo, int doChargeRelayCahnnelNo, int doDischargeRelayChannelNo)
        {
            this.A01 = A01;
            this.A02 = A02;

            A01.DataReady += OnDataReady;
            A02.DataReady += OnDataReady;

            TypeOfBattery = batteryType;

            VoltageChannelNo = aiVoltageCahnnelNo;
            CurrentCahnnelNo = aiCurrentCahnnelNo;
            

            ChargeRelayChannelNo = doChargeRelayCahnnelNo;
            DischargeRelayChannelNo = doDischargeRelayChannelNo;
        }

        private NLog.Logger Log = NLog.LogManager.GetLogger("app");
        
        public LinearConverter VoltageCoverter { get; set; } = null;

        public LinearConverter CurrentCoverter { get; set; } = null;


        private void OnStageTick(object obj, EventArgs a)
        {
            if (StageTick != null)
                StageTick(this, new StageInfoEventArgs(obj as ExperimentStageBase));
        }

        public delegate void StageEventHandler(object bs, StageInfoEventArgs sea);

        /// <summary>
        /// Событие начала стадии
        /// </summary>
        public event StageEventHandler StageStart;

        /// <summary>
        /// Событие окончания стадии
        /// </summary>
        public event StageEventHandler StageComplete;


        /// <summary>
        /// Событие окончания стадии
        /// </summary>
        public event StageEventHandler StageTick;

        /// <summary>
        /// Событие таймаут выполнения шага стадии
        /// </summary>
        public event StageEventHandler StageTimeout;


        /// <summary>
        /// Событие ошибки выполнения шага стадии
        /// </summary>
        public event StageEventHandler StageFail;


        public BatteryType TypeOfBattery
        {
            get;
            private set;
        }

        /// <summary>
        /// Номер аналогового входа для измерения напряжения
        /// </summary>
        public int  VoltageChannelNo
        {
            get { return aiVoltageChannelNo; }
            private set
            {
                if (aiVoltageChannelNo == value)
                    return;

                aiVoltageChannelNo = value;
                OnPropertyChanged("VoltageChannelNo");
            }
        }
        private int aiVoltageChannelNo;


        /// <summary>
        /// Номер аналогового входа для измерения тока
        /// </summary>
        public int CurrentCahnnelNo
        {
            get { return aiCurrentCahnnelNo; }
            private set
            {
                if (aiCurrentCahnnelNo == value)
                    return;

                aiCurrentCahnnelNo = value;
                OnPropertyChanged("CurrentCahnnelNo");
            }
        }
        private int aiCurrentCahnnelNo;


        /// <summary>
        /// Номер дискретного входа для переключения реле заряда
        /// </summary>
        public int ChargeRelayChannelNo
        {
            get { return doChargeRelayCahnnelNo; }
            private set
            {
                if (doChargeRelayCahnnelNo == value)
                    return;

                doChargeRelayCahnnelNo = value;
                OnPropertyChanged("ChargeRelayChannelNo");
            }
        }
        private int doChargeRelayCahnnelNo;


        /// <summary>
        /// Номер дискретного выхода для переключения реле разряда
        /// </summary>
        public int DischargeRelayChannelNo
        {
            get { return doDischargeRelayChannelNo; }
            private set
            {
                if (doDischargeRelayChannelNo == value)
                    return;

                doDischargeRelayChannelNo = value;
                OnPropertyChanged("DischargeRelayChannelNo");
            }
        }
        private int doDischargeRelayChannelNo;


        /// <summary>
        /// Текущее напряжение испытываемой батареи
        /// </summary>
        public double ActualVoltage
        {
            get { return actualVoltage; }
            private set
            {
                if (actualVoltage == value)
                    return;

                actualVoltage = value;
                OnPropertyChanged("ActualVoltage");
            }
        }
        private double actualVoltage;

        /// <summary>
        /// Текущий ток испытываемой батареи
        /// </summary>
        public double ActualCurrent
        {
            get { return actualCurrent; }
            private set
            {
                if (actualCurrent == value)
                    return;

                actualCurrent = value;
                OnPropertyChanged("ActualCurrent");
            }
        }
        private double actualCurrent;

        /// <summary>
        /// Состояние реле заряда
        /// </summary>
        public bool ChargeRelayState
        {
            get { return chargeRelayState; }
            private set
            {
                if (chargeRelayState == value)
                    return;

                chargeRelayState = value;
                OnPropertyChanged("ChargeRelayState");
            }
        }
        private bool chargeRelayState;

        /// <summary>
        /// Состояние реле разряда
        /// </summary>
        public bool DischargeRelayState
        {
            get { return dischargeRelayState; }
            private set
            {
                if (dischargeRelayState == value)
                    return;

                dischargeRelayState = value;
                OnPropertyChanged("DischaregeRelayState");
            }
        }
        private bool dischargeRelayState;


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


        private void OnDataReady(object sender, EventArgs e)
        {
            BaseDevice dev = (BaseDevice)sender;
            if (dev == A01)
            {
                ActualVoltage = VoltageCoverter.Convert( A01.AnalogInputs[VoltageChannelNo].Value );
                ActualCurrent = CurrentCoverter.Convert( A01.AnalogInputs[CurrentCahnnelNo].Value );                
            }

            if (dev == A02)
            {
                ChargeRelayState    = A02.DiscreteOutputs[ChargeRelayChannelNo].IsEnabled;
                DischargeRelayState = A02.DiscreteOutputs[DischargeRelayChannelNo].IsEnabled;
            }
        }

        /// <summary>
        /// Перекалибровка датчика тока 
        /// </summary>
        /// <param name="maxDelta">Максимально допустимое отклонение при изменении корректировочного коэффициента</param>
        /// <returns></returns>
        private bool CorrectCurrentDependence(double maxDelta)
        {
            double B0 = CurrentCoverter.B;
            double k0 = CurrentCoverter.K;

            //напряжение на входе измерения тока
            double U = A01.AnalogInputs[aiCurrentCahnnelNo].Value;

            double B1 = -k0 * U;

            if (Math.Abs(B0 - B1) < maxDelta)
            {
                CurrentCoverter.K = k0;
                CurrentCoverter.B = B1;
                return true;
            }

            return false;
        }


        private bool prepareStand(bool? chargeRelay, bool? dischargeRelay)
        {
            try
            {
                //РЕЛЕ ЗАРЯДА
                if (chargeRelay.HasValue)
                {
                    Log.Info("Turn {0} charge relay", (bool)chargeRelay ? "ON" : "OFF");
                    A02.SetDiscreteOutput(ChargeRelayChannelNo, (bool)chargeRelay);
                    Thread.Sleep(100);
                }

                //РЕЛЕ РАЗРЯДА
                if (dischargeRelay.HasValue)
                {

                    Log.Info("Turn {0} discharge relay", (bool)dischargeRelay ? "ON" : "OFF");
                     A02.SetDiscreteOutput(DischargeRelayChannelNo, (bool)dischargeRelay);
                    Thread.Sleep(100);
                }
                
            }
            catch (Exception e)
            {
                Log.Error(e, "{0} => preparation failure : {0}", TypeOfBattery, e.Message);
                return false;
            }

            Log.Info("Stand preparation complete.");
            return true;
        }

        public bool PrepareToCharge()
        {
            try
            {
                Log.Info("{0} => prepare to charge", TypeOfBattery);

                if (!prepareStand(true, false))
                    return false;
            }
            catch(DeviceIOException de)
            {
                Log.Error(de, "{0} => preparation to charge failed : {1}", TypeOfBattery, de.Message);
                return false;
            }
            return true;
        }

        public bool PrepareToDischarge()
        {
            try
            {
                Log.Info("{0} => prepare to discharge", TypeOfBattery);

                if (!prepareStand(false, true))
                    return false;
            }
            catch (DeviceIOException de)
            {
                Log.Error(de, "{0} => preparation to discharge failed : {1}", TypeOfBattery, de.Message);
                return false;
            }
            return true;
        }

        public bool PrepareToRelax()
        {
            try
            {
                Log.Info("{0} => prepare to relaxation", TypeOfBattery);

                if (!prepareStand(false, false))
                    return false;
            }
            catch (DeviceIOException de)
            {
                Log.Error(de, "{0} => preparation to relaxation failed : {1}", TypeOfBattery, de.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Текущий цикл
        /// </summary>
        public int CurrentCycle
        {
            get
            {
                return currentCycle;
            }
            private set
            {
                if (currentCycle == value)
                    return;

                currentCycle = value;
                OnPropertyChanged("CurrentCycle");
            }
        }
        private int currentCycle = 0;


        /// <summary>
        /// Текущая стадия
        /// </summary>
        public ExperimentStageBase CurrentStage
        {
            get
            {
                return currentStage;
            }
            private set
            {
                if (value == currentStage)
                    return;

                currentStage = value;
                OnPropertyChanged("CurrentStage");
            }
        }
        private ExperimentStageBase currentStage = null;

        private Thread workThread = null;

        /// <summary>
        /// 
        /// </summary>
        private ManualResetEvent StopAnyWork
        {
            get;
            set;
        }


        /// <summary>
        /// Запуск рабочего потока
        /// </summary>
        public void Start(ManualResetEvent stop, int startFromCycleNo)
        {
            StopAnyWork = stop;

            CurrentCycle = startFromCycleNo;

            workThread = new Thread(new ThreadStart(ExperimentFunction));
            workThread.Start();
        }

        public void Stop()
        {
            Log.Info("Stopping experiment ...");
            prepareStand(false, false);
            
        }

        public bool IsChargeMode
        {
            get { return isChargeMode; }
            set
            {
                if (isChargeMode == value)
                    return;

                isChargeMode = value;
                OnPropertyChanged("IsChargeMode");
            }
        }
        private bool isChargeMode;

        public bool IsDischargeMode
        {
            get { return isDischargeMode; }
            set
            {
                if (isDischargeMode == value)
                    return;

                isDischargeMode = value;
                OnPropertyChanged("IsDischargeMode");
            }
        }
        private bool isDischargeMode;

        private void ExperimentFunction()
        {
            ProcessParameters par = ((App)App.Current).ParametersModel;

            int lastCycleNum = CurrentCycle + par.CyclesCount;

            string testDetails = (TypeOfBattery == BatteryType.BatteryUsingCNM) ? "BATTERY WITH CNM" : "STANDART BATTERY";
            
            ///ЦИКЛ ИСПЫТАНИЙ
            do
            {
                bool cycleSuccessful = false;


                List<ExperimentStageBase> cyclestages = new List<ExperimentStageBase>();

                //начинаем с заряда или с разряда
                if (par.StartFromCharge)
                {
                    cyclestages.Add(new ChargeStage(this, "charge", testDetails, TimeSpan.FromSeconds(5.0)));
                    cyclestages.Add(new RelaxStage(this, "relax", testDetails, TimeSpan.FromSeconds(5.0)));
                    cyclestages.Add(new DischargeStage(this, "discharge", testDetails, TimeSpan.FromSeconds(5.0)));
                }
                else
                {
                    cyclestages.Add(new DischargeStage(this, "discharge", testDetails, TimeSpan.FromSeconds(5.0)));
                    cyclestages.Add(new ChargeStage(this, "charge", testDetails, TimeSpan.FromSeconds(5.0)));
                    cyclestages.Add(new RelaxStage(this, "relax", testDetails, TimeSpan.FromSeconds(5.0)));
                }

                //выполняем стадии цикла
                cycleSuccessful = doStages(cyclestages);

                if (!cycleSuccessful)
                    break;

                Log.Info("{0} : EXPERIMENT CYCLE {1} WAS COMPLETE.", TypeOfBattery, CurrentCycle);

                CurrentCycle++;

            } while (CurrentCycle <= lastCycleNum);

            //заряжаем, если установлен флаг зарядки после окончания циклов
            if (par.ChargeWhenComplete)
            {
                List<ExperimentStageBase> lastCharge = new List<ExperimentStageBase>()
                {
                    new ChargeStage(this, "last charge", testDetails, TimeSpan.FromSeconds(5.0))
                };

                doStages(lastCharge);
            }
            
        }

        private bool doStages(IEnumerable<ExperimentStageBase> stages)
        {
            string batabrr = (TypeOfBattery == BatteryType.BatteryUsingCNM) ? "modified" : "etalon";

            bool cycleSuccessful = false;

            ///последовательно выполняем стадии эксперимента
            foreach (ExperimentStageBase s in stages)
            {

                CurrentStage = s;

                Task<StageResult> t = Task.Factory.StartNew<StageResult>(() => s.DoStage(StopAnyWork));

                if (StageStart != null)
                    StageStart(this, new StageInfoEventArgs(s));

                t.Wait();

                string dir = App.GetAppSubDirectoryName("data", false);
                string fn = string.Format("{0}{1}_{2}_{3}.csv", dir, currentCycle.ToString("D3"), s.Name, batabrr);

                s.Save(fn);

                switch (t.Result)
                {
                    case StageResult.Successful:
                        {
                            Log.Info("{0} : Stage {1} was complete successfully", TypeOfBattery, s.Name);
                            cycleSuccessful = true;

                            if (StageComplete != null)
                                StageComplete(this, new StageInfoEventArgs(s));

                            break;
                        }

                    case StageResult.Failed:
                        {
                            Log.Info("{0} : Stage {1} was failed", TypeOfBattery, s.Name);
                            cycleSuccessful = false;

                            if (StageFail != null)
                                StageFail(this, new StageInfoEventArgs(s));

                            break;
                        }

                    case StageResult.Timeout:
                        {
                            Log.Info("{0} : Stage {1} duration timeout", TypeOfBattery, s.Name);
                            cycleSuccessful = false;

                            if (StageTimeout != null)
                                StageTimeout(this, new StageInfoEventArgs(s));

                            break;
                        }

                    case StageResult.Breaked:
                        {
                            Log.Info("{0} : Stage {1} was interrupted by operator", TypeOfBattery, s.Name);
                            cycleSuccessful = false;
                            break;
                        }
                }

                if (!cycleSuccessful)
                    break;
            }

            return cycleSuccessful;
        }
    }

}
