using Onewheel.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using BluetoothOnewheelAccess.Classes;

namespace Onewheel.Controls
{
    public sealed partial class BoardRidingModeControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BoardRidingModeControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/04/2018 Created [Fabian Sauter]
        /// </history>
		public BoardRidingModeControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task showChangeRidingModeDialogAsync()
        {
            ChangeRidingModeDialog dialog = new ChangeRidingModeDialog();
            await dialog.ShowAsync();

            if (!dialog.canceled)
            {
                uint curSpeed = OnewheelConnectionHelper.INSTANCE.ONEWHEEL_INFO.getCharacteristicAsUInt(OnewheelInfo.CHARACTERISTIC_SPEED_RPM);
                if(curSpeed > 0)
                {
                    // To fast to change speed
                }
                else
                {
                    await setRideModeAsync(dialog.selectedRideMode);
                }
            }
        }

        private async Task setRideModeAsync(uint rideMode)
        {
            byte[] rideModeArray = BitConverter.GetBytes((short)rideMode);

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await showChangeRidingModeDialogAsync();
        }

        #endregion
    }
}
