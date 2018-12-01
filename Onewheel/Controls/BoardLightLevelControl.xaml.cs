using Logging;
using Onewheel.Classes;
using Onewheel.Pages;
using OnewheelBluetooth.Classes;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Onewheel.Controls
{
    public sealed partial class BoardLightLevelControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public byte WhiteValue
        {
            get { return (byte)GetValue(WhiteValueProperty); }
            set { SetValue(WhiteValueProperty, value); }
        }
        public static readonly DependencyProperty WhiteValueProperty = DependencyProperty.Register(nameof(WhiteValue), typeof(byte), typeof(BoardRidingModeControl), new PropertyMetadata((byte)0));

        public byte RedValue
        {
            get { return (byte)GetValue(RedValueProperty); }
            set { SetValue(RedValueProperty, value); }
        }
        public static readonly DependencyProperty RedValueProperty = DependencyProperty.Register(nameof(RedValue), typeof(byte), typeof(BoardRidingModeControl), new PropertyMetadata((byte)0));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(BoardRidingModeControl), new PropertyMetadata("No description"));

        public Guid Uuid
        {
            get { return (Guid)GetValue(UuidProperty); }
            set { SetValue(UuidProperty, value); }
        }
        public static readonly DependencyProperty UuidProperty = DependencyProperty.Register(nameof(Uuid), typeof(Guid), typeof(BoardRidingModeControl), new PropertyMetadata(null));

        private HomePage homePage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 01/12/2018 Created [Fabian Sauter]
        /// </history>
		public BoardLightLevelControl()
        {
            this.InitializeComponent();

            red_sldr.Minimum = Consts.CUSTOM_LIGHT_LEVEL_MIN;
            red_sldr.Maximum = Consts.CUSTOM_LIGHT_LEVEL_MAX;

            white_sldr.Minimum = Consts.CUSTOM_LIGHT_LEVEL_MIN;
            white_sldr.Maximum = Consts.CUSTOM_LIGHT_LEVEL_MAX;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnLightLevelChanged(byte[] data)
        {
            if (data is null || data.Length != 2)
            {
                return;
            }

            WhiteValue = data[0];
            RedValue = data[1];
        }

        #endregion

        #region --Misc Methods (Private)--
        private void ShowInfo(string text, int duration)
        {
            homePage?.ShowInfo(text, duration);
        }

        private void LoadHomePage()
        {
            homePage = UIUtils.FindParent<HomePage>(this);
        }

        private async Task UpdateLightLevelAsync()
        {
            OnewheelBoard board = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
            if (board is null)
            {
                return;
            }

            byte[] data = { WhiteValue, RedValue };
            await board.WriteBytesAsync(Uuid, data);
            Logger.Info("Updated " + Description + " to " + WhiteValue + " and " + RedValue);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        private async void Red_sldr_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await UpdateLightLevelAsync();
        }

        private async void White_sldr_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await UpdateLightLevelAsync();
        }

        #endregion
    }
}
