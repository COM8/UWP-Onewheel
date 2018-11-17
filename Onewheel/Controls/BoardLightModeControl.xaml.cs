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
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetLightMode(uint lightMode)
        {
            if (this.lightMode != lightMode)
            {
                this.lightMode = lightMode;
                lightsOn_tggls.Toggled -= lightsOn_tggls_Toggled;
                lightsOn_tggls.IsOn = lightMode != 0;
                mode_tbx.Text = lightMode.ToString();
                lightsOn_tggls.Toggled += lightsOn_tggls_Toggled;
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
            lightsOn_tggls.IsEnabled = false;

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
                            ShowInfo("Light mode updated!", 5000);
                        }
                        else
                        {
                            ShowInfo("Failed to update light mode: " + (result == null ? "characteristic not found" : result.Status.ToString()), 5000);
                        }

                        lightsOn_tggls.IsEnabled = true;
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

        private void lightsOn_tggls_Toggled(object sender, RoutedEventArgs e)
        {
            uint lightMode = (uint)(lightsOn_tggls.IsOn ? 1 : 0);
            SetLightMode(lightMode);
            WriteLighMode(lightMode);
        }

        #endregion
    }
}
