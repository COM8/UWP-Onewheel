using Onewheel.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Bluetooth;
using OnewheelBluetooth.Classes;

namespace Onewheel.Pages
{
    public sealed partial class InfoPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OnewheelBoard onewheel = null;

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
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetBoard(OnewheelBoard onewheel)
        {
            UnSubscribeFromEvents();
            this.onewheel = onewheel;
            SubscribeToEvents();

            ShowBattery();
            ShowBoard();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void SubscribeToEvents()
        {
            if (onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                board.NameChanged += Board_NameChanged;
            }
        }

        private void UnSubscribeFromEvents()
        {
            if (onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                board.NameChanged -= Board_NameChanged;
            }
        }

        private void ShowBattery()
        {
            uint level = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_BATTERY_LEVEL);
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

        private void ShowBoard()
        {
            if (onewheel != null)
            {
                BluetoothLEDevice board = onewheel.GetBoard();
                name_tbx.Text = board.Name;
                btAddress_tbx.Text = board.BluetoothAddress.ToString();
                btAddressType_tbx.Text = board.BluetoothAddressType.ToString();
                deviceId_tbx.Text = board.DeviceId;
                accessStatus_tbx.Text = board.DeviceAccessInformation.CurrentStatus.ToString();
                connectionStatus_tbx.Text = board.ConnectionStatus.ToString();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.OnewheelChanged += INSTANCE_OnewheelChanged;
            SetBoard(OnewheelConnectionHelper.INSTANCE.GetOnewheel());
        }

        private void INSTANCE_OnewheelChanged(OnewheelConnectionHelper sender, OnewheelBluetooth.Classes.Events.OnewheelChangedEventArgs args)
        {
            SetBoard(args.ONEWHEEL);
        }

        private async void printAll_btn_Click(object sender, RoutedEventArgs e)
        {
            OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
            await onewheel?.PrintAllCharacteristicsAsync();
        }

        private void Board_NameChanged(BluetoothLEDevice sender, object args)
        {
            ShowBattery();
            ShowBoard();
        }

        private void reload_btn_Click(object sender, RoutedEventArgs e)
        {
            ShowBattery();
            ShowBoard();
        }

        #endregion
    }
}
