using Onewheel.Classes;
using Onewheel.Dialogs;
using Onewheel.Pages;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private HomePage homePage;

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
        private async Task ShowChangeRidingModeDialogAsync()
        {
            ChangeRidingModeDialog dialog = new ChangeRidingModeDialog();
            await dialog.ShowAsync();
        }

        private void ShowInfo(string text, int duration)
        {
            homePage?.ShowInfo(text, duration);
        }

        private void loadHomePage()
        {
            homePage = UIUtils.FindParent<HomePage>(this);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ShowChangeRidingModeDialogAsync();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadHomePage();
        }

        #endregion
    }
}
