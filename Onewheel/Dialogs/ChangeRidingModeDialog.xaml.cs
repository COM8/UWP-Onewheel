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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadRidingModes()
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

                case OnewheelType.ONEWHEEL_PLUS:
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 4 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 5 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 6 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 7 });
                    MODES.Add(new RidingModeDataTemplate() { ridingMode = 8 });
                    break;

                case OnewheelType.ONEWHEEL_XR:
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

        private void showModeDescription()
        {
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
            loadRidingModes();
        }

        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, OnewheelBluetooth.Classes.Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE))
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => loadRidingModes()).AsTask();
            }
        }

        private void ContentDialog_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
        }

        private void ridingModes_cmbbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ridingModes_cmbbx.SelectedIndex >= 0 && ridingModes_cmbbx.SelectedItem is RidingModeDataTemplate)
            {
                RidingModeDataTemplate ridingModeDataTemplate = ridingModes_cmbbx.SelectedItem as RidingModeDataTemplate;
                selectedRideMode = ridingModeDataTemplate.ridingMode;
            }
            showModeDescription();
        }

        #endregion
    }
}
