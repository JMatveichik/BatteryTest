using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace NanoBatteryTest.Models
{   

    public class ProcessParameters : ViewModelBase
    {
        
        /// <summary>
        /// Начать цикл испытаний с зарядки
        /// </summary>
        public bool StartFromCharge
        {
            get { return satrtFromCharge; }
            set
            {
                if (satrtFromCharge == value)
                    return;

                satrtFromCharge = value;
                OnPropertyChanged("StartFromCharge");
            }
        }
        private bool satrtFromCharge = true;

        /// <summary>
        /// Испытание батареи с добавками УНМ 
        /// </summary>
        public bool TestBatteryWithCNM
        {
            get { return testBatteryWithCNM; }
            set
            {
                if (testBatteryWithCNM == value)
                    return;

                testBatteryWithCNM = value;
                OnPropertyChanged("TestBatteryWithCNM");
            }
        }
        private bool testBatteryWithCNM = true;


        private bool chargeWhenComplete;

        public bool ChargeWhenComplete
        {
            get { return chargeWhenComplete; }
            set
            {
                if (chargeWhenComplete == value)
                    return;

                chargeWhenComplete = value;
                OnPropertyChanged("ChargeWhenComplete");
            }
        }

        /// <summary>
        /// Номинальная емкость батареи
        /// </summary>
        public double BattertyCapacity
        {
            get { return batteryCapacity; }
            set
            {
                if (batteryCapacity == value)
                    return;

                batteryCapacity = value;
                OnPropertyChanged("BattertyCapacity");
            }
        }
        private double batteryCapacity = 3.0;

        /// <summary>
        /// Напряжение заряда
        /// </summary>
        public double ChargeVoltage
        {
            get { return chargeVoltage; }
            set
            {
                if (chargeVoltage == value)
                    return;

                chargeVoltage = value;
                OnPropertyChanged("ChargeVoltage");
            }
        }
        private double chargeVoltage = 14.4;

        public TimeSpan CurrentChargeStability
        {
            get { return currentChargeStability; }
            set
            {
                if (currentChargeStability == value)
                    return;

                currentChargeStability = value;
                OnPropertyChanged("CurrentChargeStability");
            }
        }
        private TimeSpan currentChargeStability = TimeSpan.FromHours(2.0);

        /// <summary>
        /// Время простоя после заряда аккамулятора
        /// </summary>
        public TimeSpan AfterChargeRelaxation
        {
            get { return afterChargeRelaxation; }
            set
            {
                if (afterChargeRelaxation == value)
                    return;

                afterChargeRelaxation = value;
                OnPropertyChanged("AfterChargeRelaxation");
            }
        }
        private TimeSpan afterChargeRelaxation = TimeSpan.FromHours(2.0);


        /// <summary>
        /// Таймаут заряда аккамулятора
        /// </summary>
        public TimeSpan ChargeTimeout
        {
            get { return chargeTimeout; }
            set
            {
                if (chargeTimeout == value)
                    return;

                chargeTimeout = value;
                OnPropertyChanged("ChargeTimeout");
            }
        }
        private TimeSpan chargeTimeout = TimeSpan.FromHours(10.0);

        /// <summary>
        /// Ограничение тока при зарядке
        /// </summary>
        public double CurrentPercentLimit
        {
            get { return currentPercentLimit; }
            set
            {
                if (currentPercentLimit == value)
                    return;

                currentPercentLimit = value;
                OnPropertyChanged("CurrentPercentLimit");
            }
        }        
        public double currentPercentLimit = 2.0;

        /// <summary>
        /// Ограничение напряжение  при разряде
        /// </summary>
        public double LowBatteryVoltageLimit
        {
            get { return lowBatteryVoltageLimit; }
            set
            {
                if (lowBatteryVoltageLimit == value)
                    return;

                lowBatteryVoltageLimit = value;
                OnPropertyChanged("LowBatteryVoltageLimit");
            }
        }
        public double lowBatteryVoltageLimit = 10.8;

        /// <summary>
        /// Ограничение напряжение  при разряде
        /// </summary>
        public double HighBatteryVoltageLimit
        {
            get { return highBatteryVoltageLimit; }
            set
            {
                if (highBatteryVoltageLimit == value)
                    return;

                highBatteryVoltageLimit = value;
                OnPropertyChanged("HighBatteryVoltageLimit");
            }
        }
        public double highBatteryVoltageLimit = 14.3;


        
        public int CyclesCount
        {
            get { return cyclesCount; }
            set
            {
                if (cyclesCount == value)
                    return;

                cyclesCount = value;
                OnPropertyChanged("CyclesCount");
            }
        }
        private int cyclesCount = 3;


        /// <summary>
        /// Reset Facility Parameters To Default
        /// </summary>
        public void ResetToDefault()
        {

        }


        public void ResetToDemo()
        {         
           

        }

       

        public void Load()
        {
            Properties.Settings set = Properties.Settings.Default;

            StartFromCharge = set.StartFromCharge;
            BattertyCapacity = set.BatteryCapacity;
            ChargeVoltage = set.ChargeVoltage;
            CurrentChargeStability = set.CurrentStabilityWnd;
            AfterChargeRelaxation = set.AftreChargeRelaxation;
            ChargeTimeout = set.ChargeTimeout;
            CurrentPercentLimit = set.CurrentPercentLimit;
            LowBatteryVoltageLimit = set.LowBatteryVoltageLimit;
            HighBatteryVoltageLimit = set.HighBatteryVoltageLimit;
            ChargeWhenComplete = set.ChargeWhenComplete;
            CyclesCount = set.CyclesCount;
        }

        public void Save()
        {
            Properties.Settings set = Properties.Settings.Default;

            set.StartFromCharge = StartFromCharge;
            set.BatteryCapacity = BattertyCapacity;
            set.ChargeVoltage = ChargeVoltage;
            set.CurrentStabilityWnd = CurrentChargeStability;
            set.AftreChargeRelaxation = AfterChargeRelaxation;
            set.ChargeTimeout = ChargeTimeout;
            set.CurrentPercentLimit = CurrentPercentLimit;
            set.LowBatteryVoltageLimit = LowBatteryVoltageLimit;
            set.HighBatteryVoltageLimit = HighBatteryVoltageLimit;
            set.ChargeWhenComplete = ChargeWhenComplete;
            set.CyclesCount = CyclesCount;

            set.Save();
        }



    }
}
