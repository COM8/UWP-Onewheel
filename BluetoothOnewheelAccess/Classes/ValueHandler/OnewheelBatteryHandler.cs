using DataManager.Classes.DBManagers;
using DataManager.Classes.DBTables;
using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace BluetoothOnewheelAccess.Classes.ValueHandler
{
    class OnewheelBatteryHandler : AbstractValueHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private DateTime lastToastPoped5Percent;
        private DateTime lastToastPoped20Percent;
        private readonly ToastAudio ALARM_SOUND_5_PERCENT;
        private readonly ToastAudio ALARM_SOUND_20_PERCENT;

        private const uint BATTERY_THRESHOLD_5_PERCENT = 5;
        private const uint BATTERY_THRESHOLD_20_PERCENT = 20;
        private const uint TOAST_POP_TIMEOUT_SECONDS_5_PERCENT = 30;
        private const uint TOAST_POP_TIMEOUT_SECONDS_20_PERCENT = 30;

        private uint lastValue;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 16/07/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelBatteryHandler()
        {
            this.ALARM_SOUND_5_PERCENT = new ToastAudio
            {
                Loop = false,
                Src = new Uri("ms-winsoundevent:Notification.Default")
            };
            this.ALARM_SOUND_20_PERCENT = new ToastAudio
            {
                Loop = false,
                Src = new Uri("ms-winsoundevent:Notification.IM")
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void reset()
        {
            lastToastPoped5Percent = DateTime.MinValue;
            lastToastPoped20Percent = DateTime.MinValue;
            lastValue = uint.MaxValue;
        }

        public void onBatteryChargeLeftChanged(uint value, DateTime timestamp)
        {
            if(value == lastValue)
            {
                return;
            }

            if (value >= BATTERY_THRESHOLD_5_PERCENT && lastValue > BATTERY_THRESHOLD_5_PERCENT)
            {
                if(timestamp.Subtract(lastToastPoped5Percent).TotalSeconds  > TOAST_POP_TIMEOUT_SECONDS_5_PERCENT)
                {
                    lastToastPoped5Percent = timestamp;
                    popToast(generateToastContent(value));
                }
            }
            else if (value >= BATTERY_THRESHOLD_20_PERCENT && lastValue > BATTERY_THRESHOLD_20_PERCENT)
            {
                if (timestamp.Subtract(lastToastPoped20Percent).TotalSeconds > TOAST_POP_TIMEOUT_SECONDS_20_PERCENT)
                {
                    lastToastPoped20Percent = timestamp;
                    popToast(generateToastContent(value));
                }
            }

            MeasurementsDBManager.INSTANCE.setBatteryMeasurement(new BatteryTable()
            {
                dateTime = timestamp,
                value = value
            });

            lastValue = value;
        }

        #endregion

        #region --Misc Methods (Private)--
        private ToastContent generateToastContent(uint value)
        {
            return new ToastContent()
            {
                Scenario = ToastScenario.Default,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Battery alert 🔋!"
                    },

                    new AdaptiveText()
                    {
                        Text = value + "% left",
                    }
                }
                    }
                }
            };
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
