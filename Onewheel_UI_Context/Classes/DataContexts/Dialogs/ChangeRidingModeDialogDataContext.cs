using Onewheel_UI_Context.Classes.DataTemplates.Dialogs;
using System.Threading.Tasks;

namespace Onewheel_UI_Context.Classes.DataContexts.Dialogs
{
    public class ChangeRidingModeDialogDataContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeRidingModeDialogDataTemplate MODEL = new ChangeRidingModeDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ResetCarveAbility()
        {
            MODEL.ResetCarveAbility();
        }

        public void ResetStanceProfile()
        {
            MODEL.ResetStanceProfile();
        }

        public void ResetAggressiveness()
        {
            MODEL.ResetAggressiveness();
        }

        public async Task SaveCarveAbilityAsync()
        {
            await MODEL.SaveCarveAbilityAsync();
        }

        public async Task SaveStanceProfileAsync()
        {
            await MODEL.SaveStanceProfileAsync();
        }

        public async Task SaveAggressivenessAsync()
        {
            await MODEL.SaveAggressivenessAsync();
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
