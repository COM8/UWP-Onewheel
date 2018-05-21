﻿using BluetoothOnewheelAccess.Classes;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes.DBManagers;
using DataManager.Classes.DBTables;
using Onewheel.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.Devices.Bluetooth;

namespace Onewheel.Pages
{
    public sealed partial class InfoPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BluetoothLEDevice board;
        private ObservableCollection<SpeedTable> speedValues;

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
            this.speedValues = new ObservableCollection<SpeedTable>();
            loadSpeedValues();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setBoard(BluetoothLEDevice board)
        {
            if (this.board != null)
            {
                this.board.NameChanged -= Board_NameChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                this.board.NameChanged += Board_NameChanged;
            }

            showBattery();
            showBoard();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadSpeedValues()
        {
            Task.Run(async () =>
            {
                IEnumerable<SpeedTable> values = MeasurementsDBManager.INSTANCE.getAllSpeedMeasurement().Where(x => x.dateTime.Date.CompareTo(DateTime.Now.Date) == 0);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    speedValues.Clear();
                    foreach (SpeedTable v in values)
                    {
                        speedValues.Add(v);
                    }
                });
            });
        }

        private void showBattery()
        {
            uint level = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_BATTERY_LEVEL);
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
            if (board != null)
            {
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

        private void Board_NameChanged(BluetoothLEDevice sender, object args)
        {
            showBattery();
            showBoard();
        }

        private void reload_btn_Click(object sender, RoutedEventArgs e)
        {
            showBattery();
            showBoard();
            loadSpeedValues();
        }

        #endregion
    }
}
