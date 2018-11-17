using Onewheel.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Onewheel.Pages;
using Onewheel.Classes;
using OnewheelBluetooth.Classes;

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
        private async Task ShowChangeRidingModeDialogAsync()
        {
            ChangeRidingModeDialog dialog = new ChangeRidingModeDialog();
            await dialog.ShowAsync();

            if (!dialog.canceled)
            {
                uint curSpeed = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM);
                if (curSpeed <= 0)
                {
                    OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
                    if (onewheel is null || onewheel.GetBoard().ConnectionStatus == Windows.Devices.Bluetooth.BluetoothConnectionStatus.Disconnected)
                    {
                        // Not connected:
                        ShowInfo("Not connected to Onewheel!", 5000);
                    }
                    else
                    {
                        SetRideMode(dialog.selectedRideMode);
                    }
                }
                else
                {
                    // To fast to change ride mode:
                    ShowInfo("To fast to change ride mode!", 5000);
                }
            }
        }

        private void SetRideMode(uint rideMode)
        {
            main_grid.IsTapEnabled = false;
            Task.Run(async () =>
            {
                OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
                if (onewheel is null)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ShowInfo("Failed to update ride mode: not connected!", 5000));
                }
                else
                {
                    GattWriteResult result = await onewheel.WriteShortAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE, (short)rideMode);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (result != null && result.Status == GattCommunicationStatus.Success)
                        {
                            ShowInfo("Ride mode updated!", 5000);
                        }
                        else
                        {
                            ShowInfo("Failed to update ride mode: " + (result == null ? "characteristic not found" : result.Status.ToString()), 5000);
                        }
                        main_grid.IsTapEnabled = true;
                    });
                }
            });

        }

        private void ShowInfo(string text, int duration)
        {
            homePage?.ShowInfo(text, duration);
        }

        private void loadHomePage()
        {
            homePage = UIUtils.FindParent<HomePage>(this);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ShowChangeRidingModeDialogAsync();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadHomePage();
        }

        #endregion
    }
}
