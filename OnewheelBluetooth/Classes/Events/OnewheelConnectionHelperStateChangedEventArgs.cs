namespace OnewheelBluetooth.Classes.Events
{
    public class OnewheelConnectionHelperStateChangedEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OnewheelConnectionHelperState OLD_STATE;
        public readonly OnewheelConnectionHelperState NEW_STATE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelConnectionHelperStateChangedEventArgs(OnewheelConnectionHelperState oldState, OnewheelConnectionHelperState newState)
        {
            this.OLD_STATE = oldState;
            this.NEW_STATE = newState;
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
