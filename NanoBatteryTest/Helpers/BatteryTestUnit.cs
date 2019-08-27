using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace NanoBatteryTest.Helpers
{

    /// <summary>
    /// Базовый класс проведения тестирования батарей
    /// </summary>
    public abstract class BatteryTestUnit : ViewModelBase
    {
        /// <summary>
        /// Запустить тест на выполнение
        /// </summary>
        public abstract void RunTestUnit();

        /// <summary>
        /// Наименование теста
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Описание теста 
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Отображение окна настроек теста 
        /// </summary>
        public UserControl ParametersContent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Отображение текущего состояния теста
        /// </summary>
        public UserControl ProcessContent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Отображение результатов теста
        /// </summary>
        public UserControl ResultsContent
        {
            get;
            protected set;
        }


    }


    /// <summary>
    /// Использование теста С20
    /// </summary>
    public class BatteryC20TestUnit : BatteryTestUnit
    {

        /// <summary>
        /// Запустить тест на выполнение
        /// </summary>        
        public override void RunTestUnit()
        {
            throw new NotImplementedException();
        }
        
    }

    /// <summary>
    /// Использование теста иммитирушего реальную работу аккамулятора
    /// </summary>
    public class BatteryRealWorkTesUnit : BatteryTestUnit
    {
        /// <summary>
        /// Запустить тест на выполнение
        /// </summary>
        public override void RunTestUnit()
        {
            throw new NotImplementedException();
        }
    }
}