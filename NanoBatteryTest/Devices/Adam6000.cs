using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advantech.Adam;
using System.Net.Sockets;

namespace NanoBatteryTest.Devices
{

    public class Adam6000 : AccusitionDeviceBase
    {
        public Adam6000(string name, Adam6000Type model)  : base(name, new ChannelsInfo {
                                                                            AnalogInputsCount = AnalogInput.GetChannelTotal(model),
                                                                            AnalogOutputsCount = AnalogOutput.GetChannelTotal(model),
                                                                            DiscreteInputsCount = DigitalInput.GetChannelTotal(model),
                                                                            DiscreteOutputsCount = DigitalOutput.GetChannelTotal(model),
                                                                            CounterInputsCount = DigitalInput.GetChannelTotal(model)} )
        {
            Model = model;            
            if (AnalogInputs != null )
                AIRanges = new List<byte>(new byte[AnalogInputs.Count]);
        }

        /// <summary>
        /// IP Adress of ADAM-6000 series device
        /// </summary>
        public string IP
        {
            get;
            private set;
        }

        public int Port
        {
            get;
            private set;
        }

        private bool Connect(string ip, int port, int timeout)
        {
            socket = new AdamSocket();
            socket.SetTimeout(timeout, timeout, timeout);

            if (!socket.Connect(ip, ProtocolType.Tcp, port))
                return false;

            if (AnalogInputs != null)
                RefreshAnalogInputsInfo();

            Port = port;
            IP = ip;

            return true;
        }

        public override bool Connect(string connection, int timeout)
        {
            string[] addr = connection.Split(':');

            if (addr.GetLength(0) != 2)
                throw new ArgumentException("Invalid connection string");

            string ip = addr[0];
            int port = 502;
            int.TryParse(addr[1], out port);

            return Connect(ip, port, timeout);
        }

        public void Disconnect()
        {
            socket.Disconnect();
        }

        /// <summary>
        /// Device connection socket
        /// </summary>
        private AdamSocket socket;

        /// <summary>
        /// ADAM-6000 device model
        /// </summary>
        public Adam6000Type Model
        {
            get;
            private set;
        }

        public override void UpdateData()
        {
            int iDiStart = 1;  //start address for discrete inputs
            if (DiscreteInputs != null)
            {
                bool[] bData;

                if (socket.Modbus().ReadCoilStatus(iDiStart, DiscreteInputs.Count, out bData))
                {
                    for (int i = 0; i < DiscreteInputs.Count; i++)
                        DiscreteInputs.Set(i, bData[i]);

                    OnUpdateDI(DiscreteInputs);
                }
                else
                {

                }
            }

            int iDoStart = 17; //start address for discrete outputs
            if (DiscreteOutputs != null)
            {
                bool[] bData;

                if (socket.Modbus().ReadCoilStatus(iDoStart, DiscreteOutputs.Count, out bData))
                {
                    for (int i = 0; i < DiscreteOutputs.Count; i++)
                        DiscreteOutputs.Set(i, bData[i]);

                    OnUpdateDO(DiscreteOutputs);
                }
                else
                {

                }
            }

            
            int iAiStart  = 1;
            int iAiStartStatus = 101;
            if ( AnalogInputs != null)
            {
                int[] iData;
                double [] aiData = new double[AnalogInputs.Count];
                if (socket.Modbus().ReadInputRegs(iAiStart, AnalogInputs.Count, out iData) )
                {
                    for (int i = 0; i < AnalogInputs.Count; i++)
                        aiData[i] = AnalogInput.GetScaledValue(Model, AIRanges[i], iData[i])  ;
                    
                }

                int[] iAIStatus;

                if (socket.Modbus().ReadInputRegs(iAiStartStatus, (AnalogInputs.Count * 2), out iAIStatus))
                {
                    for (int i = 0; i < AnalogInputs.Count; i++)
                        ProcessAnalogInputStatus(i, (ushort)iAIStatus[i*2]);
                }

                OnUpdateAI(aiData);
            }

            if (AnalogOutputs != null)
            {


            }
            
            if (CounterInputs != null)
            {
                int iCounterStart = 1;
                int[] iData;
                int[] counterData = new int[CounterInputs.Count];
                if (socket.Modbus().ReadInputRegs(iCounterStart, CounterInputs.Count * 2, out iData))
                {
                    for (int i = 0; i < CounterInputs.Count; i++)
                        counterData[i] = iData[2 * i + 1] * 65535 + iData[2 * i];

                    OnUpdateCounter(counterData);
                }
               


            }
        }

