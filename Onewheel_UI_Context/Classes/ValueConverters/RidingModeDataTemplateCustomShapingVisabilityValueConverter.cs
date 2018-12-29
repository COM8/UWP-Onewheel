using Onewheel_UI_Context.Classes.DataTemplates;
using OnewheelBluetooth.Classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Onewheel_UI_Context.Classes.ValueConverters
{
    public sealed class RidingModeDataTemplateCustomShapingVisabilityValueConverter : IValueConverter
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
            if (value is RidingModeDataTemplate mode && mode.Mode == Consts.RIDING_MODE_OW_PLUS_CUSTOM_SHAPING)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
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
