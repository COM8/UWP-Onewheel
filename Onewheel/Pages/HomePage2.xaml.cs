using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Onewheel.Classes;
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
        private void showBoadName()
        {
            string name = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsString(OnewheelInfo.CHARACTERISTIC_CUSTOM_NAME);
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

        private void showSpeed(uint value)
        {
            double speed = Utils.rpmToKilometersPerHour(value);
            if (speed < 0)
            {
                speed = 0;
            }

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                speed_rg.Value = speed;
                speed_sg.addValue(value);
            }).AsTask();
        }

        private void showBatteryLevel()
        {
            uint level = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                bool isCharching = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.isCharging;
                if (level >= 0 && level <= 100)
                {
                    batteryPerc_tbx.Text = level + "%";
                    batteryIcon_tbx.Text = isCharching ? UIUtils.BATTERY_CHARCHING_LEVEL_ICONS[level / 10] : UIUtils.BATTERY_LEVEL_ICONS[level / 10];
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
            OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.BoardCharacteristicChanged -= ONEWHEEL_INFO_BoardCharacteristicChanged;
            OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.BoardCharacteristicChanged += ONEWHEEL_INFO_BoardCharacteristicChanged;

            // General:
            showBatteryLevel();
            showBoadName();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.BoardCharacteristicChanged -= ONEWHEEL_INFO_BoardCharacteristicChanged;
        }

        private void editName_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ONEWHEEL_INFO_BoardCharacteristicChanged(OnewheelInfo sender, BoardCharacteristicChangedEventArgs args)
        {
            // General:
            if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL))
            {
                showBatteryLevel();
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_CUSTOM_NAME))
            {
                showBoadName();
            }

            // Speed:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_SPEED_RPM))
            {
                showSpeed(sender.getCharacteristicAsUInt(args.UUID));
            }
        }

        #endregion
    }
}
