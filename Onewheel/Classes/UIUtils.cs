using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Onewheel.Classes
{
    static class UIUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly string[] BATTERY_LEVEL_ICONS = new string[] {
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

        public static readonly string[] BATTERY_CHARCHING_LEVEL_ICONS = new string[] {
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
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
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
