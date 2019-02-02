using DataManager.Classes;
using OnewheelBluetooth.Classes;
using OnewheelBluetooth.Classes.Events;
using Shared.Classes;

namespace Onewheel_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class InfoTitleBarControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _BoardName;
        public string BoardName
        {
            get { return _BoardName; }
            set { SetProperty(ref _BoardName, value); }
        }
        private int _BatteryLevel;
        public int BatteryLevel
        {
            get { return _BatteryLevel; }
            set { SetProperty(ref _BatteryLevel, value); }
        }
        private bool _Connected;
        public bool Connected
        {
            get { return _Connected; }
            set { SetProperty(ref _Connected, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public InfoTitleBarControlDataTemplate()
        {
            BoardName = Settings.getSettingString(SettingsConsts.BOARD_NAME) ?? "";
            BatteryLevel = -1;
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;
            OnewheelConnectionHelper.INSTANCE.OnewheelConnectionHelperStateChanged += INSTANCE_OnewheelConnectionHelperStateChanged;
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
        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME))
            {
                BoardName = OnewheelCharacteristicsCache.GetString(args.NEW_VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_LEVEL))
            {
                BatteryLevel = (int)OnewheelCharacteristicsCache.GetUint(args.NEW_VALUE);
            }
        }

        private void INSTANCE_OnewheelConnectionHelperStateChanged(OnewheelConnectionHelper sender, OnewheelConnectionHelperStateChangedEventArgs args)
        {
            if (args.NEW_STATE == OnewheelConnectionHelperState.CONNECTED)
            {
                Connected = true;
            }
            else
            {
                Connected = false;
                BatteryLevel = -1;
            }
        }

        #endregion
    }
}
