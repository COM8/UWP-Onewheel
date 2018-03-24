using Microsoft.Toolkit.Uwp.Connectivity;
using System.Threading.Tasks;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Diagnostics;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Windows.Devices.Bluetooth;

namespace BluetoothOnewheelAccess.Classes
{
    public class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();

        public readonly OnewheelInfo ONEWHEEL_INFO;
        private BluetoothLEHelper bluetoothLEHelper;
        public BluetoothLEDevice board { get; private set; }
        public bool autoReconnect;
        public bool connectingToLastBoard;
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
            this.autoReconnect = true;
            this.connectingToLastBoard = false;
            setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
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

                switch (this.board.ConnectionStatus)
                {
                    case BluetoothConnectionStatus.Disconnected:
                        setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
                        break;
                    case BluetoothConnectionStatus.Connected:
                        setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
                        break;
                }
            }

            setLastBoard(board);
            BoardChanged?.Invoke(this, new BoardChangedEventArgs(board));
        }

        public void setOnewheelConnectionState(OnewheelConnectionState state)
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
            connectToLastBoard();
            ONEWHEEL_INFO.init();
        }

        public async Task useBoardAsync(BluetoothLEDevice board)
        {
            setOnewheelConnectionState(OnewheelConnectionState.CONNECTING);
            setBoard(board);
            if (board.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
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

        public void connectToLastBoard()
        {
            if (!autoReconnect || connectingToLastBoard)
            {
                return;
            }

            Task.Run(async () =>
            {
                connectingToLastBoard = true;
                setOnewheelConnectionState(OnewheelConnectionState.SEARCHING);

                string lastBoardId = Settings.getSettingString(SettingsConsts.BOARD_ID);
                if (string.IsNullOrEmpty(lastBoardId))
                {
                    setOnewheelConnectionState(OnewheelConnectionState.NO_LAST_BOARD);
                }
                else
                {
                    BluetoothLEDevice board = await BluetoothLEDevice.FromIdAsync(lastBoardId);
                    setBoard(board);
                }
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
                                Debug.WriteLine("\tUUID: " + c.Uuid + ", Value: " + value + ", Handle: " + c.AttributeHandle);

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
                    setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
                    break;
                case BluetoothConnectionStatus.Connected:
                    setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
                    break;
            }
        }

        #endregion
    }
}
