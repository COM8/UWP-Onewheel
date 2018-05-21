using BluetoothOnewheelAccess.Classes;
using Onewheel.Classes;
using Onewheel.Pages;
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
        public void setLightMode(uint lightMode)
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
        private void showInfo(string text, int duration)
        {
            homePage?.showInfo(text, duration);
        }

        private void loadHomePage()
        {
            homePage = UIUtils.findParent<HomePage>(this);
        }

        private void writeLighMode(uint lightMode)
        {
            lightsOn_tggls.IsEnabled = false;

            Task.Run(async () =>
            {
                GattWriteResult result = await OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.writeDataAsync((short)lightMode, OnewheelInfo.CHARACTERISTIC_LIGHTING_MODE);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (result != null && result.Status == GattCommunicationStatus.Success)
                    {
                        showInfo("Light mode updated!", 5000);
                    }
                    else
                    {
                        showInfo("Failed to update light mode: " + (result == null ? "characteristic not found" : result.Status.ToString()), 5000);
                    }

                    lightsOn_tggls.IsEnabled = true;
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadHomePage();
        }

        private void lightsOn_tggls_Toggled(object sender, RoutedEventArgs e)
        {
            uint lightMode = (uint)(lightsOn_tggls.IsOn ? 1 : 0);
            setLightMode(lightMode);
            writeLighMode(lightMode);
        }

        #endregion
    }
}
