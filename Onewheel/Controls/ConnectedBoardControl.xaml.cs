using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class ConnectedBoardControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableBluetoothLEDevice board;

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
        public void setBoard(ObservableBluetoothLEDevice board)
        {
            if (this.board != null)
            {
                this.board.PropertyChanged -= Board_PropertyChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                this.board.PropertyChanged += Board_PropertyChanged;
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
            if (board != null)
            {
                name_tbx.Text = board.Name ?? "-";
                btAddress_tbx.Text = board.BluetoothAddressAsString ?? "-";
                deviceId_tbx.Text = board.BluetoothLEDevice?.DeviceId ?? "-";
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
        }

        private void showRSSI()
        {
            //rssi_tbx.Text = board?.RSSI.ToString() ?? "-";
        }

        private void setVisability()
        {
            if (board != null && board.IsConnected)
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

        private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BluetoothAddressAsString":
                case "Name":
                    showBoard();
                    break;

                case "BluetoothLEDevice":
                    setVisability();
                    showRSSI();
                    showBoard();
                    break;

                case "IsConnected":
                    setVisability();
                    break;

                case "RSSI":
                    showRSSI();
                    break;

            }
        }

        #endregion
    }
}
