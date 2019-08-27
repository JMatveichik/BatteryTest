using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoBatteryTest.Models
{
    public enum BanchMessageType
    {
        Information = 0,
        Warning = 1,
        Alarm = 2,
        Started = 3,
        Successfully = 4
    }

    public class BanchMessage
    {
        public BanchMessage(string message, BanchMessageType type, int moduleID = 0 , int cellID = 0)
        {
            Time = DateTime.Now;
            MessageType = type;
            Message = message;
            ModuleID = (moduleID == 0) ? string.Format("MODULE {0}", moduleID) : "";
            CellID = (cellID == 0) ? string.Format("CELL {0}", cellID) : "";

            makeSource(moduleID, cellID);

        }

        private void makeSource(int moduleID = 0, int cellID = 0)
        {
            Source = "FACILITY";
            if (moduleID > 0)
                Source += string.Format(" : MODULE {0}", moduleID);
            else
                return;

            if (cellID > 0)
                Source += string.Format(" : CELL {0}", cellID);            
        }

        public void Save(StreamWriter sw)
        {
            //string.Format("{0, 10}\t{1, 10}\t{2, 15}\t{3}", MessageType.ToString(), Time)
        }

        public string Source
        {
            get;
            private set;
        }
        public DateTime Time
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public BanchMessageType MessageType
        {
            get;
            private set;
        }

        public string ModuleID
        {
            get;
            private set;
        }
        public string CellID
        {
            get;
            private set;
        }
    }
}
