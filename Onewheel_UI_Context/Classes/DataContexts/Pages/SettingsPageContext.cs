using Logging;
using Onewheel_UI_Context.Classes.DataTemplates.Pages;
using System.Threading.Tasks;

namespace Onewheel_UI_Context.Classes.DataContexts.Pages
{
    public sealed class SettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageDataTemplate MODEL = new SettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ExportLogsAsync()
        {
            await Logger.ExportLogsAsync();
        }

        public async Task DeleteLogsAsync()
        {
            await Logger.DeleteLogsAsync();
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
