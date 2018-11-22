using Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OnewheelBluetooth.Classes
{
    public class OnewheelUnlockHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The Gemini firmware revision number.
        /// </summary>
        private readonly byte[] FIRMWARE_REVISION_BYTES = new byte[] { 0x0f, 0xc2 };

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

        private readonly OnewheelBoard ONEWHEEL;

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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
        }

        /// <summary>
        /// Sends the Gemini firmware revision to the board to request a challenge.
        /// </summary>
        public async Task SendFirmwareRevisionAsync()
        {
            await ONEWHEEL.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_FIRMWARE_REVISION, FIRMWARE_REVISION_BYTES);
            Logger.Debug("Sent firmware revision.");
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
        private bool DoFirstBytesMatch()
        {
            return SERIAL_READ_CACHE.Count >= 2
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
            Logger.Info("Send response to Onewheel challenge.");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_READ) && args.VALUE.Length > 0)
            {
                AddSerialReadDataToCache(args.VALUE);
                if (DoFirstBytesMatch())
                {
                    await CalcAndSendResponseAsync();
                }
            }
        }

        #endregion
    }
}
