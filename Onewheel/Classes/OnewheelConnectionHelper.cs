using Microsoft.Toolkit.Uwp.Connectivity;
using Onewheel.Classes.Events;
using System.Threading.Tasks;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace Onewheel.Classes
{
    class OnewheelConnectionHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static OnewheelConnectionHelper INSTANCE = new OnewheelConnectionHelper();

        public ObservableBluetoothLEDevice board { get; private set; }

        public delegate void BoardChangedHandler(OnewheelConnectionHelper helper, BoardChangedEventArgs args);

        public event BoardChangedHandler BoardChanged;

        private static readonly Guid BATTERY_SERVICE_UUID = Guid.Parse("e659f300-ea98-11e3-ac10-0800200c9a66");
        private static readonly Guid BATTERY_CHARACTERISTIC_UUID = Guid.Parse("e659f303-ea98-11e3-ac10-0800200c9a66");

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public async Task<int> getBatteryLevelAsync()
        {
            if (board != null && board.BluetoothLEDevice != null)
            {
                GattDeviceServicesResult sResult = await board.BluetoothLEDevice.GetGattServicesForUuidAsync(BATTERY_SERVICE_UUID);
                if (sResult.Status == GattCommunicationStatus.Success)
                {
                    if (sResult.Services.Count >= 1)
                    {
                        GattCharacteristicsResult cResult = await sResult.Services[0].GetCharacteristicsForUuidAsync(BATTERY_CHARACTERISTIC_UUID);
                        if (cResult.Status == GattCommunicationStatus.Success)
                        {
                            if (cResult.Characteristics.Count >= 1)
                            {
                                return await readIntFromCharacteristicAsync(cResult.Characteristics[0]);
                            }
                        }
                    }
                }
            }
            return -1;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setBoard(ObservableBluetoothLEDevice newBoard)
        {
            board = newBoard;
            BoardChanged?.Invoke(this, new BoardChangedEventArgs(newBoard));
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<int> readIntFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            try
            {
                GattReadResult vRes = await characteristic.ReadValueAsync();
                if (vRes.Status == GattCommunicationStatus.Success)
                {
                    var reader = DataReader.FromBuffer(vRes.Value);
                    var input = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(input);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(input);
                    }
                    return BitConverter.ToInt16(input, 0);
                }
            }
            catch (Exception e)
            {
            }
            return -1;
        }

        public void connectToLastBoard()
        {

        }

        public async Task<string> readStringFromCharacteristicAsync(GattCharacteristic characteristic)
        {
            try
            {
                GattReadResult vRes = await characteristic.ReadValueAsync();
                if (vRes.Status == GattCommunicationStatus.Success)
                {
                    var reader = DataReader.FromBuffer(vRes.Value);
                    var input = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(input);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(input);
                    }
                    return BitConverter.ToString(input);
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public async Task printAllAsync()
        {
            if(board != null && board.BluetoothLEDevice != null)
            {
                GattDeviceServicesResult sResult = await board.BluetoothLEDevice.GetGattServicesAsync();
                if(sResult.Status == GattCommunicationStatus.Success)
                {
                    foreach (GattDeviceService s in sResult.Services)
                    {
                        Debug.WriteLine("UUID: " + s.Uuid + ", Handle: " + s.AttributeHandle);

                        GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                        if(cResult.Status == GattCommunicationStatus.Success)
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


        #endregion
    }
}
