using DataManager.Classes;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class PreventScreenLockControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly DisplayRequest DISPLAY_REQUEST;
        private uint requestCount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 11/07/2018 Created [Fabian Sauter]
        /// </history>
        public PreventScreenLockControl()
        {
            this.DISPLAY_REQUEST = new DisplayRequest();
            this.requestCount = 0;
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            screenLock_tggls.IsOn = !Settings.getSettingBoolean(SettingsConsts.DISABLE_PREVENT_SCREEN_LOCK);
        }

        private void screenLock_tggls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DISABLE_PREVENT_SCREEN_LOCK, !screenLock_tggls.IsOn);
            if (screenLock_tggls.IsOn)
            {
                DISPLAY_REQUEST.RequestActive();
                requestCount++;
                mode_tbx.Text = "Yes";
            }
            else
            {
                DISPLAY_REQUEST.RequestRelease();
                requestCount--;
                mode_tbx.Text = "No";
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            while (requestCount-- > 0)
            {
                DISPLAY_REQUEST.RequestRelease();
            }
        }

        #endregion
    }
}
