using DataManager.Classes;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Onewheel.Pages;
using Onewheel_UI_Context.Classes;
using OnewheelBluetooth.Classes;
using System;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Onewheel
{
    sealed partial class App : Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                    UiUtils.SetupWindow(Current);
                }
                Settings.setSetting(SettingsConsts.APP_REQUESTED_THEME, value.ToString());
            }
        }

        private bool isRunning;
        private ThemeListener themeListener;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public App()
        {
            this.isRunning = false;

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Sets the log level for the logger class.
        /// </summary>
        private void InitLogger()
        {
            object o = Settings.getSetting(SettingsConsts.LOG_LEVEL);
            if (o is int)
            {
                Logger.logLevel = (LogLevel)o;
            }
            else
            {
                Settings.setSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                Logger.logLevel = LogLevel.INFO;
            }
        }

        private void OnActivatedOrLaunched(IActivatedEventArgs args)
        {
            // Prevent window from extending into title bar:
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;

            // Sets the log level:
            InitLogger();

            // Set requested theme:
            RootTheme = UiUtils.LoadRequestedTheme();

            // Setup listening for theme changes:
            SetupThemeListener();

            // Override resources to increase the UI performance on mobile devices:
            if (DeviceFamilyHelper.GetDeviceFamilyType() == DeviceFamilyType.Mobile)
            {
                UiUtils.OverrideResources();
            }

            // Setup window:
            UiUtils.SetupWindow(Current);

            isRunning = true;

            string lastBoardId = Settings.getSettingString(SettingsConsts.BOARD_ID);
            if (!string.IsNullOrEmpty(lastBoardId))
            {
                OnewheelConnectionHelper.INSTANCE.SearchForBoard(lastBoardId);
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page:
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            rootFrame.Navigate(typeof(MainPage));

            Window.Current.Activate();
        }

        private void SetupThemeListener()
        {
            if (themeListener is null)
            {
                themeListener = new ThemeListener();
                themeListener.ThemeChanged += ThemeListener_ThemeChanged;
            }
        }

        private void ThemeListener_ThemeChanged(ThemeListener sender)
        {
            UiUtils.SetupWindow(Current);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            OnActivatedOrLaunched(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnActivatedOrLaunched(args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save current state and cancel all background activities
            deferral.Complete();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Logger.Error("Failed to load page: " + e.SourcePageType.FullName);
            throw new Exception("Failed to load page " + e.SourcePageType.FullName);
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhanded exception: ", e.Exception);
        }

        private void App_Resuming(object sender, object e)
        {
            // ToDo: Reconnect to last board
            isRunning = true;
        }

        #endregion
    }
}
