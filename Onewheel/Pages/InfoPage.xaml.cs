using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Pages
{
    public sealed partial class InfoPage : Page
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
        public InfoPage()
        {
            this.board = null;
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

            showBattery();
            showBoard();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showBattery()
        {
            int level = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsInt(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL);
            if (level >= 0 && level <= 100)
            {
                batteryPercent_tbx.Text = level + "%";
                batteryIcon_tbx.Text = UIUtils.BATTERY_LEVEL_ICONS[level / 10];
            }
            else
            {
                batteryPercent_tbx.Text = "Unknown!";
                batteryIcon_tbx.Text = UIUtils.BATTERY_LEVEL_ICONS[11];
            }
        }

        private void showBoard()
        {
            if (board != null && board.BluetoothLEDevice != null)
            {
                name_tbx.Text = board.Name;
                btAddress_tbx.Text = board.BluetoothLEDevice.BluetoothAddress.ToString();
                btAddressType_tbx.Text = board.BluetoothLEDevice.BluetoothAddressType.ToString();
                deviceId_tbx.Text = board.BluetoothLEDevice.DeviceId;
                accessStatus_tbx.Text = board.BluetoothLEDevice.DeviceAccessInformation.CurrentStatus.ToString();
                connectionStatus_tbx.Text = board.BluetoothLEDevice.ConnectionStatus.ToString();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.BoardChanged += INSTANCE_BoardChanged;
            setBoard(OnewheelConnectionHelper.INSTANCE.board);
        }

        private void INSTANCE_BoardChanged(OnewheelConnectionHelper helper, BoardChangedEventArgs args)
        {
            setBoard(args.BOARD);
        }

        private void printAll_btn_Click(object sender, RoutedEventArgs e)
        {
            Task t = OnewheelConnectionHelper.INSTANCE.printAllAsync();
        }

        private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BluetoothAddressAsString":
                case "BluetoothLEDevice":
                case "Name":
                    showBattery();
                    showBoard();
                    break;
            }
        }

        private void reload_btn_Click(object sender, RoutedEventArgs e)
        {
            showBattery();
            showBoard();
        }

        #endregion
    }
}
