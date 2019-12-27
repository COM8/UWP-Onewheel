using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace OnewheelBluetooth.Classes.UnlockHelper
{
    public class OnewheelUnlockHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The Gemini firmware revision number for the Onewheel Plus.
        /// </summary>
        private readonly byte[] FIRMWARE_REVISION_OW_PLUS_BYTES = new byte[] { 0x0f, 0xc2 };

        /// <summary>
        /// The Gemini firmware revision number for the Onewheel Plus XR.
        /// </summary>
        private readonly byte[] FIRMWARE_REVISION_OW_PLUS_XR_BYTES = new byte[] { 0x10, 0x26 };

        /// <summary>
        /// A cache for the last 20 bytes received from the Onewheel.
        /// </summary>
        private readonly List<byte> SERIAL_READ_CACHE = new List<byte>(20);

        /// <summary>
        /// The period for sending the Gemini firmware revision to the board to keep it unlocked.
        /// If not send, the Onewheel looks up again after 24 seconds.
        /// </summary>
        public readonly TimeSpan UNLOCK_PERIOD = TimeSpan.FromSeconds(15);
        /// <summary>
        /// A timer for sending every 10 seconds the Gemini firmware revision to the board, to keep it unlocked.
        /// </summary>
        private ThreadPoolTimer unlockTimer = null;

        private bool disposed = false;

        private readonly OnewheelBoard ONEWHEEL;
        private ushort firmwareRevision;
        private AbstractOnewheelUnlock unlock;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OnewheelUnlockHelper(OnewheelBoard onewheel)
        {
            this.ONEWHEEL = onewheel;
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool IsOwPlusFirmwareRevision(byte[] data)
        {
            return data[0] == FIRMWARE_REVISION_OW_PLUS_BYTES[0] && data[1] == FIRMWARE_REVISION_OW_PLUS_BYTES[1];
        }

        private bool IsOwPlusXrFirmwareRevision(byte[] data)
        {
            return data[0] == FIRMWARE_REVISION_OW_PLUS_XR_BYTES[0] && data[1] == FIRMWARE_REVISION_OW_PLUS_XR_BYTES[1];
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Starts a new Task, requests the firmware revision and starts the unlock process.
        /// </summary>
        public void Start()
        {
            Task.Run(async () =>
            {
                Logger.Info("Requesting firmware revision for Gemini unlock...");
                byte[] data = await ONEWHEEL.ReadBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION);
                if (data is null || data.Length < 2)
                {
                    Logger.Info("Unlock failed - Gemini firmware does not match: " + (data is null ? "null" : Utils.ByteArrayToHexString(data)));
                }
                firmwareRevision = BitConverter.ToUInt16(data, 0);

                // Get the correct unlock mechanism based on the received firmware revision:
                if (firmwareRevision < 4100) // Andromeda
                {
                    Logger.Info("Received firmware revision (" + firmwareRevision + "). No unlock required.");
                    return;
                }
                else if (firmwareRevision < 4200) // Gemini
                {
                    unlock = new DefaultGeminiUnlock();
                    Logger.Info("Received firmware revision (" + firmwareRevision + ") for default Gemini unlock.");
                }
                else  // New Gemini and Pint
                {
                    unlock = new PintGeminiUnlock();
                    Logger.Info("Received firmware revision (" + firmwareRevision + ") for Pint Gemini unlock.");
                }

                // Start the process by subscribing to the serial read characteristic:
                bool result = await ONEWHEEL.SubscribeToCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ);
                if (!result)
                {
                    Logger.Error("Failed to unlock Onewheel - subscribe to serial read failed.");
                    return;
                }

                // Send the firmware revision back to trigger the first challenge:
                await ONEWHEEL.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION, data);
                Logger.Debug("Sent Gemini unlock firmware revision.");
            });
        }

        public void Dispose()
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
            StopUnlockTimer();
            disposed = true;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void AddSerialReadDataToCache(byte[] data)
        {
            if (SERIAL_READ_CACHE.Count > 20)
            {
                SERIAL_READ_CACHE.RemoveRange(0, data.Length);
            }
            SERIAL_READ_CACHE.AddRange(data);
        }

        private void StartUnlockTimer()
        {
            if (unlockTimer != null)
            {
                Logger.Warn("No need to Gemini start unlock timer - timer already running.");
                return;
            }
            unlockTimer = ThreadPoolTimer.CreatePeriodicTimer(OnUnlockTimeout, UNLOCK_PERIOD);
            Logger.Info("Started Gemini unlock timer with period: " + UNLOCK_PERIOD.TotalSeconds + " seconds.");
        }

        private async void OnUnlockTimeout(ThreadPoolTimer timer)
        {
            if (timer is null || disposed)
            {
                return;
            }
            try
            {
                await ONEWHEEL.WriteUShortAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION, firmwareRevision);
                Logger.Debug("Sent Gemini unlock firmware revision.");
            }
            catch (Exception e)
            {
                Logger.Error("Failed to send Gemini unlock firmware revision!", e);
            }
        }

        private void StopUnlockTimer()
        {
            unlockTimer?.Cancel();
            unlockTimer = null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ) && !(args.NEW_VALUE is null) && args.NEW_VALUE.Length > 0)
            {
                // Reverse back the byte order because we need the raw date send by the board:
                Utils.ReverseByteOrderIfNeeded(args.NEW_VALUE);

                AddSerialReadDataToCache(args.NEW_VALUE);
                if (unlock.CheckIfFirstChallengeBytesMatch(SERIAL_READ_CACHE) && SERIAL_READ_CACHE.Count == 20)
                {
                    await unlock.CalcAndSendResponseAsync(SERIAL_READ_CACHE, ONEWHEEL);

                    await ONEWHEEL.UnsubscribeFromCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ);
                    await ONEWHEEL.SubscribeToCharacteristicsAsync(OnewheelCharacteristicsCache.SUBSCRIBE_TO_CHARACTERISTICS);

                    // Send the Gemini firmware revision every UNLOCK_PERIOD to keep the board unlocked:
                    StartUnlockTimer();
                    OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
                }
            }
        }

        #endregion
    }
}
