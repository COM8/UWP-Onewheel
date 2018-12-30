namespace OnewheelBluetooth.Classes
{
    public static class Consts
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const uint RIDING_MODE_OW_CLASSIC_CLASSIC = 1;
        public const uint RIDING_MODE_OW_CLASSIC_XTREME = 2;
        public const uint RIDING_MODE_OW_CLASSIC_ELEVATE = 3;

        public const uint RIDING_MODE_OW_PLUS_SEQUOIA = 4;
        public const uint RIDING_MODE_OW_PLUS_CRUZ = 5;
        public const uint RIDING_MODE_OW_PLUS_MISSION = 6;
        public const uint RIDING_MODE_OW_PLUS_ELEVATE = 7;
        public const uint RIDING_MODE_OW_PLUS_DELIRIUM = 8;
        public const uint RIDING_MODE_OW_PLUS_CUSTOM_SHAPING = 9;

        public const uint CUSTOM_LIGHT_LEVEL_MIN = 0;
        public const uint CUSTOM_LIGHT_LEVEL_MAX = 75;

        public const uint LIGHT_MODE_OFF = 0;
        public const uint LIGHT_MODE_AUTO = 1;
        public const uint LIGHT_MODE_CUSTOM = 2;

        public const byte CUSTOM_SHAPING_IDENT_STANCE_PROFILE = 0;
        public const byte CUSTOM_SHAPING_IDENT_CARVE_ABILITY = 1;
        public const byte CUSTOM_SHAPING_IDENT_AGGRESSIVENESS = 2;

        public const double CUSTOM_SHAPING_STEP_STANCE_PROFILE = 20;
        public const sbyte CUSTOM_SHAPING_STEP_CARVE_ABILITY = 20;

        public const sbyte CUSTOM_SHAPING_MIN_STANCE_PROFILE = -20;
        public const sbyte CUSTOM_SHAPING_MIN_CARVE_ABILITY = -100;
        public const sbyte CUSTOM_SHAPING_MIN_AGGRESSIVENESS = -80;

        public const sbyte CUSTOM_SHAPING_MAX_STANCE_PROFILE = 60;
        public const sbyte CUSTOM_SHAPING_MAX_CARVE_ABILITY = 100;
        public const sbyte CUSTOM_SHAPING_MAX_AGGRESSIVENESS = 127;

        public static readonly sbyte[] CUSTOM_SHAPING_VALUES_AGGRESSIVENESS = new sbyte[] { -80, -59, -39, -18, 3, 24, 44, 64, 86, 106, 127 };

        public const sbyte CUSTOM_SHAPING_DEFAULT_STANCE_PROFILE = 0;
        public const sbyte CUSTOM_SHAPING_DEFAULT_CARVE_ABILITY = 0;
        public const sbyte CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS = 0;

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
