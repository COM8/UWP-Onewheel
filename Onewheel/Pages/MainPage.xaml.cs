using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Pages
{
    public sealed partial class MainPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(MainPage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public MainPage()
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
        private void navigateTo(HamburgerMenuGlyphItem item)
        {
            HeaderText = item.Label;
            switch (item.Tag)
            {
                case "home":
                    contentFrame.Navigate(typeof(HomePage));
                    break;

                case "info":
                    contentFrame.Navigate(typeof(InfoPage));
                    break;

                case "connect":
                    contentFrame.Navigate(typeof(ConnectPage));
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void main_hbm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is HamburgerMenuGlyphItem)
            {
                navigateTo(e.ClickedItem as HamburgerMenuGlyphItem);
            }
        }

        private void main_hbm_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            contentFrame.Navigate(typeof(SettingsPage));
        }

        #endregion
    }
}
