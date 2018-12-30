using Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace OnewheelBluetooth.Classes
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
        public static async Task PrintAsync(GattCharacteristic c)
        {
            string value = await ReadStringAsync(c);
            Logger.Info("\tUUID: " + c.Uuid + ", Value: " + value + ", Handle: " + c.AttributeHandle + ", Description: " + c.UserDescription);
            Logger.Info("\t\tProperties: " + c.CharacteristicProperties.ToString());
        }

        public static async Task<byte[]> ReadBytesAsync(GattCharacteristic c)
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
                    return data;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to read bytes from characteristic " + c.Uuid, e);
            }
            return null;
        }

        public static void ReverseByteOrderIfNeeded(byte[] data)
        {
            if (!(data is null) && BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
        }

        public static async Task<string> ReadStringAsync(GattCharacteristic c)
        {
            byte[] data = await ReadBytesAsync(c);
            ReverseByteOrderIfNeeded(data);
            if (data != null)
            {
                return BitConverter.ToString(data);
            }
            return null;
        }

        public static async Task<short> ReadShortAsync(GattCharacteristic c)
        {
            byte[] data = await ReadBytesAsync(c);
            ReverseByteOrderIfNeeded(data);
            if (data != null)
            {
                return BitConverter.ToInt16(data, 0);
            }
            return -1;
        }

        public static double RpmToKilometersPerHour(uint rpm)
        {
            return Math.Round(RpmToKilometers(rpm) * 60, 2);
        }

        public static double RpmToKilometers(uint rpm)
        {
            return (35.0 * rpm) / 39370.1;
        }

        public static double MilesToKilometers(double miles)
        {
            return miles * 1.60934;
        }

        public static double ToAmpere(uint value, OnewheelType type)
        {
            double multiplier = 0;

            switch (type)
            {
                case OnewheelType.ONEWHEEL:
                    multiplier = 0.9;
                    break;

                case OnewheelType.ONEWHEEL_PLUS:
                    multiplier = 1.8;
                    break;

                case OnewheelType.ONEWHEEL_XR:
                    multiplier = 1.8; // Not validated
                    break;
            }

            double amp = value / 1000.0 * multiplier;

            return Math.Round(amp, 2);
        }

        public static double ToVoltage(uint value)
        {
            double voltage = value / 10.0;

            return Math.Round(voltage, 2);
        }

        public static double[] ToBatteryCellVoltages(byte[] values)
        {
            double[] voltages = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                voltages[i] = values[i] / 50.0;
            }

            return voltages;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static double CarveAbilityToDouble(byte data)
        {
            sbyte dataSig = (sbyte)data;
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY;
            }

            return dataSig / Consts.CUSTOM_SHAPING_STEP_CARVE_ABILITY;
        }

        public static byte CarveAbilityToByte(double value)
        {
            sbyte dataSig = (sbyte)(value * Consts.CUSTOM_SHAPING_STEP_CARVE_ABILITY);
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY;
            }
            return (byte)dataSig;
        }

        public static double StanceProfileToDouble(byte data)
        {
            sbyte dataSig = (sbyte)data;
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE;
            }

            return dataSig / Consts.CUSTOM_SHAPING_STEP_STANCE_PROFILE;
        }

        public static byte StanceProfileToByte(double value)
        {
            sbyte dataSig = (sbyte)(value * Consts.CUSTOM_SHAPING_STEP_STANCE_PROFILE);
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE;
            }
            return (byte)dataSig;
        }

        public static double AggressivenessToDouble(byte data)
        {
            sbyte dataSig = (sbyte)data;
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_AGGRESSIVENESS)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_AGGRESSIVENESS;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_AGGRESSIVENESS)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_AGGRESSIVENESS;
            }
            return Math.Round((dataSig + 100.7) / 20.7, 1);
        }

        public static byte AggressivenessToByte(double value)
        {
            int dataSig = (int)Math.Round(value * 20.7 - 100.7, 0);
            if (dataSig > Consts.CUSTOM_SHAPING_MAX_AGGRESSIVENESS)
            {
                dataSig = Consts.CUSTOM_SHAPING_MAX_AGGRESSIVENESS;
            }
            else if (dataSig < Consts.CUSTOM_SHAPING_MIN_AGGRESSIVENESS)
            {
                dataSig = Consts.CUSTOM_SHAPING_MIN_AGGRESSIVENESS;
            }
            return (byte)dataSig;
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
