﻿using Logging;
using Shared.Classes;
using System.Threading.Tasks;
using Windows.Storage;

namespace Onewheel_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class SettingsPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _LogFolderPath;
        public string LogFolderPath
        {
            get => _LogFolderPath;
            set => SetProperty(ref _LogFolderPath, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsPageDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            Task.Run(async () =>
            {
                StorageFolder folder = await Logger.GetLogFolderAsync();
                LogFolderPath = folder is null ? "" : folder.Path;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
