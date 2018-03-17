﻿using BluetoothOnewheelAccess.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace Onewheel.Pages
{
    public sealed partial class ConnectPage : Page
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
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectPage()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.OnewheelConnectionStateChanged += INSTANCE_OnewheelConnectionStateChanged;
            status_tbx.Text = OnewheelConnectionHelper.INSTANCE.state.ToString();
        }

        private async void INSTANCE_OnewheelConnectionStateChanged(OnewheelConnectionHelper sender, BluetoothOnewheelAccess.Classes.Events.OnewheelConnectionStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => status_tbx.Text = args.NEW_STATE.ToString());
        }

        #endregion
    }
}
