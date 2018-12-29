using System;

namespace OnewheelBluetooth.Classes.Events
{
    public class CharacteristicChangedEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Guid UUID;
        public readonly byte[] NEW_VALUE;
        public readonly byte[] OLD_VALUE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public CharacteristicChangedEventArgs(Guid uuid, byte[] old_value, byte[] new_value)
        {
            this.UUID = uuid;
            this.NEW_VALUE = new_value;
            this.OLD_VALUE = old_value;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
