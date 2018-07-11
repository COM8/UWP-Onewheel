using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Onewheel.Classes;
using Onewheel.Controls;
using System;
using System.Diagnostics;
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
        public void showInfo(string text, int duration)
        {
            info_notification.Show(text, duration);
        }

        #endregion

        #region --Misc Methods (Private)--
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

        private void showInt(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = value;
                }
                else
                {
                    boardInfoControl.Value = "0";
                }
            }).AsTask();
        }

        private void showSpeed(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double speed = Utils.rpmToKilometersPerHour(value);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = speed;
                }
                else
                {
                    boardInfoControl.Value = "0";
                }
            }).AsTask();
        }

        private void showDistance(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double distance = Utils.rpmToKilometers(value);
            distance = Math.Round(distance, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = distance;
                }
                else
                {
                    boardInfoControl.Value = "0";
                }
            }).AsTask();
        }

        private void showDistanceLive()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER);
            double distance = Utils.milesToKilometers(value);
            distance = Math.Round(distance, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    odometerLive_bic.Value = distance;
                }
                else
                {
                    odometerLive_bic.Value = "0";
                }
            }).AsTask();
        }

        private void showAmpere(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double amp = Utils.convertToAmpere(value);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = amp;
                }
                else
                {
                    boardInfoControl.Value = "0";
                }
            }).AsTask();
        }

        private void showVoltage(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double voltage = Utils.convertToVoltage(value);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = voltage;
                }
                else
                {
                    boardInfoControl.Value = "0";
                }
            }).AsTask();
        }

        private void showAmpereHours(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(uuid);
            double ampHours = value / 50.0;
            ampHours = Math.Round(ampHours, 2);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value >= 0)
                {
                    boardInfoControl.Value = ampHours;
                }
                else
                {
                    boardInfoControl.Value = "0";
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

        private void showMotorControllerTemperature()
        {
            byte[] value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getRawValue(OnewheelInfo.CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value != null)
                {
                    controllerTemp_bic.Value = (uint)value[0];
                    motorTemp_bic.Value = (uint)value[1];
                }
                else
                {
                    controllerTemp_bic.Value = "0";
                    motorTemp_bic.Value = "0";
                }
            }).AsTask();
        }

        private void showBatteryTemperature()
        {
            byte[] value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getRawValue(OnewheelInfo.CHARACTERISTIC_BATTERY_TEMPERATUR);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (value != null)
                {
                    batteryTemp_bic.Value = (uint)value[1];
                }
                else
                {
                    batteryTemp_bic.Value = "0";
                }
            }).AsTask();
        }

        private void showBatteryCellVoltages()
        {
            byte[] values = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getRawValue(OnewheelInfo.CHARACTERISTIC_BATTERY_CELL_VOLTAGES);
            if (values != null)
            {
                double[] voltages = Utils.convertToBatteryCellVoltages(values);

                string s = "";
                for (int i = 0; i < voltages.Length; i++)
                {
                    s += voltages[i] + ", ";
                }
            }
        }

        private void showRidingMode()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_RIDING_MODE);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ridingMode_brmc.Value = value).AsTask();
        }

        private void showLightingMode()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_LIGHTING_MODE);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => lightingMode_blmc.setLightMode(value)).AsTask();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.BoardCharacteristicChanged += ONEWHEEL_INFO_BoardCharacteristicChanged;

            // General:
            showBatteryLevel();
            showBoadName();

            // Speed:
            showSpeed(speed_bic, OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
            showSpeed(topSpeedTrip_bic, OnewheelInfo.MOCK_TRIP_TOP_RPM);
            showSpeed(topSpeedLive_bic, OnewheelInfo.MOCK_LIVETIME_TOP_RPM);

            // Distance:
            showDistanceLive();
            showDistance(odometerTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER);

            // Ampere:
            showAmpere(batteryAmpere_bic, OnewheelInfo.CHARACTERISTIC_BATTERY_CURRENT_AMPERE);

            // Voltage:
            showVoltage(batteryVoltage_bic, OnewheelInfo.CHARACTERISTIC_BATTERY_VOLTAGE);

            // Ampere hours:
            showAmpereHours(ampereHoursLive_bic, OnewheelInfo.CHARACTERISTIC_LIFETIME_AMPERE_HOURS);
            showAmpereHours(ampereHoursTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_AMPERE_HOURS);
            showAmpereHours(ampereHoursRegenTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS);

            // Temperature:
            showBatteryTemperature();
            showMotorControllerTemperature();

            // Battery voltages:
            showBatteryCellVoltages();

            // Riding mode:
            showRidingMode();

            // Lighting mode:
            showLightingMode();
        }

        private void ONEWHEEL_INFO_BoardCharacteristicChanged(OnewheelInfo sender, BoardCharacteristicChangedEventArgs args)
        {
            /*if(!args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_UART_SERIAL_READ))
            {
                string hex = BitConverter.ToString(args.VALUE);
                Debug.WriteLine("Read Value: " + hex + " " + args.UUID);
            }*/

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
                showSpeed(speed_bic, OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
            }
            else if (args.UUID.Equals(OnewheelInfo.MOCK_TRIP_TOP_RPM))
            {
                showSpeed(topSpeedTrip_bic, OnewheelInfo.MOCK_TRIP_TOP_RPM);
            }
            else if (args.UUID.Equals(OnewheelInfo.MOCK_LIVETIME_TOP_RPM))
            {
                showSpeed(topSpeedLive_bic, OnewheelInfo.MOCK_LIVETIME_TOP_RPM);
            }

            // Distance:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_LIFETIME_ODOMETER))
            {
                showDistanceLive();
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER))
            {
                showDistance(odometerTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_ODOMETER);
            }

            // Ampere:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_CURRENT_AMPERE))
            {
                showAmpere(batteryAmpere_bic, OnewheelInfo.CHARACTERISTIC_BATTERY_CURRENT_AMPERE);
            }

            // Voltage:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_VOLTAGE))
            {
                showVoltage(batteryVoltage_bic, OnewheelInfo.CHARACTERISTIC_BATTERY_VOLTAGE);
            }

            // Ampere hours:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_LIFETIME_AMPERE_HOURS))
            {
                showAmpereHours(ampereHoursLive_bic, OnewheelInfo.CHARACTERISTIC_LIFETIME_AMPERE_HOURS);
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_TRIP_AMPERE_HOURS))
            {
                showAmpereHours(ampereHoursTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_AMPERE_HOURS);
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS))
            {
                showAmpereHours(ampereHoursRegenTrip_bic, OnewheelInfo.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS);
            }

            // Temperature:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_TEMPERATUR))
            {
                showBatteryTemperature();
            }
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE))
            {
                showMotorControllerTemperature();
            }

            // Battery voltages:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_BATTERY_CELL_VOLTAGES))
            {
                showBatteryCellVoltages();
            }

            // Riding mode:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_RIDING_MODE))
            {
                showRidingMode();
            }

            // Lighting mode:
            else if (args.UUID.Equals(OnewheelInfo.CHARACTERISTIC_LIGHTING_MODE))
            {
                showLightingMode();
            }
        }

        private void editName_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
