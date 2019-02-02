using OnewheelBluetooth.Classes;
using Shared.Classes;

namespace Onewheel_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class ChangeBoardNameDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _CustomName;
        public string CustomName
        {
            get { return _CustomName; }
            set { SetProperty(ref _CustomName, value); }
        }
        private bool _Accepted;
        public bool Accepted
        {
            get { return _Accepted; }
            set { SetProperty(ref _Accepted, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangeBoardNameDialogDataTemplate()
        {
            CustomName = OnewheelConnectionHelper.INSTANCE.CACHE.GetString(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnCanceled()
        {
            Accepted = false;
        }

        public void OnAccepted()
        {
            Accepted = true;
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
