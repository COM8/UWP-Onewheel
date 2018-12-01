using Onewheel.Classes;
using Onewheel.Pages;
using OnewheelBluetooth.Classes;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class BoardLightModeControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private uint lightMode;
        private bool skipEvents;

        private HomePage homePage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/04/2018 Created [Fabian Sauter]
        /// </history>
		public BoardLightModeControl()
        {
            this.skipEvents = true;
            this.InitializeComponent();
            this.skipEvents = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetLightMode(uint lightMode)
        {
            if (this.lightMode != lightMode)
            {
                this.lightMode = lightMode;
                skipEvents = true;
                switch (lightMode)
                {
                    case Consts.LIGHT_MODE_OFF:
                        lightsOff_rbtn.IsChecked = true;
                        mode_tbx.Text = "Off";
                        break;

                    case Consts.LIGHT_MODE_AUTO:
                        lightsOn_rbtn.IsChecked = true;
                        mode_tbx.Text = "On";
                        break;

                    case Consts.LIGHT_MODE_CUSTOM:
                        lightsCustom_rbtn.IsChecked = true;
                        mode_tbx.Text = "Custom";
                        break;

                    default:
                        break;
                }
                skipEvents = false;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void ShowInfo(string text, int duration)
        {
            homePage?.ShowInfo(text, duration);
        }

        private void LoadHomePage()
        {
            homePage = UIUtils.FindParent<HomePage>(this);
        }

        private void WriteLighMode(uint lightMode)
        {
            Task.Run(async () =>
            {
                OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
                if (onewheel is null)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ShowInfo("Updating light mode failed - not connected!", 5000));
                }
                else
                {
                    GattWriteResult result = await onewheel.WriteShortAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_LIGHTING_MODE, (short)lightMode);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (result != null && result.Status == GattCommunicationStatus.Success)
                        {
                            ShowInfo("💡 Light mode updated!", 5000);
                        }
                        else
                        {
                            ShowInfo("Failed to update light mode: " + (result == null ? "characteristic not found" : result.Status.ToString()), 5000);
                        }
                    });
                }
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        private void LightsOff_rbtn_Click(object sender, RoutedEventArgs e)
        {
            if(!skipEvents)
            {
                SetLightMode(Consts.LIGHT_MODE_OFF);
                WriteLighMode(Consts.LIGHT_MODE_OFF);
            }
        }

        private void LightsOn_rbtn_Click(object sender, RoutedEventArgs e)
        {
            if (!skipEvents)
            {
                SetLightMode(Consts.LIGHT_MODE_AUTO);
                WriteLighMode(Consts.LIGHT_MODE_AUTO);
            }
        }

        private void LightsCustom_rbtn_Click(object sender, RoutedEventArgs e)
        {
            if (!skipEvents)
            {
                SetLightMode(Consts.LIGHT_MODE_CUSTOM);
                WriteLighMode(Consts.LIGHT_MODE_CUSTOM);
            }
        }

        #endregion
    }
}
