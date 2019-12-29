using Onewheel_UI_Context.Classes.DataTemplates.Controls.Settings;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Onewheel_UI_Context.Classes.DataContexts.Controls.Settings
{
    public sealed class FolderSizeControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly FolderSizeControlDataTemplate MODEL = new FolderSizeControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateViewAsync(DependencyPropertyChangedEventArgs e)
        {
            if (!Equals(e.OldValue, e.NewValue) && e.NewValue is string path)
            {
                await MODEL.UpdateViewAsync(path);
            }
        }

        public async Task RecalculateFolderSizeAsync(string path)
        {
            await MODEL.UpdateViewAsync(path);
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
