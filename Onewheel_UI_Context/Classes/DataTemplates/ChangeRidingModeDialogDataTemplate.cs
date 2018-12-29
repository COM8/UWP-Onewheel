using OnewheelBluetooth.Classes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Onewheel_UI_Context.Classes.DataTemplates
{
    public sealed class ChangeRidingModeDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private double _CarveAbility;
        public double CarveAbility
        {
            get { return _CarveAbility; }
            set { SetCarveAbility(value); }
        }
        private double _StanceProfile;
        public double StanceProfile
        {
            get { return _StanceProfile; }
            set { SetStanceProfile(value); }
        }
        private uint _Aggressiveness;
        public uint Aggressiveness
        {
            get { return _Aggressiveness; }
            set { SetAggressiveness(value); }
        }
        private RidingModeDataTemplate _SelectedMode;
        public RidingModeDataTemplate SelectedMode
        {
            get { return _SelectedMode; }
            set { SetProperty(ref _SelectedMode, value); }
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
                CarveAbility = Utils.CarveAbilityToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_CARVE_ABILITY);
            }
            else
            {
                CarveAbility = Utils.CarveAbilityToDouble(carve[0]);
            }
        }

        private void SetCarveAbility(double value)
        {
            if (SetProperty(ref _CarveAbility, value))
            {
                Task.Run(async () =>
                {
                    // Update carve
                });
            }
        }

        private void SetStanceProfile(byte[] stance)
        {
            if (stance is null || stance.Length != 2)
            {
                StanceProfile = Utils.StanceProfileToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_STANCE_PROFILE);
            }
            else
            {
                StanceProfile = Utils.StanceProfileToDouble(stance[0]);
            }
        }

        private void SetStanceProfile(double value)
        {
            if (SetProperty(ref _StanceProfile, value))
            {
                Task.Run(async () =>
                {
                    // Update carve
                });
            }
        }

        private void SetAggressiveness(byte[] aggr)
        {
            if (aggr is null || aggr.Length != 2)
            {
                Aggressiveness = Utils.AggressivenessToUInt((byte)Consts.CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS);
            }
            else
            {
                Aggressiveness = Utils.AggressivenessToUInt(aggr[0]);
            }
        }

        private void SetAggressiveness(uint value)
        {
            if (SetProperty(ref _Aggressiveness, value))
            {
                Task.Run(async () =>
                {
                    // Update carve
                });
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
            Aggressiveness = Utils.AggressivenessToUInt((byte)Consts.CUSTOM_SHAPING_DEFAULT_AGGRESSIVENESS);
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
            byte[] carve = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_CARVE_ABILITY);
            byte[] stance = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_STANCE_PROFILE);
            byte[] aggr = OnewheelConnectionHelper.INSTANCE.CACHE.GetBytes(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_AGGRESSIVENESS);

            SetCarveAbility(carve);
            SetStanceProfile(stance);
            SetAggressiveness(aggr);
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
                SetRidingMode(args.VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_CARVE_ABILITY))
            {
                SetCarveAbility(args.VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_STANCE_PROFILE))
            {
                SetStanceProfile(args.VALUE);
            }
            else if (args.UUID.Equals(OnewheelCharacteristicsCache.MOCK_CUSTOM_SHAPING_AGGRESSIVENESS))
            {
                SetAggressiveness(args.VALUE);
            }
        }

        #endregion
    }
}
