using OnewheelBluetooth.Classes;
using System;
using Windows.UI.Xaml.Data;

namespace Onewheel_UI_Context.Classes.ValueConverters
{
    public sealed class AggressivenessUintDoubleValueConverter : IValueConverter
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
            if (value is uint i)
            {
                return (double)i;
            }
            return (double)Utils.AggressivenessToUInt((byte)Consts.CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double d)
            {
                if (d < 0)
                {
                    return (uint)0;
                }
                return (uint)d;
            }
            return Utils.AggressivenessToUInt((byte)Consts.CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS);
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
