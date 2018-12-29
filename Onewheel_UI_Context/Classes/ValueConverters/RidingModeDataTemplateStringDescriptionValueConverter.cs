using Onewheel_UI_Context.Classes.DataTemplates;
using OnewheelBluetooth.Classes;
using System;
using Windows.UI.Xaml.Data;

namespace Onewheel_UI_Context.Classes.ValueConverters
{
    public sealed class RidingModeDataTemplateStringDescriptionValueConverter : IValueConverter
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
            if (value is RidingModeDataTemplate mode)
            {
                switch (mode.Mode)
                {
                    case Consts.RIDING_MODE_OW_CLASSIC_CLASSIC:
                    case Consts.RIDING_MODE_OW_CLASSIC_XTREME:
                    case Consts.RIDING_MODE_OW_CLASSIC_ELEVATE:
                        return "";

                    // SEQUOIA:
                    case Consts.RIDING_MODE_OW_PLUS_SEQUOIA:
                        return "Max Velocity 20km/h.";

                    // CRUZ:
                    case Consts.RIDING_MODE_OW_PLUS_CRUZ:
                        return "Max Velocity 24km/h.";

                    // MISSION:
                    case Consts.RIDING_MODE_OW_PLUS_MISSION:
                        return "Max Velocity 31km/h.";

                    // ELEVATE:
                    case Consts.RIDING_MODE_OW_PLUS_ELEVATE:
                        return "Max Velocity 31km/h.";

                    // DELIRIUM:
                    case Consts.RIDING_MODE_OW_PLUS_DELIRIUM:
                        return "Max Velocity 32km/h.";

                    // CUSTOM SHAPING:
                    case Consts.RIDING_MODE_OW_PLUS_CUSTOM_SHAPING:
                        return "Custom shaping.";
                }
            }
            return "";
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