        /// <summary>
        /// Обработка ошибки чтения данных из аналогового входа
        /// </summary>
        public event EventHandler<StringEventsArgs> AnalogInputStatusFailed;

        private void ProcessAnalogInputStatus(int ch, ushort status)
        {
            string szErrorMsg = "";
            string szComma = " , ";

            if ( AIEnabled[ch] )
            {
                if (status == (ushort)Adam_AiStatus_LowAddress.No_Fault_Detected)
                    return;

                if ((status & (ushort)Adam_AiStatus_LowAddress.FailToProvideAiValueUartTimeout) == (ushort)Adam_AiStatus_LowAddress.FailToProvideAiValueUartTimeout)
                {
                    szErrorMsg = "Fail to provide AI value (UART timeout)";
                }
                else if ((status & (ushort)Adam_AiStatus_LowAddress.Over_Range) == (ushort)Adam_AiStatus_LowAddress.Over_Range)
                {
                    szErrorMsg = (szErrorMsg == string.Empty) ? ("Over Range") : (szErrorMsg + szComma + "Over Range");
                }
                else if ((status & (ushort)Adam_AiStatus_LowAddress.Under_Range) == (ushort)Adam_AiStatus_LowAddress.Under_Range)
                {
                    szErrorMsg = (szErrorMsg == string.Empty) ? ("Under Range") : (szErrorMsg + szComma + "Under Range");
                }
                else if ((status & (ushort)Adam_AiStatus_LowAddress.Open_Circuit_Burnout) == (ushort)Adam_AiStatus_LowAddress.Open_Circuit_Burnout)
                {
                    szErrorMsg = (szErrorMsg == string.Empty) ? ("Open Circuit (Burnout)") : (szErrorMsg + szComma + "Open Circuit (Burnout)");
                }
                else if ((status & (ushort)Adam_AiStatus_LowAddress.Zero_Span_CalibrationError) == (ushort)Adam_AiStatus_LowAddress.Zero_Span_CalibrationError)
                {
                    szErrorMsg = (szErrorMsg == string.Empty) ? ("Zero/Span Calibration Error") : (szErrorMsg + szComma + "Zero/Span Calibration Error");
                }

                if (AnalogInputStatusFailed != null)
                    AnalogInputStatusFailed(this, new StringEventsArgs(szErrorMsg));
            }
        }


        private List<byte> AIRanges  = null;
        private List<bool>   AIEnabled = null;

        private bool RefreshAnalogInputsInfo()
        {
            if (AnalogInputs  == null)
                return false;

            if (AIRanges == null)
                AIRanges = new List<byte>(new byte[AnalogInputs.Count]);

            if (AIEnabled == null)
                AIEnabled = new List<bool>(new bool[AnalogInputs.Count]);

            for (int ch = 0; ch < AnalogInputs.Count; ch++)
            {
                byte usRange;
                if (socket.AnalogInput().GetInputRange(ch, out usRange))
                    AIRanges[ch] = usRange;
            }

            bool[] bEnabled;

            if (socket.AnalogInput().GetChannelEnabled(AnalogInputs.Count, out bEnabled))
            {
                for (int ch = 0; ch < AnalogInputs.Count; ch++)
                    AIEnabled[ch] = bEnabled[ch];
            }
                

            return true;
        }

        public void SetDiscreteOutput(int ch, bool state)
        {
            if (socket == null)
                return;

            int iDoStart = 17 + ch; //start address for discrete outputs
            if (DiscreteOutputs.Count > 0)
            {
                if (!socket.Modbus().ForceSingleCoil(iDoStart, state))
                {
                    socket.Modbus().ForceSingleCoil(iDoStart, state);
                }
                else
                {

                }
            }
        }


       
        
    }
}
