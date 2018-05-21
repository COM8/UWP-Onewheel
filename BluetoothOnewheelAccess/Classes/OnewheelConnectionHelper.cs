using Microsoft.Toolkit.Uwp.Connectivity;
using System.Threading.Tasks;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Diagnostics;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Windows.Devices.Bluetooth;
using System.Threading;

namespace BluetoothOnewheelAccess.Classes
{
    public class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();

        public readonly OnewheelInfo ONEWHEEL_INFO;
        private BluetoothLEHelper bluetoothLEHelper;
        private CancellationTokenSource searchingToken;
        public BluetoothLEDevice board { get; private set; }
        public string autoReconnectBoardId;
        public OnewheelConnectionState state;

        public delegate void BoardChangedHandler(OnewheelConnectionHelper sender, BoardChangedEventArgs args);
        public delegate void OnewheelConnectionStateChangedHandler(OnewheelConnectionHelper sender, OnewheelConnectionStateChangedEventArgs args);

        public event BoardChangedHandler BoardChanged;
        public event OnewheelConnectionStateChangedHandler OnewheelConnectionStateChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelConnectionHelper()
        {
            this.ONEWHEEL_INFO = new OnewheelInfo();
            this.bluetoothLEHelper = null;
            this.searchingToken = null;
            this.autoReconnectBoardId = null;
            this.state = OnewheelConnectionState.DISCONNECTED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setBoard(BluetoothLEDevice board)
        {
            if (this.board == board)
            {
                return;
            }

            if (this.board != null)
            {
                this.board.ConnectionStatusChanged -= Board_ConnectionStatusChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                this.board.ConnectionStatusChanged += Board_ConnectionStatusChanged;
                this.autoReconnectBoardId = board.DeviceId;
                setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
            }
            else
            {
                this.autoReconnectBoardId = null;
                setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
            }

            setLastBoard(board);
            BoardChanged?.Invoke(this, new BoardChangedEventArgs(board));
        }

        private void setOnewheelConnectionState(OnewheelConnectionState state)
        {
            if (this.state == state)
            {
                return;
            }

            OnewheelConnectionStateChangedEventArgs args = new OnewheelConnectionStateChangedEventArgs(this.state, state);
            this.state = state;
            OnewheelConnectionStateChanged?.Invoke(this, args);

            if (state == OnewheelConnectionState.DISCONNECTED)
            {
                connectToLastBoard();
            }
        }

        public void setLastBoard(BluetoothLEDevice lastBoard)
        {
            if (board != null && board.DeviceId != null)
            {
                Settings.setSetting(SettingsConsts.BOARD_ADDRESS, lastBoard.BluetoothAddress.ToString());
                Settings.setSetting(SettingsConsts.BOARD_NAME, lastBoard.Name);
                Settings.setSetting(SettingsConsts.BOARD_ID, lastBoard.DeviceId);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void init()
        {
            bluetoothLEHelper = BluetoothLEHelper.Context;
            autoReconnectBoardId = Settings.getSettingString(SettingsConsts.BOARD_ID);
            connectToLastBoard();
            ONEWHEEL_INFO.init();
        }

        public void useBoard(BluetoothLEDevice board)
        {
            stopSearchingForLastBoard();

            setOnewheelConnectionState(OnewheelConnectionState.CONNECTING);
            setBoard(board);
            if (board != null)
            {
                setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
            }
            else
            {
                setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
            }
        }

        public async Task<byte[]> readBytesFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            GattReadResult vRes = await characteristic.ReadValueAsync();
            if (vRes.Status == GattCommunicationStatus.Success)
            {
                DataReader reader = DataReader.FromBuffer(vRes.Value);
                byte[] data = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(data);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(data);
                }
                return data;
            }
            return null;
        }

        public async Task<string> readStringFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            byte[] data = await readBytesFromCharacteristicAsync(characteristic);
            if (data != null)
            {
                return BitConverter.ToString(data);
            }
            return null;
        }

        public async Task<int> readIntFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            byte[] data = await readBytesFromCharacteristicAsync(characteristic);
            if (data != null)
            {
                return BitConverter.ToInt16(data, 0);
            }
            return -1;
        }

        private void stopSearchingForLastBoard()
        {
            if (searchingToken != null)
            {
                autoReconnectBoardId = null;
                searchingToken.Cancel();
            }
        }

        public void connectToLastBoard()
        {
            Task.Run(() =>
            {
                if (autoReconnectBoardId == null)
                {
                    setOnewheelConnectionState(OnewheelConnectionState.NO_LAST_BOARD);
                    return;
                }

                setOnewheelConnectionState(OnewheelConnectionState.SEARCHING);

                searchingToken = new CancellationTokenSource();
                BluetoothLEDevice board = null;

                Task.Run(async () =>
                {
                    while(autoReconnectBoardId != null)
                    {
                        board = await BluetoothLEDevice.FromIdAsync(autoReconnectBoardId);
                        if (board != null)
                        {
                            setBoard(board);
                            return;
                        }
                    }
                }, searchingToken.Token);
            });
        }

        public async Task printAllAsync()
        {
            if (board != null && board.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                GattDeviceServicesResult sResult = await board.GetGattServicesAsync();
                if (sResult.Status == GattCommunicationStatus.Success)
                {
                    foreach (GattDeviceService s in sResult.Services)
                    {
                        Debug.WriteLine("UUID: " + s.Uuid + ", Handle: " + s.AttributeHandle);

                        GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                        if (cResult.Status == GattCommunicationStatus.Success)
                        {
                            foreach (GattCharacteristic c in cResult.Characteristics)
                            {
                                string value = await readStringFromCharacteristicAsync(c);
                                Debug.WriteLine("\tUUID: " + c.Uuid + ", Value: " + value + ", Handle: " + c.AttributeHandle + ", Description: " + c.UserDescription);

                                Debug.WriteLine("\t\tProperties: " + c.CharacteristicProperties.ToString());
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Board_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            switch (sender.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Disconnected:
                    connectToLastBoard();
                    break;
            }

            if (board != null)
            {
                setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
            }
            else
            {
                setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
            }
        }

        #endregion
    }
}
