using Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OnewheelBluetooth.Classes.UnlockHelper
{
    public class DefaultGeminiUnlock : AbstractOnewheelUnlock
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The android challenge response password.
        /// Source: https://github.com/ponewheel/android-ponewheel/issues/86#issuecomment-440809066
        /// </summary>
        private readonly byte[] CHALLENGE_RESPONSE_PASSWORD = new byte[] { 0xD9, 0x25, 0x5F, 0x0F, 0x23, 0x35, 0x4E, 0x19, 0xBA, 0x73, 0x9C, 0xCD, 0xC4, 0xA9, 0x17, 0x65 };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override async Task CalcAndSendResponseAsync(List<byte> serialReadCache, OnewheelBoard onewheel)
        {
            byte[] challenge = serialReadCache.ToArray();
            byte[] response = CalcResponse(challenge);

            await onewheel.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_WRITE, response);
            Logger.Info("Sent Gemini unlock response to Onewheel challenge.");
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
            response.AddRange(CHALLENGE_FIRST_BYTES);

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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--



        #endregion
    }
}
