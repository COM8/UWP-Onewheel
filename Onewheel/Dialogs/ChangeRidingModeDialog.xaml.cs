using Onewheel.Controls;
using Onewheel_UI_Context.Classes.DataContexts.Dialogs;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Dialogs
{
    public sealed partial class ChangeRidingModeDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeRidingModeDialogDataContext VIEW_MODEL = new ChangeRidingModeDialogDataContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/04/2018 Created [Fabian Sauter]
        /// </history>
		public ChangeRidingModeDialog()
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
        private void ResetCarve_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VIEW_MODEL.ResetCarveAbility();
        }

        private void ResetStance_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VIEW_MODEL.ResetStanceProfile();
        }

        private void ResetAggressiveness_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VIEW_MODEL.ResetAggressiveness();
        }

        private async void Stance_sldr_ValueChangeCompleted(ExtendedSlider sender, Classes.Events.SliderValueChangeCompletedEventArgs args)
        {
            await VIEW_MODEL.SaveStanceProfileAsync();
        }

        private async void Aggressiveness_sldr_ValueChangeCompleted(ExtendedSlider sender, Classes.Events.SliderValueChangeCompletedEventArgs args)
        {
            await VIEW_MODEL.SaveAggressivenessAsync();
        }

        private async void Carve_sldr_ValueChangeCompleted(ExtendedSlider sender, Classes.Events.SliderValueChangeCompletedEventArgs args)
        {
            await VIEW_MODEL.SaveCarveAbilityAsync();
        }

        #endregion
    }
}
