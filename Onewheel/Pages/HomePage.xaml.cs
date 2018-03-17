using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Onewheel.Classes;
using Onewheel.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Pages
{
    public sealed partial class HomePage : Page
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
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public HomePage()
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
        private void showBatteryLevel()
        {
            uint level = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (level >= 0 && level <= 100)
                {
                    batteryPerc_tbx.Text = level + "%";
                    batteryIcon_tbx.Text = UIUtils.BATTERY_LEVEL_ICONS[level / 10];
                }
                else
                {
                    batteryPerc_tbx.Text = "Unknown";
                    batteryIcon_tbx.Text = UIUtils.BATTERY_LEVEL_ICONS[11];
                }
            }).AsTask();
        }

        private void showInt(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.ValueText = value.ToString();
                }
                else
                {
                    boardInfoControl.ValueText = "0";
                }
            }).AsTask();
        }

        private void showSpeed(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double speed = Utils.rpmToKilometersPerHour(value);
            speed = Math.Round(speed, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.ValueText = speed.ToString();
                }
                else
                {
                    boardInfoControl.ValueText = "0";
                }
            }).AsTask();
        }

        private void showDistance(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double distance = Utils.milesToKilometers(value);
            distance = Math.Round(distance, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.ValueText = distance.ToString();
                }
                else
                {
                    boardInfoControl.ValueText = "0";
                }
            }).AsTask();
        }

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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.BoardCharacteristicChanged += ONEWHEEL_INFO_BoardCharacteristicChanged;
            showBatteryLevel();
            showSpeed(speed_bic, OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
            showSpeed(topSpeedTrip_bic, OnewheelInfo.MOCK_TOP_RPM_TRIP);
            showSpeed(topSpeedLive_bic, OnewheelInfo.MOCK_TOP_RPM_LIVE);
            showDistance(odometerLive_bic, OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER);
            showDistance(odometerTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER);
            showBoadName();
        }

        private void ONEWHEEL_INFO_BoardCharacteristicChanged(OnewheelInfo sender, BoardCharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL))
            {
                showBatteryLevel();
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_SPEED_RPM))
            {
                showSpeed(speed_bic, OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
            }
            else if (args.UUID.Equals(OnewheelInfo.MOCK_TOP_RPM_TRIP))
            {
                showSpeed(topSpeedTrip_bic, OnewheelInfo.MOCK_TOP_RPM_TRIP);
            }
            else if (args.UUID.Equals(OnewheelInfo.MOCK_TOP_RPM_LIVE))
            {
                showSpeed(topSpeedLive_bic, OnewheelInfo.MOCK_TOP_RPM_LIVE);
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER))
            {
                showDistance(odometerLive_bic, OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER);
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER))
            {
                showDistance(odometerTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER);
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_CUSTOM_NAME))
            {
                showBoadName();
            }
        }

        #endregion

        private void editName_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
