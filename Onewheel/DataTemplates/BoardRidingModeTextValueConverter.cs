using System;
using Windows.UI.Xaml.Data;

namespace Onewheel.DataTemplates
{
    class BoardRidingModeTextValueConverter : IValueConverter
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
        public BoardRidingModeTextValueConverter()
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
                        return "CLASSIC";

                    case 2:
                        return "XTREME";

                    case 3:
                        return "ELEVATE";

                    case 4:
                        return "SEQUOIA";

                    case 5:
                        return "CRUZ";

                    case 6:
                        return "MISSION";

                    case 7:
                        return "ELEVATE";

                    case 8:
                        return "DELIRIUM";

                    case 9:
                        return "CUSTOM";
                }
            }

            return value == null ? "-" :  value.ToString();
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
