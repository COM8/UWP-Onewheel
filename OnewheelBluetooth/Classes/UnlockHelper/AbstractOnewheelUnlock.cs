using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnewheelBluetooth.Classes.UnlockHelper
{
    public abstract class AbstractOnewheelUnlock
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The first three bytes of a challenge message from the Onewheel.
        /// </summary>
        protected readonly byte[] CHALLENGE_FIRST_BYTES = new byte[] { 0x43, 0x52, 0x58 };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public abstract Task CalcAndSendResponseAsync(List<byte> serialReadCache, OnewheelBoard onewheel);

        public bool CheckIfFirstChallengeBytesMatch(List<byte> serialReadCache)
        {
            return serialReadCache.Count >= 3
                && serialReadCache[0] == CHALLENGE_FIRST_BYTES[0]
                && serialReadCache[1] == CHALLENGE_FIRST_BYTES[1]
                && serialReadCache[2] == CHALLENGE_FIRST_BYTES[2];
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
