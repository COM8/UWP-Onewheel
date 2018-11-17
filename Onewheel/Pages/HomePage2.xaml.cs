using DataManager.Classes;
using Onewheel.Classes;
using OnewheelBluetooth.Classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Pages
{
    public sealed partial class HomePage2 : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 01/09/2018 Created [Fabian Sauter]
        /// </history>
        public HomePage2()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void ShowBoadName()
        {
            string name = OnewheelConnectionHelper.INSTANCE.CACHE.GetString(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME);
            if (name == null)
            {
                name = Settings.getSettingString(SettingsConsts.BOARD_NAME);
            }

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (name != null)
                {
                    boardName_tbx.Text = name;
                    editName_btn.Visibility = Visibility.Visible;
                }
                else
                {
                    boardName_tbx.Text = "";
                    editName_btn.Visibility = Visibility.Collapsed;
                }
            }).AsTask();
        }

        private void ShowSpeed()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM);
            double speed = Utils.RpmToKilometersPerHour(value);
            if (speed < 0)
            {
                speed = 0;
            }

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                speed_rg.Value = speed;
                speed_sg.AddValue(value);
            }).AsTask();
        }

        private void ShowBatteryLevel()
        {
            uint level = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_LEVEL);
            byte[] status = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.CHARACTERISTIC_STATUS);
            OnewheelStatus onewheelStatus = new OnewheelStatus(status);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (level >= 0 && level <= 100)
                {
                    batteryPerc_tbx.Text = level + "%";
                    batteryIcon_tbx.Text = onewheelStatus.CHARGING ? UIUtils.BATTERY_CHARCHING_LEVEL_ICONS[level / 10] : UIUtils.BATTERY_LEVEL_ICONS[level / 10];
                }
                else
                {
                    batteryPerc_tbx.Text = "Unknown";
                    batteryIcon_tbx.Text = UIUtils.BATTERY_LEVEL_ICONS[11];
                }
            }).AsTask();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;

            // General:
            ShowBatteryLevel();
            ShowBoadName();
        }

        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, OnewheelBluetooth.Classes.Events.CharacteristicChangedEventArgs args)
        {
            // Battery:
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_LEVEL))
            {
                ShowBatteryLevel();
            }

            // General:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME))
            {
                ShowBoadName();
            }

            // Speed:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM))
            {
                ShowSpeed();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
        }

        private void editName_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
