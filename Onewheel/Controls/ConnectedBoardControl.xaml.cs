using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.Devices.Bluetooth;

namespace Onewheel.Controls
{
    public sealed partial class ConnectedBoardControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BluetoothLEDevice board;

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
        public void setBoard(BluetoothLEDevice board)
        {
            if (this.board != null)
            {
                this.board.ConnectionStatusChanged -= Board_ConnectionStatusChanged;
                this.board.NameChanged -= Board_NameChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                this.board.ConnectionStatusChanged += Board_ConnectionStatusChanged;
                this.board.NameChanged += Board_NameChanged;
            }

            showBoard();
            showRSSI();
            setVisability();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showBoard()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (board != null)
                {
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
                        name_tbx.Text = Settings.getSettingString(SettingsConsts.BOARD_NAME);
                        deviceId_tbx.Text = "-";
                    }
                }
            }).AsTask();
        }

        private void showRSSI()
        {
            //rssi_tbx.Text = board?.RSSI.ToString() ?? "-";
        }

        private void setVisability()
        {
            if (board != null && board.ConnectionStatus == BluetoothConnectionStatus.Connected)
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
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);
        }

        private void INSTANCE_BoardChanged(OnewheelConnectionHelper helper, BoardChangedEventArgs args)
        {
            setBoard(args.BOARD);
        }

        private void Board_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => setVisability()).AsTask();
        }

        private void Board_NameChanged(BluetoothLEDevice sender, object args)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => showBoard()).AsTask();
        }

        #endregion
    }
}
