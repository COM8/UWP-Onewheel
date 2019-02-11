using Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace OnewheelBluetooth.Classes
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
        /// The first three bytes of a challenge message from the Onewheel.
        /// </summary>
        private readonly byte[] CHALLENGE_FIRT_BYTES = new byte[] { 0x43, 0x52, 0x58 };

        /// <summary>
        /// The android challenge response password.
        /// Source: https://github.com/ponewheel/android-ponewheel/issues/86#issuecomment-440809066
        /// </summary>
        private readonly byte[] CHALLENGE_RESPONSE_PASSWORD = new byte[] { 0xD9, 0x25, 0x5F, 0x0F, 0x23, 0x35, 0x4E, 0x19, 0xBA, 0x73, 0x9C, 0xCD, 0xC4, 0xA9, 0x17, 0x65 };

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
        private OnewheelType type;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/11/2018 Created [Fabian Sauter]
        /// </history>
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
                Logger.Info("Requesting firmware version for Gemini unlock...");
                byte[] data = await ONEWHEEL.ReadBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION);

                if (!CheckForGeminiFirmwareRevision(data))
                {
                    Logger.Info("Unlock failed - Gemini firmware does not match: " + (data is null ? "null" : Utils.ByteArrayToHexString(data)));
                    return;
                }
                Logger.Info("Received firmware version for Gemini unlock.");

                bool result = await ONEWHEEL.SubscribeToCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ);
                if (!result)
                {
                    Logger.Error("Failed to unlock Onewheel - subscribe to serial read failed.");
                    return;
                }

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

        /// <summary>
        /// Calculates the response for the given challenge.
        /// Source: https://github.com/ponewheel/android-ponewheel/issues/86#issuecomment-440809066
        /// </summary>
        /// <param name="challenge">The challenge send by the Onewheel.</param>
        /// <returns>The response for the given challenge.</returns>
        public byte[] CalcResponse(byte[] challenge)
        {
            List<byte> response = new List<byte>(20);
            response.AddRange(CHALLENGE_FIRT_BYTES);

            byte[] md5In = new byte[16 + CHALLENGE_RESPONSE_PASSWORD.Length];
            Buffer.BlockCopy(challenge, 3, md5In, 0, 16);
            Buffer.BlockCopy(CHALLENGE_RESPONSE_PASSWORD, 0, md5In, 16, CHALLENGE_RESPONSE_PASSWORD.Length);

            MD5 md5 = MD5.Create();
            byte[] md5Out = md5.ComputeHash(md5In);
            response.AddRange(md5Out);

            // Calculate the validation byte:
            byte checkByte = 0;
            for (int i = 0; i < response.Count; i++)
            {
                checkByte = ((byte)(response[i] ^ checkByte));
            }
            response.Add(checkByte);

            return response.ToArray();
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool CheckForGeminiFirmwareRevision(byte[] data)
        {
            if (!(data is null) && data.Length >= 2)
            {
                if (IsOwPlusXrFirmwareRevision(data))
                {
                    type = OnewheelType.ONEWHEEL_PLUS_XR;
                    return true;
                }
                else if (IsOwPlusFirmwareRevision(data))
                {
                    type = OnewheelType.ONEWHEEL_PLUS;
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool CheckIfFirstChallengeBytesMatch()
        {
            return SERIAL_READ_CACHE.Count >= 3
                && SERIAL_READ_CACHE[0] == CHALLENGE_FIRT_BYTES[0]
                && SERIAL_READ_CACHE[1] == CHALLENGE_FIRT_BYTES[1]
                && SERIAL_READ_CACHE[2] == CHALLENGE_FIRT_BYTES[2];
        }

        private void AddSerialReadDataToCache(byte[] data)
        {
            if (SERIAL_READ_CACHE.Count > 20)
            {
                SERIAL_READ_CACHE.RemoveRange(0, data.Length);
            }
            SERIAL_READ_CACHE.AddRange(data);
        }

        private async Task CalcAndSendResponseAsync()
        {
            byte[] challenge = SERIAL_READ_CACHE.ToArray();
            byte[] response = CalcResponse(challenge);

            await ONEWHEEL.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_WRITE, response);
            Logger.Info("Sent Gemini unlock response to Onewheel challenge.");

            await ONEWHEEL.UnsubscribeFromCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ);

            await ONEWHEEL.SubscribeToCharacteristicsAsync(OnewheelCharacteristicsCache.SUBSCRIBE_TO_CHARACTERISTICS);

            // Send the Gemini firmware revision every UNLOCK_PERIOD to keep the board unlocked:
            StartUnlockTimer();
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
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
                if (type == OnewheelType.ONEWHEEL_PLUS)
                {
                    await ONEWHEEL.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION, FIRMWARE_REVISION_OW_PLUS_BYTES);
                }
                else
                {
                    await ONEWHEEL.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION, FIRMWARE_REVISION_OW_PLUS_XR_BYTES);
                }
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
                if (CheckIfFirstChallengeBytesMatch() && SERIAL_READ_CACHE.Count == 20)
                {
                    await CalcAndSendResponseAsync();
                }
            }
        }

        #endregion
    }
}
