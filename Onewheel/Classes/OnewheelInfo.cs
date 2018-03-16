﻿using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Onewheel.Classes
{
    public class OnewheelInfo
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // UUID source: https://github.com/ponewheel/android-ponewheel/blob/master/app/src/main/java/net/kwatts/powtools/model/OWDevice.java
        public static readonly Guid CHARACTERISTIC_SERIAL_NUMBER = Guid.Parse("e659F301-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_RIDING_MODE = Guid.Parse("e659f302-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LEVEL = Guid.Parse("e659f303-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_5 = Guid.Parse("e659f304-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LOW_20 = Guid.Parse("e659f305-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_SERIAL_NUMBER = Guid.Parse("e659f306-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_TEMPERATUR = Guid.Parse("e659f315-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_VOLTAGE = Guid.Parse("e659f316-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_CURRENT_AMPERE = Guid.Parse("e659f312-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_LIFETIME_AMPERE_HOURS = Guid.Parse("e659f31a-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_TRIP_AMPERE_HOURS = Guid.Parse("e659f314-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_BATTERY_TRIP_REGEN_AMPERE_HOURS = Guid.Parse("e659f313-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_TEMPERATURE = Guid.Parse("e659f310-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_CUSTOM_NAME = Guid.Parse("e659f3fd-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_FIRMWARE_REVISION = Guid.Parse("e659f311-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_HARDWARE_REVISION = Guid.Parse("e659f318-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_MODE = Guid.Parse("e659f30c-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_BACK = Guid.Parse("e659f30e-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_LIGHTING_FRONT = Guid.Parse("e659f30d-ea98-11e3-ac10-0800200c9a66");
        public static readonly Guid CHARACTERISTIC_SPEED_RPM = Guid.Parse("e659f30b-ea98-11e3-ac10-0800200c9a66");

        private Dictionary<Guid, byte[]> characteristics;
        private ObservableBluetoothLEDevice board;

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

        public string getCharacteristicAsString(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            if (value != null)
            {
                return System.Text.Encoding.ASCII.GetString(value);
            }
            return null;
        }

        public int getCharacteristicAsInt(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            if (value != null)
            {
                switch (value.Length)
                {
                    case 2:
                        return BitConverter.ToInt16(value, 0);

                    case 4:
                        return BitConverter.ToInt32(value, 0);

                    default:
                        return -2;
                }
            }
            return -1;
        }

        public long getCharacteristicAsLong(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            if (value != null)
            {
                if (value.Length == 8)
                {
                    return BitConverter.ToInt64(value, 0);
                }
                else
                {
                    return -2;
                }
            }
            return -1;
        }

        public bool getCharacteristicAsBool(Guid uuid)
        {
            characteristics.TryGetValue(uuid, out byte[] value);
            if (value != null)
            {
                if (value.Length == 1)
                {
                    return BitConverter.ToBoolean(value, 0);
                }
            }
            return false;
        }

        public void init()
        {
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged1;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);
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
        private void loadBoard()
        {
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);
        }

        private void INSTANCE_BoardChanged(OnewheelConnectionHelper helper, Events.BoardChangedEventArgs args)
        {
            setBoard(args.BOARD);
        }

        private void loadCharacteristics()
        {
            if (board != null && board.IsConnected)
            {
                Task.Run(async () =>
                {
                    if (board != null && board.BluetoothLEDevice != null)
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

                                        // Subscribe to value changes:
                                        if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                                        {
                                            c.ValueChanged -= C_ValueChanged;
                                            c.ValueChanged += C_ValueChanged;
                                        }
                                    }
                                }
                            }
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
            addCharacteristicToDictionary(c.Uuid, value);
        }

        public void addCharacteristicToDictionary(Guid uuid, byte[] value)
        {
            characteristics.Add(uuid, value);
            BoardCharacteristicChanged?.Invoke(this, new BoardCharacteristicChangedEventArgs(uuid, value));
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
                case "IsConnected":
                    loadCharacteristics();
                    break;
            }
        }

        private void INSTANCE_BoardChanged1(OnewheelConnectionHelper helper, Events.BoardChangedEventArgs args)
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
            addCharacteristicToDictionary(sender.Uuid, value);
        }

        #endregion
    }
}
