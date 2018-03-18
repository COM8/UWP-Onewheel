using System;

namespace DataManager.Classes
{
    public class Utils
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
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public Utils()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static double rpmToKilometersPerHour(uint rpm)
        {
            return Math.Round(rpmToKilometers(rpm) * 60, 2);
        }

        public static double rpmToKilometers(uint rpm)
        {
            return (35.0 * rpm) / 39370.1;
        }

        public static double milesToKilometers(double miles)
        {
            return miles * 1.60934;
        }

        public static double convertToAmpere(uint value)
        {
            double multiplier = 0;

            if (true)
            {
                // Onewheel+:
                multiplier = 1.8;
            }
            else
            {
                // Onewheel:
                multiplier = 0.9;
            }

            double amp = value / 1000.0 * multiplier;

            return Math.Round(amp, 2);
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
