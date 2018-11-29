using Onewheel.DataTemplates;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using System;
using OnewheelBluetooth.Classes;

namespace Onewheel.Dialogs
{
    public sealed partial class ChangeRidingModeDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableCollection<RidingModeDataTemplate> MODES;
        public bool canceled;
        public uint selectedRideMode;

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
            this.MODES = new ObservableCollection<RidingModeDataTemplate>();
            this.canceled = true;
            this.selectedRideMode = 0;
            this.InitializeComponent();

            carve_sldr.Value = Consts.CUSTOM_SHAPING_CARVE_DEFAULT;
            stance_sldr.Value = Consts.CUSTOM_SHAPING_STANCE_DEFAULT;
            aggressiveness_sldr.Value = Consts.CUSTOM_SHAPING_AGGRESSIVENESS_DEFAULT;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Tuple<int, double, int> GetCustomShpingValues()
        {
            return new Tuple<int, double, int>((int)carve_sldr.Value, stance_sldr.Value, (int)aggressiveness_sldr.Value);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadRidingModes()
        {
            MODES.Clear();
            OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
            switch (onewheel is null ? OnewheelType.ONEWHEEL_PLUS : onewheel.TYPE)
            {
                case OnewheelType.ONEWHEEL:
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 1 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 2 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 3 });
                    break;

                case OnewheelType.ONEWHEEL_XR:
                case OnewheelType.ONEWHEEL_PLUS:
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 4 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 5 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 6 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 7 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 8 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 9 });
                    break;
            }

            selectedRideMode = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE);
            for (int i = 0; i < MODES.Count; i++)
            {
                if (MODES[i].ridingMode == selectedRideMode)
                {
                    ridingModes_cmbbx.SelectedIndex = i;
                    break;
                }
            }
        }

        private void ShowModeDescription()
        {
            customShaping_stckp.Visibility = selectedRideMode == 9 ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;

            switch (selectedRideMode)
            {
                case 1:
                    description_tbx.Text = "";
                    break;

                case 2:
                    description_tbx.Text = "";
                    break;

                case 3:
                    description_tbx.Text = "";
                    break;

                // SEQUOIA:
                case 4:
                    description_tbx.Text = "Max Velocity 20km/h.";
                    break;

                // CRUZ:
                case 5:
                    description_tbx.Text = "Max Velocity 24km/h.";
                    break;

                // MISSION:
                case 6:
                    description_tbx.Text = "Max Velocity 31km/h.";
                    break;

                // ELEVATE:
                case 7:
                    description_tbx.Text = "Max Velocity 31km/h.";
                    break;

                // DELIRIUM:
                case 8:
                    description_tbx.Text = "Max Velocity 32km/h.";
                    break;

                // CUSTOM SHAPING:
                case 9:
                    description_tbx.Text = "Custom shaping.";
                    break;

                default:
                    description_tbx.Text = "";
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.canceled = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.canceled = false;
        }

        private void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;
            LoadRidingModes();
        }

        private void ContentDialog_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
        }

        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, OnewheelBluetooth.Classes.Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE))
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoadRidingModes()).AsTask();
            }
        }

        private void ridingModes_cmbbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ridingModes_cmbbx.SelectedIndex >= 0 && ridingModes_cmbbx.SelectedItem is RidingModeDataTemplate)
            {
                RidingModeDataTemplate ridingModeDataTemplate = ridingModes_cmbbx.SelectedItem as RidingModeDataTemplate;
                selectedRideMode = ridingModeDataTemplate.ridingMode;
            }
            ShowModeDescription();
        }

        private void ResetCarve_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            carve_sldr.Value = Consts.CUSTOM_SHAPING_CARVE_DEFAULT;
        }

        private void Carve_sldr_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            resetCarve_btn.IsEnabled = carve_sldr.Value != Consts.CUSTOM_SHAPING_CARVE_DEFAULT;
        }

        private void Stance_sldr_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            resetStance_btn.IsEnabled = stance_sldr.Value != Consts.CUSTOM_SHAPING_STANCE_DEFAULT;
        }

        private void ResetStance_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            stance_sldr.Value = Consts.CUSTOM_SHAPING_STANCE_DEFAULT;
        }

        private void Aggressiveness_sldr_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            resetAggressiveness_btn.IsEnabled = aggressiveness_sldr.Value != Consts.CUSTOM_SHAPING_AGGRESSIVENESS_DEFAULT;
        }

        private void ResetAggressiveness_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            aggressiveness_sldr.Value = Consts.CUSTOM_SHAPING_AGGRESSIVENESS_DEFAULT;
        }

        #endregion
    }
}
