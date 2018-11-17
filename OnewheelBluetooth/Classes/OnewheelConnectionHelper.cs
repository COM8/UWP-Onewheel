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
        private CancellationTokenSource searchingToken;

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
                searchingToken.Cancel();
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

            Task.Run(() =>
            {
                SetState(OnewheelConnectionHelperState.SEARCHING);

                searchingToken = new CancellationTokenSource();
                BluetoothLEDevice board = null;

                try
                {
                    Task.Run(async () =>
                    {
                        while (!string.IsNullOrEmpty(boardId))
                        {
                            Logger.Info("Searching for: " + boardId);
                            CancellationTokenSource loadDeviceTimeout = new CancellationTokenSource(1000);
                            try
                            {
                                board = await BluetoothLEDevice.FromIdAsync(boardId).AsTask(loadDeviceTimeout.Token);
                            }
                            catch (TaskCanceledException e)
                            {
                                Logger.Error("Failed to load bluetooth device - timeout.", e);
                            }
                            if (board != null)
                            {
                                Logger.Info("Found: " + boardId);
                                SetBoard(board);
                                return;
                            }
                        }
                    }, searchingToken.Token);
                }
                catch (TaskCanceledException)
                {
                    Logger.Info("Searching for boardId was canceled.");
                    SetState(OnewheelConnectionHelperState.DISCONNECTED);
                }
            });
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
