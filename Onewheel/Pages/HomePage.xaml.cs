using DataManager.Classes;
using Onewheel.Classes;
using Onewheel.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            int level = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsInt(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL);
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
            int value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsInt(uuid);
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

        private void showSpeed()
        {
            int value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsInt(OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
            double speed = Utils.rpmToKilometersPerHour(value);
            speed = Math.Round(speed, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    speed_bic.ValueText = speed.ToString();
                }
                else
                {
                    speed_bic.ValueText = "0";
                }
            }).AsTask();
        }

        private void showDistance(BoardInfoControl boardInfoControl, Guid uuid)
        {
            int value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsInt(OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
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
            if(name == null)
            {
                name = Settings.getSettingString(SettingsConsts.BOARD_NAME);
            }

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if(name != null)
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
            showSpeed();
            showDistance(odometerLive_bic, OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER);
            showDistance(odometerTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER);
            showBoadName();
        }

        private void ONEWHEEL_INFO_BoardCharacteristicChanged(OnewheelInfo sender, Classes.Events.BoardCharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL))
            {
                showBatteryLevel();
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_SPEED_RPM))
            {
                showSpeed();
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
