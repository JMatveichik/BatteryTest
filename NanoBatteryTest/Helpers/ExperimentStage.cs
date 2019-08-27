using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using ArtAuto.Common;
using NanoBatteryTest.Models;
using System.Threading.Tasks;
using System.Threading;

namespace NanoBatteryTest.Helpers
{
    public enum StageResult
    {
        Successful = 0,

        Failed = 1,

        Timeout = 2,

        Breaked = 3
    }

    public abstract class ExperimentStageBase : TimedControl 
    {
        

        public ExperimentStageBase(BatterySet battery, string name, string description, TimeSpan interval) : base(name, description, interval)
        {
            ExperimentalData = new ObservableCollection<MeassurePoint>();
            Battery = battery;
        }


        public ObservableCollection<MeassurePoint> ExperimentalData
        {
            get;
            private set;
        }

        public BatterySet Battery
        {
            get;
            private set;
        }

        
        /// <summary>
        /// Событие окончания стадии
        /// </summary>
        public event EventHandler Tick;

        

        /// <summary>
        /// Событие ошибки выполнения шага стадии
        /// </summary>
        public event EventHandler Fail;

        private MeassurePoint MakeMeassurePoint()
        {
            return new MeassurePoint()
            {
                Time = DateTime.Now,
                Current = Battery.ActualCurrent,
                Voltage = Battery.ActualVoltage                
            };
        }

        public void ClearData()
        {
            App.Current.Dispatcher.Invoke(() => ExperimentalData.Clear());
        }

        protected override void ControlProcedure()
        {
           
            MeassurePoint mp = MakeMeassurePoint();

            mp.TotalMinutes = (DateTime.Now - StartTime).TotalMinutes;
            TotalCharge += CalcStepCharge(mp);

            App.Current.Dispatcher.Invoke( () => ExperimentalData.Add(mp));

            mp.Save(temporaryStream);
        }

        protected double CalcStepCharge(MeassurePoint mp)
        {
            if (ExperimentalData.Count == 0)
                return 0.0;

            MeassurePoint mpl = null;

            lock (collectionLock)
            {
                mpl = ExperimentalData.Last();
            }

            double stepCharge = 0.0;

            if (mpl != null)
            {
                TimeSpan dt = mp.Time - mpl.Time;
                stepCharge = dt.TotalHours * Math.Abs(mp.Current);
            }

            return stepCharge;
        }

        protected const uint MaxRetries = 5;

        public DateTime StartTime
        {
            get;
            protected set;

        }
        public TimeSpan Duration
        {
            get;
            protected set;
        }

        public double TotalCharge
        {
            get
            {
                return charge;
            }
            protected set
            {
                if (value == charge)
                    return;

                charge = value;
                OnPropertyChanged("TotalCharge");
            }
        }
        private double charge = 0.0;

        
        /// <summary>
        /// Событие окончания стадии
        /// </summary>
        //public event StageEventHandler StageTick;


        protected object collectionLock = new object();

        protected NLog.Logger Log = NLog.LogManager.GetLogger("app");

        
        /// <summary>
        /// Подготовка к проведению стадии
        /// </summary>
        /// <returns></returns>
        abstract protected bool Prepare();


        /// <summary>
        /// Окончание стадии
        /// </summary>
        /// <returns></returns>
        abstract protected bool Finalize();

        /// <summary>
        /// Проверка условий завершения
        /// </summary>
        /// <returns></returns>
        abstract protected bool CheckForCompletion();

        /// <summary>
        /// Проверка таймаута стадии
        /// </summary>
        /// <returns></returns>
        abstract protected bool CheckForTimeout();

        /// <summary>
        /// Проверка ошибки стадии
        /// </summary>
        /// <returns></returns>
        abstract protected bool CheckForFailure();

        /// <summary>
        /// Выполнение стадии
        /// </summary>
        /// <param name="stop"></param>
        /// <returns></returns>
        public StageResult DoStage(ManualResetEvent stop)
        {
            
            Log.Info("Stage {0} started in {1} cycle", Name, Battery.CurrentCycle);


            if (! Prepare() )
                return StageResult.Failed;


            string tfn = Path.GetTempFileName();
            temporaryStream = new StreamWriter( tfn );
            Log.Info("Stage temporary file : {0}", tfn);

            Start();
           

            StartTime = DateTime.Now;
            OnPropertyChanged("StartTime");

            StageResult sr = StageResult.Failed;
            while (true)
            {
               
                if (CheckForCompletion())
                {
                    sr = StageResult.Successful;
                    break;
                }                    

                if (stop.WaitOne(10))
                {
                    sr = StageResult.Breaked;
                    break;
                }                    

                if ( CheckForTimeout() )
                {
                    sr = StageResult.Timeout;                    
                    break;
                }
                    

                if (CheckForFailure())
                {
                    sr = StageResult.Failed;
                    break;
                }
                    
                Thread.Sleep(1000);
                Duration = DateTime.Now - StartTime;

                if (Tick != null)
                    Tick(this, EventArgs.Empty);

                OnPropertyChanged("Duration");
            }

            Stop();

            if ( !Finalize() )
                return StageResult.Failed;

            return sr;
        }

        private StreamWriter temporaryStream = null;

