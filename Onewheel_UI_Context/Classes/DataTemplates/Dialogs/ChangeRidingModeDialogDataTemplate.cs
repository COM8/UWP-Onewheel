using Logging;
using OnewheelBluetooth.Classes;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Onewheel_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class ChangeRidingModeDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private double _CarveAbility;
        public double CarveAbility
        {
            get { return _CarveAbility; }
            set { SetProperty(ref _CarveAbility, value); }
        }
        private double _StanceProfile;
        public double StanceProfile
        {
            get { return _StanceProfile; }
            set { SetProperty(ref _StanceProfile, value); }
        }
        private double _Aggressiveness;
        public double Aggressiveness
        {
            get { return _Aggressiveness; }
            set { SetProperty(ref _Aggressiveness, value); }
        }
        private RidingModeDataTemplate _SelectedMode;
        public RidingModeDataTemplate SelectedMode
        {
            get { return _SelectedMode; }
            set { SetProperty(ref _SelectedMode, value); }
        }
        private string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set { SetProperty(ref _StatusText, value); }
        }
        private SolidColorBrush _StatusTextBrush;
        public SolidColorBrush StatusTextBrush
        {
            get { return _StatusTextBrush; }
            set { SetProperty(ref _StatusTextBrush, value); }
        }

        public readonly ObservableCollection<RidingModeDataTemplate> MODES = new ObservableCollection<RidingModeDataTemplate>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangeRidingModeDialogDataTemplate()
        {
            LoadRidingModes();
            LoadCustomShaping();
            OnewheelConnectionHelper.INSTANCE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetCarveAbility(byte[] carve)
        {
            if (carve is null || carve.Length != 2)
            {
                ResetCarveAbility();
            }
            else
            {
                CarveAbility = Utils.CarveAbilityToDouble(carve[0]);
            }
        }

        private void SetStanceProfile(byte[] stance)
        {
            if (stance is null || stance.Length != 2)
            {
                ResetStanceProfile();
            }
            else
            {
                StanceProfile = Utils.StanceProfileToDouble(stance[0]);
            }
        }

        private void SetAggressiveness(byte[] aggr)
        {
            if (aggr is null || aggr.Length != 2)
            {
                ResetAggressiveness();
            }
            else
            {
                Aggressiveness = Utils.AggressivenessToDouble(aggr[0]);
            }
        }

        private void SetRidingMode(byte[] data)
        {
            uint mode = OnewheelCharacteristicsCache.GetUint(data);
            for (int i = 0; i < MODES.Count; i++)
            {
                if (MODES[i].Mode == mode)
                {
                    SelectedMode = MODES[i];
                    break;
                }
            }
        }

        private void SetErrorText(string text)
        {
            StatusText = text;
            StatusTextBrush = new SolidColorBrush(Colors.Red);
        }

        private void SetSuccessText(string text)
        {
            StatusText = text;
            StatusTextBrush = new SolidColorBrush(Colors.Green);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ResetCarveAbility()
        {
            CarveAbility = Utils.CarveAbilityToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_CARVE_ABILITY);
        }

        public void ResetStanceProfile()
        {
            StanceProfile = Utils.StanceProfileToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_STANCE_PROFILE);
        }

        public void ResetAggressiveness()
        {
            Aggressiveness = Utils.AggressivenessToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS);
        }

        public async Task SaveCarveAbilityAsync()
        {
            byte[] data = new byte[2];
            data[0] = Consts.CUSTOM_SHAPING_IDENT_CARVE_ABILITY;
            data[1] = Utils.CarveAbilityToByte(CarveAbility);
            if (!await SaveCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_SHAPING, data, "Carve ability"))
            {
                LoadCarveAbility();
            }
        }

        public async Task SaveStanceProfileAsync()
        {
            byte[] data = new byte[2];
            data[0] = Consts.CUSTOM_SHAPING_IDENT_STANCE_PROFILE;
            data[1] = Utils.StanceProfileToByte(StanceProfile);
            if (!await SaveCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_SHAPING, data, "Stance profile"))
            {
                LoadStanceProfile();
            }
        }

        public async Task SaveAggressivenessAsync()
        {
            byte[] data = new byte[2];
            data[0] = Consts.CUSTOM_SHAPING_IDENT_AGGRESSIVENESS;
            data[1] = Utils.AggressivenessToByte(Aggressiveness);
            if (!await SaveCharacteristicAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_SHAPING, data, "Aggressiveness"))
            {
                LoadAggressiveness();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadRidingModes()
        {
            MODES.Clear();
            OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
            switch (onewheel is null ? OnewheelType.ONEWHEEL_PLUS : onewheel.TYPE)
            {
                case OnewheelType.ONEWHEEL:
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_CLASSIC_CLASSIC });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_CLASSIC_XTREME });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_CLASSIC_ELEVATE });
                    break;

                case OnewheelType.ONEWHEEL_XR:
                case OnewheelType.ONEWHEEL_PLUS:
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_SEQUOIA });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_CRUZ });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_MISSION });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_ELEVATE });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_DELIRIUM });
                    MODES.Add(new RidingModeDataTemplate() { Mode = Consts.RIDING_MODE_OW_PLUS_CUSTOM_SHAPING });
                    break;
            }

            byte[] mode = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE);
            SetRidingMode(mode);
        }

        private void LoadCustomShaping()
        {
            LoadCarveAbility();
            LoadStanceProfile();
            LoadAggressiveness();
        }

        private void LoadCarveAbility()
        {
            byte[] carve = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_CARVE_ABILITY);
            SetCarveAbility(carve);
        }

        private void LoadStanceProfile()
        {
            byte[] stance = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_STANCE_PROFILE);
            SetStanceProfile(stance);
        }

        private void LoadAggressiveness()
        {
            byte[] aggr = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_AGGRESSIVENESS);
            SetAggressiveness(aggr);
        }

        private async Task<bool> SaveCharacteristicAsync(Guid uuid, byte[] data, string name)
        {
            uint curSpeed = OnewheelConnectionHelper.INSTANCE.CACHE.GetUint(OnewheelCharacteristicsCache.CHARACTERISTIC_SPEED_RPM);
            if (curSpeed <= 0)
            {
                OnewheelBoard onewheel = OnewheelConnectionHelper.INSTANCE.GetOnewheel();
                if (onewheel is null || onewheel.GetBoard().ConnectionStatus == Windows.Devices.Bluetooth.BluetoothConnectionStatus.Disconnected)
                {
                    // Not connected:
                    SetErrorText("Not connected to Onewheel!");
                    Logger.Info("Failed to update " + name + " - not connected.");
                    return false;
                }
                else
                {
                    GattWriteResult result = await onewheel.WriteBytesAsync(uuid, data);
                    if (!(result is null) && result.Status == GattCommunicationStatus.Success)
                    {
                        // Success:
                        SetSuccessText(name + " updated!");
                        Logger.Info(name + " updated to: " + Utils.ByteArrayToHexString(data));
                        return true;
                    }
                    else
                    {
                        // Internal error:
                        SetErrorText("Failed to update " + name + " - internal error!");
                        Logger.Info("Failed to update " + name + " - " + (result is null ? "characteristic not found" : result.Status.ToString()) + '.');
                        return false;
                    }
                }
            }
            else
            {
                // Too fast:
                SetErrorText("Failed to update " + name + " - too fast!");
                Logger.Info("Failed to update " + name + " - too fast(" + curSpeed + ").");
                return false;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CACHE_CharacteristicChanged(OnewheelCharacteristicsCache sender, OnewheelBluetooth.Classes.Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(OnewheelCharacteristicsCache.CHARACTERISTIC_RIDING_MODE))
            {
                SetRidingMode(args.NEW_VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_CARVE_ABILITY))
            {
                SetCarveAbility(args.NEW_VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_STANCE_PROFILE))
            {
                SetStanceProfile(args.NEW_VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_AGGRESSIVENESS))
            {
                SetAggressiveness(args.NEW_VALUE);
            }
        }

        #endregion
    }
}
