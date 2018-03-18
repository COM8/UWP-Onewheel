using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Microsoft.Toolkit.Uwp.Connectivity;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothOnewheelAccess.Classes
{
    public class OnewheelInfo
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // UUID source: https://github.com/ponewheel/android-ponewheel/blob/master/app/src/main/java/net/kwatts/powtools/model/OWDevice.java
        public static readonly Guid CHARACTERISTIC_SERIAL_NUMBER = Guid.Parse("e659F301-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_FIRMWARE_REVISION = Guid.Parse("e659f311-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_HARDWARE_REVISION = Guid.Parse("e659f318-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_RIDING_MODE = Guid.Parse("e659f302-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_CUSTOM_NAME = Guid.Parse("e659f3fd-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_BATTERY_SERIAL_NUMBER = Guid.Parse("e659f306-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LEVEL = Guid.Parse("e659f303-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_5 = Guid.Parse("e659f304-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_20 = Guid.Parse("e659f305-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_TEMPERATUR = Guid.Parse("e659f315-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_VOLTAGE = Guid.Parse("e659f316-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_CURRENT_AMPERE = Guid.Parse("e659f312-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE = Guid.Parse("e659f310-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_MODE = Guid.Parse("e659f30c-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_BACK = Guid.Parse("e659f30e-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_FRONT = Guid.Parse("e659f30d-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_SPEED_RPM = Guid.Parse("e659f30b-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_TRIP_ODOMETER = Guid.Parse("e659f30a-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_TRIP_AMPERE_HOURS = Guid.Parse("e659f314-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS = Guid.Parse("e659f313-ea98-11e3-ac10-0800200c9a66");

        public static readonly Guid CHARACTERISTIC_LIFETIME_ODOMETER = Guid.Parse("e659f319-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIFETIME_AMPERE_HOURS = Guid.Parse("e659f31a-ea98-11e3-ac10-0800200c9a66");

        // Mock UUID objects:
        public static readonly Guid MOCK_TRIP_TOP_RPM = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid MOCK_LIVETIME_TOP_RPM = Guid.Parse("00000000-0000-0000-0000-000000000002");

        public static readonly Guid[] SUBSCRIBED_CHARACTERISTICS = new Guid[]
        {
            CHARACTERISTIC_SERIAL_NUMBER,
            CHARACTERISTIC_FIRMWARE_REVISION,
            CHARACTERISTIC_HARDWARE_REVISION,

            CHARACTERISTIC_RIDING_MODE,
            CHARACTERISTIC_CUSTOM_NAME,

            CHARACTERISTIC_BATTERY_SERIAL_NUMBER,
            CHARACTERISTIC_BATTERY_LEVEL,
            CHARACTERISTIC_BATTERY_LOW_5,
            CHARACTERISTIC_BATTERY_LOW_20,
            CHARACTERISTIC_BATTERY_TEMPERATUR,
            CHARACTERISTIC_BATTERY_VOLTAGE,
            CHARACTERISTIC_BATTERY_CURRENT_AMPERE,

            CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE,
            CHARACTERISTIC_LIGHTING_MODE,
            CHARACTERISTIC_LIGHTING_BACK,
            CHARACTERISTIC_LIGHTING_FRONT,
            CHARACTERISTIC_SPEED_RPM,

            CHARACTERISTIC_TRIP_ODOMETER,
            CHARACTERISTIC_TRIP_AMPERE_HOURS,
            CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS,

            CHARACTERISTIC_LIFETIME_ODOMETER,
            CHARACTERISTIC_LIFETIME_AMPERE_HOURS
        };

        private Dictionary<Guid, byte[]> characteristics;
        private ObservableBluetoothLEDevice board;

        private const string BACKGROUND_TASK_ENTRY_POINT = "BluetoothBackgroundTask.Classes.BackgroundTask";
        private const string BACKGROUND_TASK_NAME = "onewheel_bluetooth_background_task";

        public delegate void BoardCharacteristicChangedHandler(OnewheelInfo sender, BoardCharacteristicChangedEventArgs args);

        public event BoardCharacteristicChangedHandler BoardCharacteristicChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 16/03/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelInfo()
        {
            this.characteristics = new Dictionary<Guid, byte[]>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setBoard(ObservableBluetoothLEDevice board)
        {
            if (this.board != null)
            {
                this.board.PropertyChanged -= Board_PropertyChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                this.board.PropertyChanged += Board_PropertyChanged;
            }
            loadCharacteristics();
        }

        public string getCharacteristicAsString(byte[] value)
        {
            if (value != null)
            {
                return System.Text.Encoding.ASCII.GetString(value);
            }
            return null;
        }

        public string getCharacteristicAsString(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            return getCharacteristicAsString(value);
        }

        public uint getCharacteristicAsUInt(byte[] value)
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

        public uint getCharacteristicAsUInt(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            return getCharacteristicAsUInt(value);
        }

        public ulong getCharacteristicAsULong(byte[] value)
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

        public ulong getCharacteristicAsULong(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            return getCharacteristicAsULong(value);
        }

        public bool getCharacteristicAsBool(byte[] value)
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

        public bool getCharacteristicAsBool(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            return getCharacteristicAsBool(value);
        }

        public byte[] getRawValue(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            return value;
        }

        public void init()
        {
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged1;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);

            // Load top RPM:
            byte[] topRpm = Settings.getSettingByteArray(SettingsConsts.BOARD_TOP_RPM_LIVE);
            if(topRpm != null)
            {
                addCharacteristicToDictionary(MOCK_LIVETIME_TOP_RPM, topRpm, false);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<byte[]> readBytesFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            GattReadResult vRes = await characteristic.ReadValueAsync();
            if (vRes.Status == GattCommunicationStatus.Success)
            {
                return readBytesFromBuffer(vRes.Value);
            }
            return null;
        }

        public byte[] readBytesFromBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            return data;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void registerGATTBackgroundTask(GattCharacteristic c)
        {
            if (BackgroundTaskHelper.IsBackgroundTaskRegistered(BACKGROUND_TASK_NAME))
            {
                BackgroundTaskHelper.Unregister(BACKGROUND_TASK_NAME);
            }

            BackgroundTaskBuilder backgroundTaskBuilder = new BackgroundTaskBuilder
            {
                Name = BACKGROUND_TASK_NAME,
                TaskEntryPoint = BACKGROUND_TASK_ENTRY_POINT,
            };
            backgroundTaskBuilder.SetTrigger(new GattCharacteristicNotificationTrigger(c));

            BackgroundTaskRegistration backgroundTaskRegistration = backgroundTaskBuilder.Register();
        }

        private void loadBoard()
        {
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);
        }

        private void INSTANCE_BoardChanged(OnewheelConnectionHelper helper, BoardChangedEventArgs args)
        {
            setBoard(args.BOARD);
        }

        private async Task subscribeToCharacteristicAsync(GattCharacteristic c)
        {
            // Check if characteristic supports subscriptions:
            GattClientCharacteristicConfigurationDescriptorValue cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            }
            else if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            }
            else
            {
                return;
            }

            // Set subscribed:
            GattCommunicationStatus status;
            try
            {
                status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            // Add event handler:
            if (status == GattCommunicationStatus.Success)
            {
                c.ValueChanged -= C_ValueChanged;
                c.ValueChanged += C_ValueChanged;
            }
        }

        private void loadCharacteristics()
        {
            if (board != null && board.IsConnected)
            {
                Task.Run(async () =>
                {
                    if (board != null && board.BluetoothLEDevice != null)
                    {
                        try
                        {
                            // Get all services:
                            GattDeviceServicesResult sResult = await board.BluetoothLEDevice.GetGattServicesAsync();
                            if (sResult.Status == GattCommunicationStatus.Success)
                            {
                                foreach (GattDeviceService s in sResult.Services)
                                {
                                    // Get all characteristics:
                                    GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                                    if (cResult.Status == GattCommunicationStatus.Success)
                                    {
                                        foreach (GattCharacteristic c in cResult.Characteristics)
                                        {
                                            await loadCharacteristicValueAsync(c);

                                            if (SUBSCRIBED_CHARACTERISTICS.Contains(c.Uuid))
                                            {
                                                await subscribeToCharacteristicAsync(c);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ToDo: Log exception
                        }
                    }
                });
            }
        }

        private async Task loadCharacteristicValueAsync(GattCharacteristic c)
        {
            byte[] value = null;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
            {
                try
                {
                    // Load value from characteristic:
                    value = await readBytesFromCharacteristicAsync(c);
                }
                catch (Exception)
                {
                }
            }

            // Insert characteristic and its value into a dictionary:
            addCharacteristicToDictionary(c.Uuid, value, true);
        }

        /// <summary>
        /// Adds the given value to the characteristics dictionary and triggers the BoardCharacteristicChanged event.
        /// </summary>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="value">The value of the characteristics.</param>
        /// <param name="updateMockObjects">Whether to update the mock objects</param>
        private void addCharacteristicToDictionary(Guid uuid, byte[] value, bool shouldUpdateMockObjects)
        {
            characteristics[uuid] = value;
            BoardCharacteristicChanged?.Invoke(this, new BoardCharacteristicChangedEventArgs(uuid, value));

            if (shouldUpdateMockObjects)
            {
                updateMockObjects(uuid, value);
            }
        }

        private void updateMockObjects(Guid uuid, byte[] value)
        {
            if (uuid.Equals(CHARACTERISTIC_SPEED_RPM))
            {
                uint rpm = getCharacteristicAsUInt(value);

                if (rpm > getCharacteristicAsUInt(MOCK_TRIP_TOP_RPM))
                {
                    addCharacteristicToDictionary(MOCK_TRIP_TOP_RPM, value, false);
                }

                if (rpm > getCharacteristicAsUInt(MOCK_LIVETIME_TOP_RPM))
                {
                    addCharacteristicToDictionary(MOCK_LIVETIME_TOP_RPM, value, false);
                    Settings.setSetting(SettingsConsts.BOARD_TOP_RPM_LIVE, value);
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsConnected" when board.IsConnected:
                    loadCharacteristics();
                    break;
            }
        }

        private void INSTANCE_BoardChanged1(OnewheelConnectionHelper helper, BoardChangedEventArgs args)
        {
            setBoard(args.BOARD);
        }

        private void C_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] value = null;
            try
            {
                // Read value:
                value = readBytesFromBuffer(args.CharacteristicValue);
            }
            catch (Exception)
            {
            }

            // Insert characteristic and its value into a dictionary:
            addCharacteristicToDictionary(sender.Uuid, value, true);
        }

        #endregion
    }
}
