using DataManager.Classes;
using OnewheelBluetooth.Classes.Events;
using OnewheelBluetooth.Classes.Handler;
using System;
using System.Collections.Generic;

namespace OnewheelBluetooth.Classes
{
    /// <summary>
    /// A runtime storage for Onewheel characteristics with their last value.
    /// </summary>
    public class OnewheelCharacteristicsCache
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly Guid CHARACTERISTIC_SERIAL_NUMBER = Guid.Parse("e659F301-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_FIRMWARE_REVISION = Guid.Parse("e659f311-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_HARDWARE_REVISION = Guid.Parse("e659f318-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_RIDING_MODE = Guid.Parse("e659f302-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_CUSTOM_NAME = Guid.Parse("e659f3fd-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_STATUS = Guid.Parse("e659f30f-ea98-11e3-ac10-0800200c9a66"); // Unknown result
        public static readonly Guid CHARACTERISTIC_SAFETY_HR = Guid.Parse("e659f317-ea98-11e3-ac10-0800200c9a66"); // Unknown result
        public static readonly Guid CHARACTERISTIC_LAST_ERRORS = Guid.Parse("e659f31c-ea98-11e3-ac10-0800200c9a66"); // Unknown result

        public static readonly Guid CHARACTERISTIC_DATA_29 = Guid.Parse("e659f31d-ea98-11e3-ac10-0800200c9a66"); // Unknown result
        public static readonly Guid CHARACTERISTIC_DATA_30 = Guid.Parse("e659f31e-ea98-11e3-ac10-0800200c9a66"); // Unknown result
        public static readonly Guid CHARACTERISTIC_DATA_31 = Guid.Parse("e659f31f-ea98-11e3-ac10-0800200c9a66"); // Unknown result
        public static readonly Guid CHARACTERISTIC_DATA_32 = Guid.Parse("e659f320-ea98-11e3-ac10-0800200c9a66"); // Unknown result

        public static readonly Guid CHARACTERISTIC_BATTERY_SERIAL_NUMBER = Guid.Parse("e659f306-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LEVEL = Guid.Parse("e659f303-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_5 = Guid.Parse("e659f304-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_20 = Guid.Parse("e659f305-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_TEMPERATUR = Guid.Parse("e659f315-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_VOLTAGE = Guid.Parse("e659f316-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_CURRENT_AMPERE = Guid.Parse("e659f312-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_CELL_VOLTAGES = Guid.Parse("e659f31b-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE = Guid.Parse("e659f310-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_MODE = Guid.Parse("e659f30c-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_BACK = Guid.Parse("e659f30e-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_FRONT = Guid.Parse("e659f30d-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_SPEED_RPM = Guid.Parse("e659f30b-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_PITCH = Guid.Parse("e659f307-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_ROLL = Guid.Parse("e659f308-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_YAW = Guid.Parse("e659f309-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_TRIP_ODOMETER = Guid.Parse("e659f30a-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS = Guid.Parse("e659f314-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_TRIP_AMPERE_HOURS = Guid.Parse("e659f313-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_LIFETIME_ODOMETER = Guid.Parse("e659f319-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIFETIME_AMPERE_HOURS = Guid.Parse("e659f31a-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_UART_SERIAL_WRITE = Guid.Parse("e659f3ff-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_UART_SERIAL_READ = Guid.Parse("e659f3fe-ea98-11e3-ac10-0800200c9a66");

        // Unknown usage. Handle 1 is probably used for firmware updates:
        public static readonly Guid CHARACTERISTIC_HANDLE_1_UNKNOWN_1 = Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb");
        public static readonly Guid CHARACTERISTIC_HANDLE_1_UNKNOWN_2 = Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb");
        public static readonly Guid CHARACTERISTIC_HANDLE_1_UNKNOWN_3 = Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb");
        public static readonly Guid CHARACTERISTIC_HANDLE_1_UNKNOWN_4 = Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb");
        public static readonly Guid CHARACTERISTIC_HANDLE_1_UNKNOWN_5 = Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb");
        public static readonly Guid CHARACTERISTIC_HANDLE_12_UNKNOWN_1 = Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb");

        // Mock UUID objects:
        public static readonly Guid MOCK_TRIP_TOP_RPM = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid MOCK_LIVETIME_TOP_RPM = Guid.Parse("00000000-0000-0000-0000-000000000002");

        public static readonly Guid[] SUBSCRIBE_TO_CHARACTERISTICS = new Guid[]
        {
            CHARACTERISTIC_SERIAL_NUMBER,
            CHARACTERISTIC_FIRMWARE_REVISION,
            CHARACTERISTIC_HARDWARE_REVISION,

            CHARACTERISTIC_RIDING_MODE,
            CHARACTERISTIC_CUSTOM_NAME,
            CHARACTERISTIC_STATUS,
            CHARACTERISTIC_SAFETY_HR,
            CHARACTERISTIC_LAST_ERRORS,

            CHARACTERISTIC_DATA_29,
            CHARACTERISTIC_DATA_30,
            CHARACTERISTIC_DATA_31,
            CHARACTERISTIC_DATA_32,

            CHARACTERISTIC_BATTERY_SERIAL_NUMBER,
            CHARACTERISTIC_BATTERY_LEVEL,
            CHARACTERISTIC_BATTERY_LOW_5,
            CHARACTERISTIC_BATTERY_LOW_20,
            CHARACTERISTIC_BATTERY_TEMPERATUR,
            CHARACTERISTIC_BATTERY_VOLTAGE,
            CHARACTERISTIC_BATTERY_CURRENT_AMPERE,
            CHARACTERISTIC_BATTERY_CELL_VOLTAGES,

            CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE,
            CHARACTERISTIC_LIGHTING_MODE,
            CHARACTERISTIC_LIGHTING_BACK,
            CHARACTERISTIC_LIGHTING_FRONT,
            CHARACTERISTIC_SPEED_RPM,

            CHARACTERISTIC_PITCH,
            CHARACTERISTIC_ROLL,
            CHARACTERISTIC_YAW,

            CHARACTERISTIC_TRIP_ODOMETER,
            CHARACTERISTIC_TRIP_AMPERE_HOURS,
            CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS,

            CHARACTERISTIC_LIFETIME_ODOMETER,
            CHARACTERISTIC_LIFETIME_AMPERE_HOURS,

            // CHARACTERISTIC_UART_SERIAL_WRITE,
            // CHARACTERISTIC_UART_SERIAL_READ,
        };

        private readonly Dictionary<Guid, byte[]> CHARACTERISTICS = new Dictionary<Guid, byte[]>();
        private readonly object CHARACTERISTICS_LOCK = new object();

        internal readonly OnewheelSpeedHandler SPEED_HANDLER = new OnewheelSpeedHandler();
        internal readonly OnewheelThermalHandler THERMAL_HANDLER = new OnewheelThermalHandler();
        internal readonly OnewheelBatteryHandler BATTERY_HANDLER = new OnewheelBatteryHandler();

        public delegate void CharacteristicChangedHandler(OnewheelCharacteristicsCache sender, CharacteristicChangedEventArgs args);
        public event CharacteristicChangedHandler CharacteristicChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelCharacteristicsCache()
        {
            // Load top RPM:
            byte[] topRpm = Settings.getSettingByteArray(SettingsConsts.BOARD_TOP_RPM_LIVE);
            if (topRpm != null)
            {
                AddToDictionary(MOCK_LIVETIME_TOP_RPM, topRpm, false);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string GetString(byte[] value)
        {
            if (value != null)
            {
                return System.Text.Encoding.ASCII.GetString(value);
            }
            return null;
        }

        public string GetString(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetString(value);
        }

        public uint GetUint(byte[] value)
        {
            if (value != null)
            {
                switch (value.Length)
                {
                    case 2:
                        return BitConverter.ToUInt16(value, 0);

                    case 4:
                        return BitConverter.ToUInt32(value, 0);
                }
            }
            return 0;
        }

        public uint GetUint(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetUint(value);
        }

        public ulong GetULong(byte[] value)
        {
            if (value != null)
            {
                if (value.Length == 8)
                {
                    return BitConverter.ToUInt64(value, 0);
                }
            }
            return 0;
        }

        public ulong GetULong(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetULong(value);
        }

        public bool GetBool(byte[] value)
        {
            if (value != null)
            {
                if (value.Length == 1)
                {
                    return BitConverter.ToBoolean(value, 0);
                }
            }
            return false;
        }

        public bool GetBool(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetBool(value);
        }

        public byte[] GetBytes(Guid uuid)
        {
            byte[] value = null;
            lock (CHARACTERISTICS_LOCK)
            {
                CHARACTERISTICS.TryGetValue(uuid, out value);
            }
            return value;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateMockObjects(Guid uuid, byte[] value)
        {
            if (uuid.Equals(CHARACTERISTIC_SPEED_RPM))
            {
                uint rpm = GetUint(value);

                if (rpm > GetUint(MOCK_TRIP_TOP_RPM))
                {
                    AddToDictionary(MOCK_TRIP_TOP_RPM, value, false);
                }

                if (rpm > GetUint(MOCK_LIVETIME_TOP_RPM))
                {
                    AddToDictionary(MOCK_LIVETIME_TOP_RPM, value, false);
                    Settings.setSetting(SettingsConsts.BOARD_TOP_RPM_LIVE, value);
                }
            }
        }

        #endregion

        #region --Misc Methods (Internal)--
        /// <summary>
        /// Adds the given value to the characteristics dictionary and triggers the BoardCharacteristicChanged event.
        /// </summary>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="value">The value of the characteristics.</param>
        /// <param name="updateMockObjects">Whether to update the mock objects</param>
        internal void AddToDictionary(Guid uuid, byte[] value, bool shouldUpdateMockObjects)
        {
            AddToDictionary(uuid, value, DateTime.Now, shouldUpdateMockObjects);
        }

        /// <summary>
        /// Adds the given value to the characteristics dictionary and triggers the BoardCharacteristicChanged event.
        /// </summary>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="value">The value of the characteristics.</param>
        /// <param name="timestamp">A when did the value change?</param>
        /// <param name="updateMockObjects">Whether to update the mock objects</param>
        internal void AddToDictionary(Guid uuid, byte[] value, DateTime timestamp, bool shouldUpdateMockObjects)
        {
            lock (CHARACTERISTICS_LOCK)
            {
                CHARACTERISTICS[uuid] = value;
            }
            CharacteristicChanged?.Invoke(this, new CharacteristicChangedEventArgs(uuid, value));

            if (shouldUpdateMockObjects)
            {
                UpdateMockObjects(uuid, value);
            }

            // Add battery level values to the DB:
            if (uuid.Equals(CHARACTERISTIC_BATTERY_LEVEL))
            {
                BATTERY_HANDLER.onBatteryChargeChanged(GetUint(value), timestamp);
            }
            // Add speed values to the DB:
            else if (uuid.Equals(CHARACTERISTIC_SPEED_RPM))
            {
                uint rpm = GetUint(value);
                SPEED_HANDLER.onRpmChanged(rpm, timestamp);
            }
            else if (uuid.Equals(CHARACTERISTIC_BATTERY_TEMPERATUR))
            {
                THERMAL_HANDLER.onTempChanged(OnewheelThermalHandler.BATTERY_TEMP, value[1], timestamp);
            }
            else if (uuid.Equals(CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE))
            {

                THERMAL_HANDLER.onTempChanged(OnewheelThermalHandler.CONTROLLER_TEMP, value[0], timestamp);
                THERMAL_HANDLER.onTempChanged(OnewheelThermalHandler.MOTOR_TEMP, value[1], timestamp);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
