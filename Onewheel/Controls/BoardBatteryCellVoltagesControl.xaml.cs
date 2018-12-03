using Onewheel.DataTemplates;
using OnewheelBluetooth.Classes;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class BoardBatteryCellVoltagesControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableCollection<BatteryCellVoltageDataTemplate> VOLTAGES = new ObservableCollection<BatteryCellVoltageDataTemplate>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/12/2018 Created [Fabian Sauter]
        /// </history>
        public BoardBatteryCellVoltagesControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetVoltages(byte[] data)
        {
            if (data is null)
            {
                return;
            }
            double[] voltages = Utils.ToBatteryCellVoltages(data);
            VOLTAGES.Clear();
            for (uint i = 0; i < voltages.Length; i++)
            {
                VOLTAGES.Add(new BatteryCellVoltageDataTemplate(i, voltages[i]));
            }
        }

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


        #endregion
    }
}
