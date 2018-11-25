using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using OnewheelBluetooth.Classes.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace OnewheelBluetooth.Classes
{
    public class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();
        private OnewheelConnectionHelperState state = OnewheelConnectionHelperState.DISCONNECTED;

        public readonly OnewheelCharacteristicsCache CACHE = new OnewheelCharacteristicsCache();
        private readonly BluetoothLEHelper BLE_HELPER = BluetoothLEHelper.Context;

        private OnewheelBoard onewheel = null;
        private string boardId = null;

        public delegate void OnewheelConnectionHelperStateChangedHandler(OnewheelConnectionHelper sender, OnewheelConnectionHelperStateChangedEventArgs args);
        public delegate void OnewheelChangedHandler(OnewheelConnectionHelper sender, OnewheelChangedEventArgs args);

        public event OnewheelConnectionHelperStateChangedHandler OnewheelConnectionHelperStateChanged;
        public event OnewheelChangedHandler OnewheelChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2018 Created [Fabian Sauter]
        /// </history>
        private OnewheelConnectionHelper()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public OnewheelBoard GetOnewheel()
        {
            return onewheel;
        }

        public OnewheelConnectionHelperState GetState()
        {
            return state;
        }

        private void SetState(OnewheelConnectionHelperState state)
        {
            if (state != this.state)
            {
                Logger.Debug("[OnewheelConnectionHelper] new state: " + this.state + " -> " + state);
                OnewheelConnectionHelperStateChangedEventArgs args = new OnewheelConnectionHelperStateChangedEventArgs(this.state, state);
                this.state = state;
                OnewheelConnectionHelperStateChanged?.Invoke(this, args);
            }
        }

        private void SetBoard(BluetoothLEDevice board)
        {
            if (onewheel is null || onewheel.GetBoard() != board)
            {
                if (board is null)
                {
                    onewheel = null;
                    SetState(OnewheelConnectionHelperState.DISCONNECTED);
                }
                else
                {
                    onewheel = new OnewheelBoard(board);
                    SetState(OnewheelConnectionHelperState.CONNECTED);
                }
                OnewheelChanged?.Invoke(this, new OnewheelChangedEventArgs(onewheel));
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UseAsBoard(BluetoothLEDevice board)
        {
            StopSearching();
            SetBoard(board);
        }

        public void StopSearching()
        {
            if (state == OnewheelConnectionHelperState.SEARCHING)
            {
                state = OnewheelConnectionHelperState.DISCONNECTED;
                BLE_HELPER.BluetoothLeDevices.CollectionChanged -= BluetoothLeDevices_CollectionChanged;
            }
            Logger.Info("Stopped searching for: " + boardId);
        }

        public void SearchForBoard(string boardId)
        {
            if (string.IsNullOrEmpty(boardId))
            {
                Exception e = new InvalidOperationException("Unable to search for given board id. Bord id either null or empty string.");
                Logger.Error("SearchForBoard() failed!", e);
                throw e;
            }
            this.boardId = boardId;

            if (state == OnewheelConnectionHelperState.SEARCHING)
            {
                Logger.Info("Already searching. Just updated the board id.");
            }

            Task.Run(async () =>
            {
                SetState(OnewheelConnectionHelperState.SEARCHING);

                Logger.Info("Searching for: " + boardId);
                BLE_HELPER.StartEnumeration();
                BLE_HELPER.BluetoothLeDevices.CollectionChanged += BluetoothLeDevices_CollectionChanged;
                await OnDevicesChangedAsync();
            });
        }

        private async Task OnDevicesChangedAsync()
        {
            foreach (var device in BLE_HELPER.BluetoothLeDevices)
            {
                if (!(device.DeviceInfo is null) && string.Equals(device.DeviceInfo.Id, boardId))
                {
                    Logger.Info("Found: " + boardId);
                    if (!device.IsConnected)
                    {
                        try
                        {
                            CancellationTokenSource cancellation = new CancellationTokenSource(1000);
                            BluetoothLEDevice board = await BluetoothLEDevice.FromIdAsync(device.DeviceInfo.Id).AsTask(cancellation.Token);
                            BLE_HELPER.BluetoothLeDevices.CollectionChanged -= BluetoothLeDevices_CollectionChanged;
                            Logger.Error("Connect to: " + device.DeviceInfo.Id);
                            SetBoard(board);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Failed to connect to: " + device.DeviceInfo.Id, e);
                        }
                    }
                }
            }
        }

        private async void BluetoothLeDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (state == OnewheelConnectionHelperState.SEARCHING)
            {
                await OnDevicesChangedAsync();
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
