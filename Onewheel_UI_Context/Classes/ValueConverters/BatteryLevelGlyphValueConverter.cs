using OnewheelBluetooth.Classes;
using System;
using Windows.UI.Xaml.Data;

namespace Onewheel_UI_Context.Classes.ValueConverters
{
    public sealed class BatteryLevelGlyphValueConverter : IValueConverter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int i && i >= 0 && i <= 100)
            {
                OnewheelStatus status = OnewheelConnectionHelper.INSTANCE.CACHE.GetStatus();
                if (status.CHARGING)
                {
                    return UiUtils.BATTERY_CHARCHING_LEVEL_GLYPHS[i / 10];
                }
                else
                {
                    return UiUtils.BATTERY_LEVEL_GLYPHS[i / 10];
                }
            }
            return UiUtils.BATTERY_LEVEL_GLYPHS[11];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
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
