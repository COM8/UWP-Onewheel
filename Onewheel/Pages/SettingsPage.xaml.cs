﻿using Onewheel_UI_Context.Classes.DataContexts.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Pages
{
    public sealed partial class SettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageContext VIEW_MODEL = new SettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsPage()
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
        private async void DeleteLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.DeleteLogsAsync();
            await logsFolder_fsc.RecalculateFolderSizeAsync();
        }

        private async void ExportLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ExportLogsAsync();
        }

        #endregion
    }
}