        public void Save(string fn)
        {

            if (ExperimentalData.Count == 0)
                return;

            StreamWriter sr = new StreamWriter(fn);

            MeassurePoint mpf = ExperimentalData.First();
            MeassurePoint mpl = ExperimentalData.Last();

            TimeSpan dur = mpl.Time - mpf.Time;

            sr.WriteLine("{0:20} : {1}", "DESCRIPTION",  Description);
            sr.WriteLine("{0:20} : {1} at {2}", "STARTED",     mpf.Time.ToShortDateString(), mpf.Time.ToShortTimeString());
            sr.WriteLine("{0:20} : {1} at {2}", "COMPLETED",   mpl.Time.ToShortDateString(), mpl.Time.ToShortTimeString());
            sr.WriteLine("{0:20} : {1} ({2} minutes)", "DURATION",  dur.ToString(@"hh\:mm\:ss"), dur.TotalMinutes );
            sr.WriteLine("{0:20} : {1:F3} Ah", "TOTAL CHARGE", TotalCharge);


            sr.WriteLine("TIME, MINUTES, IBAT, UBAT");

            foreach (MeassurePoint mp in ExperimentalData)
                mp.Save(sr);

            sr.Flush();
            sr.Close();            
        }
    }

    public class ChargeStage : ExperimentStageBase
    {
        public ChargeStage(BatterySet battery, string name, string description, TimeSpan interval) : base(battery, name, description, interval)
        {
            
        }

        

        private double deltaCurrent = double.NaN;

        public double DeltaCurrent
        {
            get { return deltaCurrent; }
            set
            {
                if (deltaCurrent == value)
                    return;

                deltaCurrent = value;
                OnPropertyChanged("DeltaCurrent");
            }
        }
        
        private bool checkCurrentStability()
        {
            ProcessParameters par = ((App)App.Current).ParametersModel;
            
            DeltaCurrent = double.NaN;

            if (Duration < par.CurrentChargeStability)
                return false;

            ObservableCollection<MeassurePoint> ed = ExperimentalData;

            double Imax = ed.Select(i => i.Current).Max();

            if (Battery.ActualCurrent > Imax * 0.9)
                return false;

            lock (collectionLock)
            {
                DateTime checkPoint = DateTime.Now - par.CurrentChargeStability;
                TimeSpan dataTime = ed.Last().Time - ed.First().Time;

                if (dataTime < par.CurrentChargeStability)
                    return false;

                var Ips = (from mp in ed where mp.Time > checkPoint select mp.Current);
                DeltaCurrent = Math.Abs(Ips.Max()) - Math.Abs(Ips.Min());
            }

            double deltaBase = Imax * par.CurrentPercentLimit / 100.0;

            if (DeltaCurrent < deltaBase)
                return true;

            return false;
        }

        protected override bool CheckForCompletion()
        {
           /*
            if (Duration < TimeSpan.FromMinutes(1.0))
                return false;
            else
                return true;
                */


            ProcessParameters par = ((App)App.Current).ParametersModel;
            
            if (Battery.ActualVoltage >= par.HighBatteryVoltageLimit)
            {
                try
                {
                    if ( checkCurrentStability() )
                    {
                        Log.Info("{0} => Stage was interrupted with current stabilization : I={1:F3} A  dI={2:F5}A", Battery.TypeOfBattery, Battery.ActualCurrent, DeltaCurrent);                        
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("{0} => Check current stability failed : {1}", Battery.TypeOfBattery, e.Message);
                    return false;
                }
            }
                        
            return false;
        }

        protected override bool CheckForFailure()
        {
            return false;
        }

        protected override bool CheckForTimeout()
        {
            ProcessParameters par = ((App)App.Current).ParametersModel;

            if (Duration >= par.ChargeTimeout)
            {
                Log.Error("{0} => {1} stage timeout : {2}", Battery.TypeOfBattery, Name, Duration.ToString());
                return true;
            }                

            return false;
        }
     

        protected override bool Finalize()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage finalization retrie {2}...", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToRelax())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }

        protected override bool Prepare()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage preparation retrie {2}", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToCharge())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }
    }

    public class DischargeStage : ExperimentStageBase
    {
        public DischargeStage(BatterySet battery, string name, string description, TimeSpan interval) : base(battery, name, description, interval)
        {
            
        }       

        protected override bool CheckForCompletion() 
        {
            /*if (Duration < TimeSpan.FromMinutes(1.0))
                return false;
            else
                return true;
                */

            ProcessParameters par = ((App)App.Current).ParametersModel;

            if (Battery.ActualVoltage <= par.LowBatteryVoltageLimit)
            {
                Log.Info("{0} => {1} stage was interrupted with voltage limit : U={2:F2}V Umin={3:F2}V ", Battery.TypeOfBattery, Name, Battery.ActualVoltage, par.LowBatteryVoltageLimit);                
                return true;
            }
            
            return false;
        }

        protected override bool CheckForFailure()
        {
            return false;
        }

        protected override bool CheckForTimeout()
        {
            return false;
        }

        protected override bool Finalize()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage finalization retrie {2}...", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToRelax())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }

        protected override bool Prepare()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage preparation retrie {2}", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToDischarge())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }
    }

    public class RelaxStage : ExperimentStageBase
    {
        public RelaxStage(BatterySet battery, string name, string description, TimeSpan interval) : base(battery, name, description, interval)
        {
            
        }

        
        protected override bool CheckForCompletion()
        {
            /*
             * if (Duration < TimeSpan.FromMinutes(1.0))
                return false;
            else
                return true;
                */

            ProcessParameters par = ((App)App.Current).ParametersModel;
            if (Duration >= par.AfterChargeRelaxation)
                return true;               

            return false;
        }

        protected override bool CheckForFailure()
        {
            return false;
        }

        protected override bool CheckForTimeout()
        {
            return false;
        }

        protected override bool Finalize()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage finalization retrie {2}...", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToRelax())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }

        protected override bool Prepare()
        {
            int r = 0;
            do
            {
                Log.Info("{0} => {1} stage preparation retrie {2}", Battery.TypeOfBattery, Name, ++r);

                if (Battery.PrepareToRelax())
                    return true;

                Thread.Sleep(1000);

            } while (r < MaxRetries);

            return false;
        }
    }


}
