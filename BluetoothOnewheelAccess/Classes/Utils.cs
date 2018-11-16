using Logging;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothOnewheelAccess.Classes
{
    public static class Utils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task printGattCharacteristicAsync(GattCharacteristic c)
        {
            string value = await readStringFromCharacteristicAsync(c);
            Logger.Info("\tUUID: " + c.Uuid + ", Value: " + value + ", Handle: " + c.AttributeHandle + ", Description: " + c.UserDescription);
            Logger.Info("\t\tProperties: " + c.CharacteristicProperties.ToString());
        }

        public static async Task<byte[]> readBytesFromCharacteristicAsync(GattCharacteristic c)
        {
            try
            {
                GattReadResult vRes = await c.ReadValueAsync();
                if (vRes.Status == GattCommunicationStatus.Success)
                {
                    byte[] data = null;
                    using (DataReader reader = DataReader.FromBuffer(vRes.Value))
                    {
                        data = new byte[reader.UnconsumedBufferLength];
                        reader.ReadBytes(data);
                    }

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(data);
                    }
                    return data;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to read bytes from characteristic " + c.Uuid, e);
            }
            return null;
        }

        public static async Task<string> readStringFromCharacteristicAsync(GattCharacteristic c)
        {
            byte[] data = await readBytesFromCharacteristicAsync(c);
            if (data != null)
            {
                return BitConverter.ToString(data);
            }
            return null;
        }

        public static async Task<int> readIntFromCharacteristicAsync(GattCharacteristic c)
        {
            byte[] data = await readBytesFromCharacteristicAsync(c);
            if (data != null)
            {
                return BitConverter.ToInt16(data, 0);
            }
            return -1;
        }

        public static double rpmToKilometersPerHour(uint rpm)
        {
            return Math.Round(rpmToKilometers(rpm) * 60, 2);
        }

        public static double rpmToKilometers(uint rpm)
        {
            return (35.0 * rpm) / 39370.1;
        }

        public static double milesToKilometers(double miles)
        {
            return miles * 1.60934;
        }

        public static double convertToAmpere(uint value)
        {
            double multiplier = 0;

            if (true)
            {
                // Onewheel+:
                multiplier = 1.8;
            }
            else
            {
                // Onewheel:
                multiplier = 0.9;
            }

            double amp = value / 1000.0 * multiplier;

            return Math.Round(amp, 2);
        }

        public static double convertToVoltage(uint value)
        {
            double voltage = value / 10.0;

            return Math.Round(voltage, 2);
        }

        public static double[] convertToBatteryCellVoltages(byte[] values)
        {
            double[] voltages = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                voltages[i] = values[i] / 50.0;
            }

            return voltages;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
