using System;
using Windows.UI.Xaml.Data;

namespace Onewheel.DataTemplates
{
    class BoardInfoValueConverter : IValueConverter
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
        /// 25/03/2018 Created [Fabian Sauter]
        /// </history>
        public BoardInfoValueConverter()
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
                if(i >= 10000)
                {
                    return Math.Round((i / 10000.0), 2).ToString() + "K";
                }
            }
            else if (value is double d)
            {
                if(d >= 100)
                {
                    return Math.Round(d, 0).ToString();
                }
                if (d >= 10000)
                {
                    return Math.Round((d / 10000.0), 2).ToString() + "K";
                }
            }

            return value.ToString();
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
