using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class SpeedGaugeControl : UserControl, INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public SpeedGaugeControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void setValue(int value)
        {
            if (value > gauge_gc.To)
            {
                gauge_gc.To = value;
            }
            gauge_gc.Value = value;
            gauge_gc.UpdateLayout();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void ChangeValueOnClick(object sender, RoutedEventArgs e)
        {
            gauge_gc.Value = new Random().Next(0, 60);
        }

        #endregion

        #region --Misc Methods (Protected)--
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
