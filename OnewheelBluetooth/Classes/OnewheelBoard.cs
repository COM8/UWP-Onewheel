using Logging;
using OnewheelBluetooth.Classes.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace OnewheelBluetooth.Classes
{
    public class OnewheelBoard : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly List<GattCharacteristic> SUBSCRIBED_CHARACTERISTICS = new List<GattCharacteristic>();
        private readonly CancellationTokenSource REQUEST_SUBS_CANCEL_TOKEN = new CancellationTokenSource();

        public readonly OnewheelType TYPE = OnewheelType.ONEWHEEL_PLUS; // Just the + is supported right now
        private readonly BluetoothLEDevice BOARD;
        public bool characteristicsLoaded = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        internal OnewheelBoard(BluetoothLEDevice board)
        {
            if (board is null)
            {
                throw new InvalidOperationException("Unable to create a new instance of Onewheel - BluetoothLEDevice is null");
            }
            this.BOARD = board;
            this.BOARD.ConnectionStatusChanged += BOARD_ConnectionStatusChanged;

            RequestCharacteristics();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public BluetoothLEDevice GetBoard()
        {
            return BOARD;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<GattWriteResult> WriteShortAsync(Guid uuid, short data)
        {
            GattCharacteristic c = SUBSCRIBED_CHARACTERISTICS.Find((car) => car.Uuid.CompareTo(uuid) == 0);
            if (c != null)
            {
                byte[] dataArr = BitConverter.GetBytes(data);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(dataArr);
                }

                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(dataArr);
                GattWriteResult result = await c.WriteValueWithResultAsync(buffer);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataArr);
                    }
                    OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(uuid, dataArr, true);
                }
                return result;
            }
            return null;
        }

        public async Task<byte[]> ReadBytesAsync(GattCharacteristic characteristic)
        {
            GattReadResult vRes = await characteristic.ReadValueAsync();
            if (vRes.Status == GattCommunicationStatus.Success)
            {
                return ReadBytesFromBuffer(vRes.Value);
            }
            return null;
        }

        public void Dispose()
        {
            REQUEST_SUBS_CANCEL_TOKEN?.Cancel();
        }

        public async Task PrintAllCharacteristicsAsync()
        {
            Logger.Info("=======================================================");
            Logger.Info("Status: " + BOARD.ConnectionStatus);
            if (BOARD.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                GattDeviceServicesResult sResult = await BOARD.GetGattServicesAsync();
                if (sResult.Status == GattCommunicationStatus.Success)
                {
                    foreach (GattDeviceService s in sResult.Services)
                    {
                        Logger.Info("UUID: " + s.Uuid + ", Handle: " + s.AttributeHandle);

                        GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                        if (cResult.Status == GattCommunicationStatus.Success)
                        {
                            foreach (GattCharacteristic c in cResult.Characteristics)
                            {
                                await Utils.PrintAsync(c);
                            }
                        }
                    }
                }
            }
            Logger.Info("=======================================================");
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Starts a new Task and tries to subscribe to all characteristics stored in SUBSCRIBE_TO_CHARACTERISTICS.
        /// Can only be called once!
        /// </summary>
        private void RequestCharacteristics()
        {
            if (characteristicsLoaded)
            {
                Logger.Warn("Requesting characteristics failed - already loaded!");
                return;
            }
            characteristicsLoaded = true;

            Logger.Debug("Requesting characteristics for: " + BOARD.DeviceId);
            Task.Run(async () =>
            {
                try
                {
                    // Get all services:
                    GattDeviceServicesResult sResult = await BOARD.GetGattServicesAsync();
                    if (sResult.Status == GattCommunicationStatus.Success)
                    {
                        SUBSCRIBED_CHARACTERISTICS.Clear();
                        foreach (GattDeviceService s in sResult.Services)
                        {
                            // Get all characteristics:
                            GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                            if (cResult.Status == GattCommunicationStatus.Success)
                            {
                                foreach (GattCharacteristic c in cResult.Characteristics)
                                {
                                    await LoadCharacteristicValueAsync(c);

                                    if (OnewheelCharacteristicsCache.SUBSCRIBE_TO_CHARACTERISTICS.Contains(c.Uuid))
                                    {
                                        await SubscribeToCharacteristicAsync(c);
                                    }
                                    else
                                    {
                                        //Logger.Debug("New characteristic found: " + c.Uuid);
                                        //await Utils.printGattCharacteristicAsync(c);
                                    }
                                }
                            }
                        }
                        Logger.Debug("Finished requesting characteristics for: " + BOARD.DeviceId);
                    }
                    else
                    {
                        Logger.Warn("Failed to request GetGattServicesAsync() for " + BOARD.DeviceId + " - " + sResult.Status.ToString());
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Error during requesting characteristics for: " + BOARD.DeviceId, e);
                }
                Logger.Debug("Finished requesting characteristics for: " + BOARD.DeviceId);
            }, REQUEST_SUBS_CANCEL_TOKEN.Token);
        }

        /// <summary>
        /// Loads the characteristics value from the given GattCharacteristic
        /// and adds the value to the characteristics dictionary.
        /// </summary>
        /// <param name="c">The characteristic the value should get added to the characteristics dictionary.</param>
        private async Task LoadCharacteristicValueAsync(GattCharacteristic c)
        {
            byte[] value = null;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
            {
                try
                {
                    // Load value from characteristic:
                    value = await ReadBytesAsync(c);
                }
                catch (Exception)
                {
                }
            }

            // Insert characteristic and its value into a dictionary:
            OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(c.Uuid, value, true);
        }

        /// <summary>
        /// Subscribes to the given GattCharacteristic if it allows it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to subscribe to.</param>
        /// <returns></returns>
        private async Task SubscribeToCharacteristicAsync(GattCharacteristic c)
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
                // Add characteristic to global field to prevent it getting disposed:
                SUBSCRIBED_CHARACTERISTICS.Add(c);

                c.ValueChanged -= C_ValueChanged;
                c.ValueChanged += C_ValueChanged;
            }
        }

        /// <summary>
        /// Reads all available bytes from the given buffer and converts them to big endian if necessary. 
        /// </summary>
        /// <param name="buffer">The buffer object you want to read from.</param>
        /// <returns>All available bytes from the buffer in big endian.</returns>
        private byte[] ReadBytesFromBuffer(IBuffer buffer)
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

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // Read bytes:
            byte[] bytes = ReadBytesFromBuffer(args.CharacteristicValue);

            // Insert characteristic and its value into a dictionary:
            OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(sender.Uuid, bytes, args.Timestamp.DateTime, true);
        }

        private void BOARD_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (!characteristicsLoaded && sender.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                RequestCharacteristics();
            }
        }

        #endregion
    }
}
