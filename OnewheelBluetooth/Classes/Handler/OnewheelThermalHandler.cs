using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace OnewheelBluetooth.Classes.Handler
{
    class OnewheelThermalHandler : AbstractValueHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const uint TEMERATURE_THRESHOLD = 80;
        private const uint TOAST_POP_TIMEOUT_SECONDS = 300;
        public const ushort BATTERY_TEMP = 0;
        public const ushort MOTOR_TEMP = 1;
        public const ushort CONTROLLER_TEMP = 2;

        private DateTime[] lastToastPoped;
        private readonly ToastAudio ALARM_SOUND;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelThermalHandler()
        {
            this.ALARM_SOUND = new ToastAudio
            {
                Loop = true,
                Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm9")
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onTempChanged(ushort type, byte value, DateTime timestamp)
        {
            if(value > TEMERATURE_THRESHOLD)
            {
                tryPopToast(type, value, timestamp);
            }
        }

        public override void reset()
        {
            lastToastPoped = new DateTime[3];
            lastToastPoped[BATTERY_TEMP] = DateTime.Now;
            lastToastPoped[MOTOR_TEMP] = DateTime.Now;
            lastToastPoped[CONTROLLER_TEMP] = DateTime.Now;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void tryPopToast(ushort type, byte value, DateTime timestamp)
        {
            if (timestamp.Subtract(lastToastPoped[type]).TotalSeconds > TOAST_POP_TIMEOUT_SECONDS)
            {
                lastToastPoped[type] = timestamp;
                switch (type)
                {
                    case BATTERY_TEMP:
                        popToast(generateToastContent(value, "Battery"));
                        break;

                    case MOTOR_TEMP:
                        popToast(generateToastContent(value, "Motor"));
                        break;

                    default:
                        popToast(generateToastContent(value, "Controller"));
                        break;
                }
            }
        }

        private ToastContent generateToastContent(byte value, string name)
        {
            return new ToastContent()
            {
                Scenario = ToastScenario.Alarm,
                Audio = ALARM_SOUND,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Temperature alarm 🌡!"
                    },

                    new AdaptiveText()
                    {
                        Text = name + " temp is over " + TEMERATURE_THRESHOLD + "°C"
                    },

                    new AdaptiveText()
                    {
                        Text = value + "°C",
                    }
                }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButtonDismiss()
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
