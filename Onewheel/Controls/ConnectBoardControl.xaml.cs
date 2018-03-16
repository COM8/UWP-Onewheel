﻿using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class ConnectBoardControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BluetoothLEHelper bluetoothLEHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectBoardControl()
        {
            this.bluetoothLEHelper = BluetoothLEHelper.Context;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void showBoards()
        {
            if (BluetoothLEHelper.IsBluetoothLESupported)
            {
                error_stckp.Visibility = Visibility.Collapsed;

                bluetoothLEHelper.StartEnumeration();
            }
            else
            {
                error_itbx.Text = "Your device does not support Bluetooth LE!";
                error_stckp.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            showBoards();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            bluetoothLEHelper.StopEnumeration();
        }

        private void retry_btn_Click(object sender, RoutedEventArgs e)
        {
            showBoards();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            connect_btn.IsEnabled = boards_lstv.SelectedIndex >= 0;
        }

        private async void connect_btn_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                if (!bluetoothLEHelper.BluetoothLeDevices[boards_lstv.SelectedIndex].IsConnected)
                {
                    await bluetoothLEHelper.BluetoothLeDevices[boards_lstv.SelectedIndex].ConnectAsync();
                }
                OnewheelConnectionHelper.INSTANCE.setBoard(bluetoothLEHelper.BluetoothLeDevices[boards_lstv.SelectedIndex]);
                error_stckp.Visibility = Visibility.Collapsed;
            }
            catch (System.Exception e)
            {
                error_itbx.Text = e.Message + "\n" + e.StackTrace;
                error_stckp.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}
