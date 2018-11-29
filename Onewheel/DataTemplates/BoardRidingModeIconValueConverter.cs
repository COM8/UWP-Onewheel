using System;
using Windows.UI.Xaml.Data;

namespace Onewheel.DataTemplates
{
    class BoardRidingModeIconValueConverter : IValueConverter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/04/2018 Created [Fabian Sauter]
        /// </history>
        public BoardRidingModeIconValueConverter()
        {
        }

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
                switch (i)
                {
                    case 1:
                        return "\uEC49";

                    case 2:
                        return "\uEC4A";

                    case 3:
                        return "\uE803";

                    // SEQUOIA:
                    case 4:
                        return "\uE9D9";

                    // CRUZ:
                    case 5:
                        return "\uE1C3";

                    // MISSION:
                    case 6:
                        return "\uE7C1";

                    // ELEVATE:
                    case 7:
                        return "\uE81E";

                    // DELIRIUM:
                    case 8:
                        return "\uE81F";

                    // CUSTOM SHAPING:
                    case 9:
                        return "\uE90F";
                }
            }

            return "\uE11B";
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
