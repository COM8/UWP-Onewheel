using System.Diagnostics;
using System.Text;

namespace OnewheelBluetooth.Classes
{
    public class UartHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string ascii = "";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public UartHelper(OnewheelBoard onewheel)
        {
            onewheel.CharacteristicChanged += Onewheel_CharacteristicChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void PrintByteArray(byte[] data)
        {
            /*string dec = "";
            for (int i = 0; i < data.Length; i++)
            {
                dec += data[i];
                
            }*/
            //string hex = BitConverter.ToString(data);
            //Debug.WriteLine(hex);
            Debug.Write(Encoding.ASCII.GetString(data));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Onewheel_CharacteristicChanged(OnewheelBoard sender, Events.OnewheelCharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelBoard.CHARACTERISTIC_UART_SERIAL_READ))
            {
                byte[] data = sender.GetBytes(OnewheelBoard.CHARACTERISTIC_UART_SERIAL_READ);
                if (data != null)
                {
                    PrintByteArray(data);
                }
            }
            else if (args.UUID.Equals(OnewheelBoard.CHARACTERISTIC_UART_SERIAL_WRITE))
            {
                byte[] data = sender.GetBytes(OnewheelBoard.CHARACTERISTIC_UART_SERIAL_WRITE);
                if (data != null)
                {
                    PrintByteArray(data);
                }
            }
        }

        #endregion
    }
}
