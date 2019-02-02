using DataManager.Classes;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Onewheel_UI_Context.Classes
{
    public static class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly string[] BATTERY_LEVEL_GLYPHS = new string[] {
            "\uEBA0", // 0%
            "\uEBA1", // 10%
            "\uEBA2", // 20%
            "\uEBA3", // 30%
            "\uEBA4", // 40%
            "\uEBA5", // 50%
            "\uEBA6", // 60%
            "\uEBA7", // 70%
            "\uEBA8", // 80%
            "\uEBA9", // 90%
            "\uEBAA", // 100%
            "\uEC02", // Unknown
        };

        public static readonly string[] BATTERY_CHARCHING_LEVEL_GLYPHS = new string[] {
            "\xEBAB", // 0%
            "\xEBAC", // 10%
            "\uEBAD", // 20%
            "\uEBAE", // 30%
            "\uEBAF", // 40%
            "\xEBB0", // 50%
            "\uEBB1", // 60%
            "\uEBB2", // 70%
            "\uEBB3", // 80%
            "\uEBB4", // 90%
            "\uEBB5", // 100%
            "\uEC02", // Unknown
        };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static List<KeyboardAccelerator> GetGoBackKeyboardAccelerators()
        {
            return new List<KeyboardAccelerator>
            {
                new KeyboardAccelerator
                {
                    Key = VirtualKey.Back
                },
                new KeyboardAccelerator
                {
                    Key = VirtualKey.Left
                },
                new KeyboardAccelerator
                {
                    Key = VirtualKey.GoBack
                }
            };
        }

        public static bool IsDarkThemeActive()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public static bool IsApplicationViewApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView");
        }

        public static bool IsStatusBarApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        /// <summary>
        /// The KeyboardAccelerator class got introduced with v10.0.16299.0.
        /// Source: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.input.keyboardaccelerator
        /// </summary>
        /// <returns></returns>
        public static bool IsKeyboardAcceleratorApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.KeyboardAccelerator");
        }

        public static bool OnGoBackRequested(Frame frame)
        {
            if (frame is null)
            {
                Logger.Error("Failed to execute back request - frame is null!");
                return false;
            }
            else
            {
                if (frame.CanGoBack)
                {
                    frame.GoBack();
                    return true;
                }
                return false;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Overrides the default resources like "ButtonRevealStyle" with more performant versions
        /// to increase the UI performance on low end devices like phones.
        /// </summary>
        public static void OverrideResources()
        {
            // Styles:
            Application.Current.Resources["ButtonRevealStyle"] = Application.Current.Resources["DefaultButtonStyle"];

            // Brushes:
            if (IsDarkThemeActive())
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// Source: https://social.msdn.microsoft.com/Forums/sqlserver/en-US/0cc87160-5b0c-4fc1-b685-ff50117984f7/uwp-access-control-on-parent-page-through-frame-object?forum=wpdevelop
        /// </summary>
        public static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        /// <summary>
        /// Calls the UI thread dispatcher and executes the given callback on it.
        /// </summary>
        /// <param name="callback">The callback that should be executed in the UI thread.</param>
        public static async Task CallDispatcherAsync(DispatchedHandler callback)
        {
            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                callback();
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
            }
        }

        public static ElementTheme LoadRequestedTheme()
        {
            string themeString = Settings.getSettingString(SettingsConsts.APP_REQUESTED_THEME);
            ElementTheme theme = ElementTheme.Dark;
            if (themeString != null)
            {
                Enum.TryParse(themeString, out theme);
            }
            return theme;
        }

        public static void SetupWindow(Application application)
        {
            // PC, Mobile:
            if (IsApplicationViewApiAvailable())
            {
                ApplicationView appView = ApplicationView.GetForCurrentView();

                // Dye title:
                Brush windowBrush = new SolidColorBrush((Color)application.Resources["SystemAccentColorDark3"]);
                if (windowBrush is Microsoft.UI.Xaml.Media.AcrylicBrush acrylicWindowBrush)
                {
                    appView.TitleBar.BackgroundColor = acrylicWindowBrush.TintColor;
                }
                else
                {
                    appView.TitleBar.BackgroundColor = ((SolidColorBrush)windowBrush).Color;
                }

                //Dye title bar buttons:
                appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["SystemListLowColor"];
                appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];

                // Extend window:
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            // Mobile:
            if (IsStatusBarApiAvailable())
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundColor = (Color)application.Resources["SystemAccentColorDark3"];
                    statusBar.BackgroundOpacity = 1.0d;
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
