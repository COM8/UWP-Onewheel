using Onewheel.Classes.Events;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onewheel.Controls
{
    sealed class ExtendedSlider : Slider
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public delegate void SlideValueChangeCompletedEventHandler(ExtendedSlider sender, SliderValueChangeCompletedEventArgs args);
        public event SlideValueChangeCompletedEventHandler ValueChangeCompleted;

        public bool IsValueChanging
        {
            get { return (bool)GetValue(IsValueChangingProperty); }
            set { SetValue(IsValueChangingProperty, value); }
        }
        public static readonly DependencyProperty IsValueChangingProperty = DependencyProperty.Register(nameof(IsValueChanging), typeof(bool), typeof(ExtendedSlider), new PropertyMetadata(false));

        private DispatcherTimer isValueChangingTimer = new DispatcherTimer();

        private bool initialValueSet = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ExtendedSlider()
        {
            isValueChangingTimer.Tick += IsChangingTimer_Tick;
            isValueChangingTimer.Interval = TimeSpan.FromSeconds(0.5);
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
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (oldValue != newValue)
            {
                if (!initialValueSet)
                {
                    initialValueSet = true;
                    return;
                }

                IsValueChanging = true;
                isValueChangingTimer.Stop();
                if (!isValueChangingTimer.IsEnabled)
                {
                    isValueChangingTimer.Start();
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void IsChangingTimer_Tick(object sender, object e)
        {
            if (IsValueChanging && isValueChangingTimer.IsEnabled)
            {
                isValueChangingTimer.Stop();
                IsValueChanging = false;
                ValueChangeCompleted?.Invoke(this, new SliderValueChangeCompletedEventArgs(Value));
            }
        }

        #endregion
    }
}
