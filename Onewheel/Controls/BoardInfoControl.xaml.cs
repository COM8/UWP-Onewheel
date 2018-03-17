using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    public sealed partial class BoardInfoControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText", typeof(string), typeof(BoardInfoControl), null);

        public string DescriptionPrimary
        {
            get { return (string)GetValue(DescriptionPrimaryProperty); }
            set { SetValue(DescriptionPrimaryProperty, value); }
        }
        public static readonly DependencyProperty DescriptionPrimaryProperty = DependencyProperty.Register("DescriptionPrimary", typeof(string), typeof(BoardInfoControl), null);

        public string DescriptionSecondary
        {
            get { return (string)GetValue(DescriptionSecondaryProperty); }
            set { SetValue(DescriptionSecondaryProperty, value); }
        }
        public static readonly DependencyProperty DescriptionSecondaryProperty = DependencyProperty.Register("DescriptionSecondary", typeof(string), typeof(BoardInfoControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public BoardInfoControl()
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


        #endregion
    }
}
