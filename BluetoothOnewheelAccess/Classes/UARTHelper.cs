using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothOnewheelAccess.Classes
{
    public class UARTHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string ascii;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/06/2018 Created [Fabian Sauter]
        /// </history>
        public UARTHelper(OnewheelInfo info)
        {
            this.ascii = "";
            info.BoardCharacteristicChanged += Info_BoardCharacteristicChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void printByteArray(byte[] data)
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
        private void Info_BoardCharacteristicChanged(OnewheelInfo sender, Events.BoardCharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_UART_SERIAL_READ))
            {
                byte[] data = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getRawValue(OnewheelInfo.CHARACTERISTIC_UART_SERIAL_READ);
                if (data != null)
                {
                    printByteArray(data);
                }
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_UART_SERIAL_WRITE))
            {
                byte[] data = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getRawValue(OnewheelInfo.CHARACTERISTIC_UART_SERIAL_WRITE);
                if (data != null)
                {
                    printByteArray(data);
                }
            }
        }

        #endregion
    }
}
