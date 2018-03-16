using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Onewheel.Pages
{
    public sealed partial class InfoPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableBluetoothLEDevice board;

        private readonly string[] BATTERY_LEVEL_ICONS = new string[] {
            "\uEBA0", // 0%
            "\uEBA1", // 10%
            "\uEBA2", // 20%
            "\uEBA3", // 30%
            "\uEBA4", // 40%
            "\uEBA5", // 50%
            "\uEBA6", // 60%
            "\uEBA7", // 70%
            "\uEBA8", // 80%
            "\uEBA9", // 90%
            "\uEBAA", // 100%
            "\uEC02", // Unknown
        };

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
            if(this.board != null)
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
            Task.Run(async () =>
            {
                int level = await OnewheelConnectionHelper.INSTANCE.getBatteryLevelAsync();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (level >= 0 && level <= 100)
                    {
                        batteryPercent_tbx.Text = level + "%";
                        batteryIcon_tbx.Text = BATTERY_LEVEL_ICONS[level / 10];
                    }
                    else
                    {
                        batteryPercent_tbx.Text = "Unknown!";
                        batteryIcon_tbx.Text = BATTERY_LEVEL_ICONS[11];
                    }
                });
            });
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

        private void INSTANCE_BoardChanged(OnewheelConnectionHelper helper, Classes.Events.BoardChangedEventArgs args)
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
