using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes.Events;
using System.Threading.Tasks;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Diagnostics;
using DataManager.Classes;

namespace Onewheel.Classes
{
    class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();

        public readonly OnewheelInfo ONEWHEEL_INFO;
        private BluetoothLEHelper bluetoothLEHelper;
        public ObservableBluetoothLEDevice board { get; private set; }

        public delegate void BoardChangedHandler(OnewheelConnectionHelper sender, BoardChangedEventArgs args);

        public event BoardChangedHandler BoardChanged;

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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setBoard(ObservableBluetoothLEDevice board)
        {
            this.board = board;
            setLastBoard(board);
            BoardChanged?.Invoke(this, new BoardChangedEventArgs(board));
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
            Task.Run(async () =>
            {
                if (BluetoothLEHelper.IsBluetoothLESupported && Settings.getSettingString(SettingsConsts.BOARD_ADDRESS) != null)
                {
                    bluetoothLEHelper.StartEnumeration();

                    bluetoothLEHelper.BluetoothLeDevices.CollectionChanged += BluetoothLeDevices_CollectionChanged;
                    await searchBoardAsync();
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
                if (Equals(bluetoothLEHelper.BluetoothLeDevices[i].BluetoothAddressAsString, boardAddress))
                {
                    await bluetoothLEHelper.BluetoothLeDevices[i].ConnectAsync();
                    setBoard(bluetoothLEHelper.BluetoothLeDevices[i]);
                    bluetoothLEHelper.StopEnumeration();
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void BluetoothLeDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            searchBoardAsync();
        }

        #endregion
    }
}
