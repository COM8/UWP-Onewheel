using Logging;
using OnewheelBluetooth.Classes.UnlockHelper;
using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly Dictionary<Guid, GattCharacteristic> ONEWHEEL_CHARACTERISTICS = new Dictionary<Guid, GattCharacteristic>();
        private readonly CancellationTokenSource REQUEST_SUBS_CANCEL_TOKEN = new CancellationTokenSource();

        public OnewheelType owType = OnewheelType.UNKNOWN;
        private readonly OnewheelUnlockHelper UNLOCK_HELPER;
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
            this.UNLOCK_HELPER = new OnewheelUnlockHelper(this);

            if (board.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                // Old firmware request all characteristics:
                RequestCharacteristics();
            }
            else
            {
                RequestCharacteristics();
            }
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
        /// <summary>
        /// Subscribes to the given GattCharacteristic if it allows it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to subscribe to.</param>
        /// <returns>Returns true on success.</returns>
        public async Task<bool> SubscribeToCharacteristicAsync(Guid uuid)
        {
            ONEWHEEL_CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            if (c != null)
            {
                return await SubscribeToCharacteristicAsync(c);
            }
            return false;
        }

        /// <summary>
        /// Unsubscribes from the given GattCharacteristic if subscribed to it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to unsubscribe from.</param>
        /// <returns>Returns true on success.</returns>
        public async Task<bool> UnsubscribeFromCharacteristicAsync(Guid uuid)
        {
            ONEWHEEL_CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            if (c != null)
            {
                return await UnsubscribeFromCharacteristicAsync(c);
            }
            return false;
        }

        public async Task<GattWriteResult> WriteShortAsync(Guid uuid, short data)
        {
            byte[] dataArr = BitConverter.GetBytes(data);
            Utils.ReverseByteOrderIfNeeded(dataArr);

            return await WriteBytesAsync(uuid, dataArr);
        }

        public async Task<GattWriteResult> WriteUShortAsync(Guid uuid, ushort data)
        {
            byte[] dataArr = BitConverter.GetBytes(data);
            Utils.ReverseByteOrderIfNeeded(dataArr);

            return await WriteBytesAsync(uuid, dataArr);
        }

        public async Task<GattWriteResult> WriteStringAsync(Guid uuid, string data)
        {
            return await WriteBytesAsync(uuid, Encoding.ASCII.GetBytes(data));
        }

        public async Task<GattWriteResult> WriteBytesAsync(Guid uuid, byte[] data)
        {
            ONEWHEEL_CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            if (c != null)
            {
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(data);
                GattWriteResult result = await c.WriteValueWithResultAsync(buffer);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    // Convert to little endian:
                    Utils.ReverseByteOrderIfNeeded(data);
                    OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(uuid, data, true);
                }
                return result;
            }
            else
            {
                Logger.Warn("Failed to write to write bytes to: " + uuid.ToString() + " - not loaded.");
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

        public async Task<byte[]> ReadBytesAsync(Guid uuid)
        {
            ONEWHEEL_CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            if (c != null)
            {
                return await ReadBytesAsync(c);
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

        public async Task SubscribeToCharacteristicsAsync(Guid[] uuids)
        {
            foreach (Guid uuid in uuids)
            {
                if (ONEWHEEL_CHARACTERISTICS.ContainsKey(uuid))
                {
                    GattCharacteristic c = ONEWHEEL_CHARACTERISTICS[uuid];
                    if (await SubscribeToCharacteristicAsync(c))
                    {
                        await LoadCharacteristicValueAsync(c);
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Starts a new Task and tries to load all characteristics.
        /// Can only be called once!
        /// </summary>
        internal void RequestCharacteristics()
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
                        ONEWHEEL_CHARACTERISTICS.Clear();
                        foreach (GattDeviceService s in sResult.Services)
                        {
                            // Get all characteristics:
                            GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                            if (cResult.Status == GattCommunicationStatus.Success)
                            {
                                foreach (GattCharacteristic c in cResult.Characteristics)
                                {
                                    ONEWHEEL_CHARACTERISTICS.Add(c.Uuid, c);
                                }
                            }
                        }
                        Logger.Debug("Finished requesting characteristics for: " + BOARD.DeviceId);

                        // Run unlock:
                        UNLOCK_HELPER.Start();
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
        /// Subscribes to the given GattCharacteristic if it allows it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to subscribe to.</param>
        /// <returns>Returns true on success.</returns>
        private async Task<bool> SubscribeToCharacteristicAsync(GattCharacteristic c)
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
                return false;
            }

            // Set subscribed:
            GattCommunicationStatus status;
            try
            {
                status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            // Add event handler:
            if (status == GattCommunicationStatus.Success)
            {
                c.ValueChanged -= C_ValueChanged;
                c.ValueChanged += C_ValueChanged;
                Logger.Debug("Subscribed to characteristic: " + c.Uuid);
                return true;
            }
            else
            {
                Logger.Warn("Failed to subscribe to characteristic " + c.Uuid + " with " + status);
            }
            return false;
        }

        /// <summary>
        /// Unsubscribes from the given GattCharacteristic if subscribed to it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to unsubscribe from.</param>
        /// <returns>Returns true on success.</returns>
        private async Task<bool> UnsubscribeFromCharacteristicAsync(GattCharacteristic c)
        {
            try
            {
                GattCommunicationStatus status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);

                // Add event handler:
                if (status == GattCommunicationStatus.Success)
                {
                    c.ValueChanged -= C_ValueChanged;
                    Logger.Debug("Unsubscribed from characteristic: " + c.Uuid);
                    return true;
                }
                else
                {
                    Logger.Warn("Failed to unsubscribe from characteristic " + c.Uuid + " with " + status);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        /// <summary>
        /// Loads the characteristics value from the given GattCharacteristic
        /// and adds the value to the characteristics dictionary.
        /// </summary>
        /// <param name="c">The characteristic the value should get added to the characteristics dictionary.</param>
        private async Task LoadCharacteristicValueAsync(GattCharacteristic c)
        {
            byte[] data = null;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
            {
                try
                {
                    // Load value from characteristic:
                    data = await ReadBytesAsync(c);

                    if (!(data is null))
                    {
                        // Convert to little endian:
                        Utils.ReverseByteOrderIfNeeded(data);
                        OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(c.Uuid, data, true);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Loading value from characteristic " + c.Uuid + " failed!", e);
                }
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
            return data;
        }

        private void TryLoadingOnewheelType(byte[] hardwareRevisionArray)
        {
            if (hardwareRevisionArray is null || hardwareRevisionArray.Length < 2)
            {
                return;
            }

            ushort hardwareRevision = BitConverter.ToUInt16(hardwareRevisionArray, 0);
            // Based on: https://github.com/OnewheelCommunityEdition/OWCE_App/blob/4fc1923323543db849deccbf2998646cdf1bae31/OWCE/OWCE/OWBoard.cs#L458-L473
            if (hardwareRevision >= 1 && hardwareRevision <= 2999)
            {
                owType = OnewheelType.ONEWHEEL;
                Logger.Info("Received hardware revision of an original Onewheel.");
            }
            else if (hardwareRevision >= 3000 && hardwareRevision <= 3999)
            {
                owType = OnewheelType.ONEWHEEL_PLUS;
                Logger.Info("Received hardware revision of an Onewheel+.");
            }
            else if (hardwareRevision >= 4000 && hardwareRevision <= 4999)
            {
                owType = OnewheelType.ONEWHEEL_PLUS_XR;
                Logger.Info("Received hardware revision of an Onewheel+ XR.");
            }
            else if (hardwareRevision >= 5000 && hardwareRevision <= 5999)
            {
                owType = OnewheelType.ONEWHEEL_PINT;
                Logger.Info("Received hardware revision of a Pint.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // Read bytes:
            byte[] data = ReadBytesFromBuffer(args.CharacteristicValue);

            // Convert to little endian:
            Utils.ReverseByteOrderIfNeeded(data);

            // Try loading the Onewheel type:
            if (sender.Uuid.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_HARDWARE_REVISION))
            {
                TryLoadingOnewheelType(data);
            }

            // Insert characteristic and its value into a dictionary:
            OnewheelConnectionHelper.INSTANCE.CACHE.AddToDictionary(sender.Uuid, data, args.Timestamp.DateTime, true);
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
