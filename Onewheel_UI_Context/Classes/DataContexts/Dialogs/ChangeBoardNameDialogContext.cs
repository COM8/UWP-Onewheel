using Onewheel_UI_Context.Classes.DataTemplates.Dialogs;

namespace Onewheel_UI_Context.Classes.DataContexts.Dialogs
{
    public sealed class ChangeBoardNameDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeBoardNameDialogDataTemplate MODEL = new ChangeBoardNameDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnCanceled()
        {
            MODEL.OnCanceled();
        }

        public void OnAccepted()
        {
            MODEL.OnAccepted();
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
