using Microsoft.Toolkit.Uwp.Connectivity;
using System.Threading.Tasks;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Diagnostics;
using BluetoothOnewheelAccess.Classes.Events;
using DataManager.Classes;
using Windows.ApplicationModel.Background;
using Microsoft.Toolkit.Uwp.Helpers;

namespace BluetoothOnewheelAccess.Classes
{
    public class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();

        public readonly OnewheelInfo ONEWHEEL_INFO;
        private BluetoothLEHelper bluetoothLEHelper;
        public ObservableBluetoothLEDevice board { get; private set; }
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
        private void setBoard(ObservableBluetoothLEDevice board)
        {
            if (this.board == board)
            {
                return;
            }

            if (this.board != null)
            {
                board.PropertyChanged -= Board_PropertyChanged;
            }

            this.board = board;

            if (this.board != null)
            {
                board.PropertyChanged += Board_PropertyChanged;
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

        public void setLastBoard(ObservableBluetoothLEDevice lastBoard)
        {
            if (board != null && board.BluetoothAddressAsString != null)
            {
                Settings.setSetting(SettingsConsts.BOARD_ADDRESS, lastBoard.BluetoothAddressAsString);
                Settings.setSetting(SettingsConsts.BOARD_NAME, lastBoard.Name);
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

        public async Task useBoardAsync(ObservableBluetoothLEDevice board)
        {
            setOnewheelConnectionState(OnewheelConnectionState.CONNECTING);
            await board.ConnectAsync();
            setBoard(board);
            if (board.IsConnected)
            {
                setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
            }
            stopSearchingForLastBoard();
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
                if (BluetoothLEHelper.IsBluetoothLESupported && Settings.getSettingString(SettingsConsts.BOARD_ADDRESS) != null)
                {
                    setOnewheelConnectionState(OnewheelConnectionState.SEARCHING);

                    bluetoothLEHelper.StartEnumeration();

                    bluetoothLEHelper.BluetoothLeDevices.CollectionChanged += BluetoothLeDevices_CollectionChanged;
                    await searchBoardAsync();
                }
                else
                {
                    setOnewheelConnectionState(OnewheelConnectionState.NO_LAST_BOARD);
                }
            });
        }

        public async Task printAllAsync()
        {
            if (board != null && board.BluetoothLEDevice != null)
            {
                GattDeviceServicesResult sResult = await board.BluetoothLEDevice.GetGattServicesAsync();
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
        private async Task searchBoardAsync()
        {
            string boardAddress = Settings.getSettingString(SettingsConsts.BOARD_ADDRESS);
            for (int i = 0; i < bluetoothLEHelper.BluetoothLeDevices.Count; i++)
            {
                // Searching got canceled:
                if (!connectingToLastBoard)
                {
                    return;
                }

                if (Equals(bluetoothLEHelper.BluetoothLeDevices[i].BluetoothAddressAsString, boardAddress))
                {
                    setOnewheelConnectionState(OnewheelConnectionState.CONNECTING);
                    await bluetoothLEHelper.BluetoothLeDevices[i].ConnectAsync();
                    setBoard(bluetoothLEHelper.BluetoothLeDevices[i]);
                    stopSearchingForLastBoard();
                }
            }
        }

        private void stopSearchingForLastBoard()
        {
            bluetoothLEHelper.StopEnumeration();
            bluetoothLEHelper.BluetoothLeDevices.CollectionChanged -= BluetoothLeDevices_CollectionChanged;
            connectingToLastBoard = false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void BluetoothLeDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await searchBoardAsync();
        }

        private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsConnected":
                    if (board.IsConnected)
                    {
                        setOnewheelConnectionState(OnewheelConnectionState.CONNECTED);
                    }
                    else
                    {
                        setOnewheelConnectionState(OnewheelConnectionState.DISCONNECTED);
                    }
                    break;
            }
        }

        #endregion
    }
}
