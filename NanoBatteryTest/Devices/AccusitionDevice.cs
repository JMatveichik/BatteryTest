using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoBatteryTest.Devices
{

    public class ChannelsInfo
    {

        public int AnalogInputsCount
        {
            get; set;
        }
        public int AnalogOutputsCount
        {
            get; set;
        }
        public int DiscreteInputsCount
        {
            get; set;
        }
        public int DiscreteOutputsCount
        {
            get; set;
        }

        public int CounterInputsCount
        {
            get; set;
        }
    }

    public class DiscreteDataEventArgs : EventArgs
    {
        public DiscreteDataEventArgs(BitArray data)
        {
            this.Data = new BitArray(data);
        }

        public BitArray Data
        {
            get; private set;
        }
    }

    public class AnalogDataEventArgs : EventArgs
    {
        public AnalogDataEventArgs(List<double> data)
        {
            this.Data = new List<double>(data);

        }

        public AnalogDataEventArgs(double [] data)
        {
            this.Data = new List<double>(data);           
        }

        public List<double> Data
        {
            get;
            private set;
        }
    }

    public class CounterDataEventArgs : EventArgs
    {
        public CounterDataEventArgs(List<int> data)
        {
            this.Data = new List<int>(data);

        }

        public CounterDataEventArgs(int[] data)
        {
            this.Data = new List<int>(data);
        }

        public List<int> Data
        {
            get;
            private set;
        }
    }

    public class StringEventsArgs : EventArgs
    {
        public StringEventsArgs(string msg)
        {
            this.Message = msg;
        }
        public string Message
        {
            get;
            private set;
        }
    }
    public abstract class AccusitionDeviceBase
    {

        public AccusitionDeviceBase(string name, ChannelsInfo ci)
        {
            Name = name;

            if (ci.AnalogInputsCount > 0)
                AnalogInputs = new List<double>(new double[ci.AnalogInputsCount]);

            if (ci.AnalogOutputsCount > 0)
                AnalogOutputs = new List<double>(new double[ci.AnalogOutputsCount]);

            if (ci.DiscreteInputsCount > 0)
                DiscreteInputs = new BitArray (ci.DiscreteInputsCount);

            if (ci.DiscreteOutputsCount > 0)
                DiscreteOutputs = new BitArray(ci.DiscreteOutputsCount);

            if (ci.CounterInputsCount > 0)
                CounterInputs = new List<int>(new int[ci.CounterInputsCount]);

        }

        public abstract bool Connect(string connection, int timeout);

        public BitArray DiscreteInputs
        {
            get;
            private set;
        }
        protected BitArray DiscreteOutputs
        {
            get;
            private set;
        }
        protected List<double> AnalogInputs
        {
            get;
            private set;
        }
        protected List<double> AnalogOutputs
        {
            get;
            private set;
        }

        protected List<int> CounterInputs
        {
            get;
            private set;
        }


        /// <summary>
        /// Обработчик окончания сбора данных
        /// <summary>
        public event EventHandler<DiscreteDataEventArgs> DiscreteInputsUpdate;

        public event EventHandler<DiscreteDataEventArgs> DiscreteOutputUpdate;

        public event EventHandler<AnalogDataEventArgs> AnalogInputsUpdate;

        public event EventHandler<AnalogDataEventArgs> AnalogOutputUpdate;

        public event EventHandler<CounterDataEventArgs> CounterInputsUpdate;


        public string  Name
        {
            get;
            private set;

        }
        public AccusitionDeviceBase()
        {
            Interval = BaseDelay;
        }


        public bool Start(int interval = 0)
        {
            if (controlThread != null && controlThread.IsAlive)
                return false;

            controlThread = new Thread(new ThreadStart(ControlThreadFunction));
            controlThread.Start();
            Interval = interval;
            return true;
        }

        public void Stop()
        {
            stopEvent.Set();
        }

        public bool IsAlive
        {
            get
            {
                if (controlThread == null)
                    return false;

                return controlThread.IsAlive;
            }
        }

        protected virtual void OnExit()
        {
            Debug.WriteLine("Device {0} was stoped !", Name);
        }

        public virtual void UpdateData()
        {

        }

        protected void OnUpdateDI(BitArray dinp)
        {
            if (DiscreteInputsUpdate != null)
                DiscreteInputsUpdate(this, new DiscreteDataEventArgs(dinp));
        }
                

        protected void OnUpdateDO(BitArray dout)
        {
            if (DiscreteOutputUpdate != null)
                DiscreteOutputUpdate(this, new DiscreteDataEventArgs(dout));
        }

        protected void OnUpdateAI(double[] ainp)
        {
            if (AnalogInputsUpdate != null)
                AnalogInputsUpdate(this, new AnalogDataEventArgs(ainp));
        }

        protected void OnUpdateAO(double[] aout)
        {
            if (AnalogOutputUpdate != null)
                AnalogOutputUpdate(this, new AnalogDataEventArgs(aout));
        }

        protected void OnUpdateCounter(int[] count)
        {
            if (CounterInputsUpdate != null)
                CounterInputsUpdate(this, new CounterDataEventArgs(count));
        }

        private void ControlThreadFunction()
        {
            while (true)
            {
                Thread.Sleep(Interval);
                UpdateData();

                if (stopEvent.WaitOne(TimeSpan.FromMilliseconds(10)))
                {
                    OnExit();
                    break;
                }
            }
        }


        /// <summary>
        /// Period for call object control procedure
        /// </summary>
        public int Interval
        {
            get
            {
                lock (locker)
                {
                    return interval;
                }
            }
            set
            {
                lock (locker)
                {
                    if (interval != value)
                        interval = value;
                }
            }
        }


        private int interval = 50;

        private const int MinDelay = 100;

        private const int BaseDelay = 250;

        /// <summary>
        /// Sync object
        /// </summary>
        private object locker = new object();

        /// <summary>
        /// Main thread object
        /// </summary>
        private Thread controlThread = null;


        /// <summary>
        /// Stop event
        /// </summary>
        private ManualResetEvent stopEvent = new ManualResetEvent(false);
    }
}
