using DataManager.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.Devices.Bluetooth;
using OnewheelBluetooth.Classes;

namespace Onewheel.Controls
{
    public sealed partial class ConnectedBoardControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OnewheelBoard onewheel;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectedBoardControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetOnewheel(OnewheelBoard onewheel)
        {
            UnsubscribeFromEvents();
            this.onewheel = onewheel;
            SubscribeToEvents();

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ShowOnewheel();
                SetVisability();
            }).AsTask();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void SubscribeToEvents()
        {
            if (this.onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                board.ConnectionStatusChanged += Board_ConnectionStatusChanged;
                board.NameChanged += Board_NameChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (this.onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                board.ConnectionStatusChanged -= Board_ConnectionStatusChanged;
                board.NameChanged -= Board_NameChanged;
            }
        }

        private void ShowOnewheel()
        {
            if (onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                name_tbx.Text = board.Name ?? "-";
                btAddress_tbx.Text = board.BluetoothAddress.ToString() ?? "-";
                deviceId_tbx.Text = board.DeviceId ?? "-";
            }
            else
            {
                string btAddress = Settings.getSettingString(SettingsConsts.BOARD_ADDRESS);
                if (btAddress != null)
                {
                    btAddress_tbx.Text = btAddress;
                    name_tbx.Text = Settings.getSettingString(SettingsConsts.BOARD_NAME) ?? "";
                    deviceId_tbx.Text = Settings.getSettingString(SettingsConsts.BOARD_ID) ?? "";
                }
            }
        }

        private void SetVisability()
        {
            if (!(onewheel is null) && OnewheelConnectionHelper.INSTANCE.GetState() == OnewheelConnectionHelperState.CONNECTED)
            {
                connecting_prgr.Visibility = Visibility.Collapsed;
                connected_tbx.Visibility = Visibility.Visible;
            }
            else
            {
                connected_tbx.Visibility = Visibility.Collapsed;
                connecting_prgr.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.OnewheelChanged += INSTANCE_OnewheelChanged;
            SetOnewheel(OnewheelConnectionHelper.INSTANCE.GetOnewheel());
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.OnewheelChanged -= INSTANCE_OnewheelChanged;
        }

        private void INSTANCE_OnewheelChanged(OnewheelConnectionHelper sender, OnewheelBluetooth.Classes.Events.OnewheelChangedEventArgs args)
        {
            SetOnewheel(args.ONEWHEEL);
        }

        private async void Board_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => SetVisability());
        }

        private async void Board_NameChanged(BluetoothLEDevice sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ShowOnewheel());
        }

        #endregion
    }
}
