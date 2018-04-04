using Onewheel.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using BluetoothOnewheelAccess.Classes;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Onewheel.Pages;
using Onewheel.Classes;

namespace Onewheel.Controls
{
    public sealed partial class BoardRidingModeControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BoardRidingModeControl), null);

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
		public BoardRidingModeControl()
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
        private async Task showChangeRidingModeDialogAsync()
        {
            ChangeRidingModeDialog dialog = new ChangeRidingModeDialog();
            await dialog.ShowAsync();

            if (!dialog.canceled)
            {
                uint curSpeed = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
                if(curSpeed <= 0)
                {
                    if (OnewheelConnectionHelper.INSTANCE.state == OnewheelConnectionState.CONNECTED)
                    {
                        await setRideModeAsync(dialog.selectedRideMode);
                    }
                    else
                    {
                        // Not connected:
                        showInfo("Not connected to Onewheel!", 5000);
                    }
                }
                else
                {
                    // To fast to change ride mode:
                    showInfo("To fast to change ride mode!", 5000);
                }
            }
        }

        private async Task setRideModeAsync(uint rideMode)
        {
            GattWriteResult result = await OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.writeDataAsync((short)rideMode, OnewheelInfo.CHARACTERISTIC_RIDING_MODE);
            if (result != null && result.Status == GattCommunicationStatus.Success)
            {
                showInfo("Ride mode updated!", 5000);
            }
            else
            {
                showInfo("Failed to update ride mode: " + (result == null ? "characteristic not found" : result.Status.ToString()), 5000);
            }
        }

        private void showInfo(string text, int duration)
        {
            homePage?.showInfo(text, duration);
        }

        private void loadHomePage()
        {
            homePage = UIUtils.findParent<HomePage>(this);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await showChangeRidingModeDialogAsync();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadHomePage();
        }

        #endregion
    }
}
