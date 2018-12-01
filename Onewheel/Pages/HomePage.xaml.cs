using DataManager.Classes;
using Logging;
using Onewheel.Classes;
using Onewheel.Controls;
using OnewheelBluetooth.Classes;
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
        public void ShowInfo(string text, int duration)
        {
            info_notification.Show(text, duration);
        }

        #endregion

        #region --Misc Methods (Private)--
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

        private void ShowInt(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);
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

        private void ShowSpeed(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);
            double speed = Utils.RpmToKilometersPerHour(value);

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

        private void ShowDistance(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);
            double distance = Utils.RpmToKilometers(value);
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

        private void ShowDistanceLive()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_LIFETIME_ODOMETER);
            double distance = Utils.MilesToKilometers(value);
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

        private void ShowAmpere(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);

            OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
            double amp = Utils.ToAmpere(value, onewheel is null ? OnewheelType.ONEWHEEL_PLUS : onewheel.TYPE);

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

        private void ShowVoltage(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);
            double voltage = Utils.ToVoltage(value);

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

        private void ShowAmpereHours(BoardInfoControl boardInfoControl, Guid uuid)
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(uuid);
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

        private void ShowMotorControllerTemperature()
        {
            byte[] value = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE);

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

        private void ShowBatteryTemperature()
        {
            byte[] value = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_TEMPERATUR);

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

        private void ShowBatteryCellVoltages()
        {
            byte[] values = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_CELL_VOLTAGES);
            if (values != null)
            {
                double[] voltages = Utils.ToBatteryCellVoltages(values);

                string s = "";
                for (int i = 0; i < voltages.Length; i++)
                {
                    s += voltages[i] + ", ";
                }
            }
        }

        private void ShowRidingMode()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ridingMode_brmc.Value = value).AsTask();
        }

        private void ShowLightingMode()
        {
            uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_LIGHTING_MODE);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => lightingMode_blmc.SetLightMode(value)).AsTask();
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

            // Speed:
            ShowSpeed(speed_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM);
            ShowSpeed(topSpeedTrip_bic, OnewheelCharacteristicsCache.MOCK_TRIP_TOP_RPM);
            ShowSpeed(topSpeedLive_bic, OnewheelCharacteristicsCache.MOCK_LIVETIME_TOP_RPM);

            // Distance:
            ShowDistanceLive();
            ShowDistance(odometerTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_ODOMETER);

            // Ampere:
            ShowAmpere(batteryAmpere_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_CURRENT_AMPERE);

            // Voltage:
            ShowVoltage(batteryVoltage_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_VOLTAGE);

            // Ampere hours:
            ShowAmpereHours(ampereHoursLive_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_LIFETIME_AMPERE_HOURS);
            ShowAmpereHours(ampereHoursTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_AMPERE_HOURS);
            ShowAmpereHours(ampereHoursRegenTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS);

            // Temperature:
            ShowBatteryTemperature();
            ShowMotorControllerTemperature();

            // Battery voltages:
            ShowBatteryCellVoltages();

            // Riding mode:
            ShowRidingMode();

            // Lighting mode:
            ShowLightingMode();
        }

        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, OnewheelBluetooth.Classes.Events.CharacteristicChangedEventArgs args)
        {
            // General:
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_LEVEL))
            {
                ShowBatteryLevel();
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME))
            {
                ShowBoadName();
            }

            // Speed:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM))
            {
                ShowSpeed(speed_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_TRIP_TOP_RPM))
            {
                ShowSpeed(topSpeedTrip_bic, OnewheelCharacteristicsCache.MOCK_TRIP_TOP_RPM);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_LIVETIME_TOP_RPM))
            {
                ShowSpeed(topSpeedLive_bic, OnewheelCharacteristicsCache.MOCK_LIVETIME_TOP_RPM);
            }

            // Distance:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_LIFETIME_ODOMETER))
            {
                ShowDistanceLive();
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_ODOMETER))
            {
                ShowDistance(odometerTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_ODOMETER);
            }

            // Ampere:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_CURRENT_AMPERE))
            {
                ShowAmpere(batteryAmpere_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_CURRENT_AMPERE);
            }

            // Voltage:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_VOLTAGE))
            {
                ShowVoltage(batteryVoltage_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_VOLTAGE);
            }

            // Ampere hours:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_LIFETIME_AMPERE_HOURS))
            {
                ShowAmpereHours(ampereHoursLive_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_LIFETIME_AMPERE_HOURS);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_AMPERE_HOURS))
            {
                ShowAmpereHours(ampereHoursTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_AMPERE_HOURS);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS))
            {
                ShowAmpereHours(ampereHoursRegenTrip_bic, OnewheelCharacteristicsCache.CHARACTERISTIC_TRIP_REGEN_AMPERE_HOURS);
            }

            // Temperature:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_TEMPERATUR))
            {
                ShowBatteryTemperature();
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_MOTOR_CONTROLLER_TEMPERATURE))
            {
                ShowMotorControllerTemperature();
            }

            // Battery voltages:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_CELL_VOLTAGES))
            {
                ShowBatteryCellVoltages();
            }

            // Riding mode:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE))
            {
                ShowRidingMode();
            }

            // Lighting mode:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_LIGHTING_MODE))
            {
                ShowLightingMode();
            }

            // Status:
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_STATUS))
            {
                uint value = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_STATUS);
                Logger.Debug("CHARACTERISTIC_STATUS: " + value);
            }
        }

        private void editName_btn_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion
    }
}
