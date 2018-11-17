namespace OnewheelBluetooth.Classes
{
    public class OnewheelStatus
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly bool RIDER_DETECTED = false;
        private readonly bool RIDER_DETECTED_PAD_1 = false;
        private readonly bool RIDER_DETECTED_PAD_2 = false;
        private readonly bool ICSU_FAULT = false;
        private readonly bool ICSV_FAULT = false;
        private readonly bool CHARGING = false;
        private readonly bool BMS_CTRL_COMMS = false;
        private readonly bool BROKEN_CAPACITOR = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelStatus(byte[] data)
        {
            if (data.Length > 0)
            {
                this.RIDER_DETECTED = IsBitSet(data[0], 0);
                this.RIDER_DETECTED_PAD_1 = IsBitSet(data[0], 1);
                this.RIDER_DETECTED_PAD_2 = IsBitSet(data[0], 2);
                this.ICSU_FAULT = IsBitSet(data[0], 3);
                this.ICSV_FAULT = IsBitSet(data[0], 4);
                this.CHARGING = IsBitSet(data[0], 5);
                this.BMS_CTRL_COMMS = IsBitSet(data[0], 6);
                this.BROKEN_CAPACITOR = IsBitSet(data[0], 7);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private bool IsBitSet(byte b, byte pos)
        {
            return (b & (1 << pos)) != 0;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
