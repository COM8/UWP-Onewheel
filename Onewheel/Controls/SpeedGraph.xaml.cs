using LiveCharts;
using LiveCharts.Configurations;
using Onewheel.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Onewheel.Controls
{
    public sealed partial class SpeedGraph : UserControl, INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public double AxisMin
        {
            get { return (double)GetValue(AxisMinProperty); }
            set
            {
                SetValue(AxisMinProperty, value);
                OnPropertyChanged(nameof(AxisMin));
            }
        }
        public static readonly DependencyProperty AxisMinProperty = DependencyProperty.Register(nameof(AxisMin), typeof(double), typeof(SpeedGraph), new PropertyMetadata(0));

        public double AxisMax
        {
            get { return (double)GetValue(AxisMaxProperty); }
            set { SetValue(AxisMaxProperty, value); }
        }
        public static readonly DependencyProperty AxisMaxProperty = DependencyProperty.Register(nameof(AxisMax), typeof(double), typeof(SpeedGraph), new PropertyMetadata(0));

        private readonly ChartValues<SpeedMeasurement> CHART_VALUES;
        private readonly DispatcherTimer TIMER;
        private readonly Random RANDOM;

        private double AxisStep { get; set; }

        private readonly List<double> SPEED_VALUE_CACHE;
        private static readonly SemaphoreSlim SPEED_VALUE_CACHE_SEMA = new SemaphoreSlim(1);

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 27/08/2018 Created [Fabian Sauter]
        /// </history>
        public SpeedGraph()
        {
            this.InitializeComponent();

            var mapper = Mappers.Xy<SpeedMeasurement>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<SpeedMeasurement>(mapper);


            //the values property will store our values array
            CHART_VALUES = new ChartValues<SpeedMeasurement>();
            SPEED_VALUE_CACHE = new List<double>();

            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            SetAxisLimits(DateTime.Now);

            TIMER = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            TIMER.Tick += TimerOnTick;
            RANDOM = new Random();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(0).Ticks; // lets force the axis to be 0 seconds ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks; //we only care about the last 60 seconds
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void addValue(double value)
        {
            SPEED_VALUE_CACHE_SEMA.Wait();
            SPEED_VALUE_CACHE.Add(value);
            SPEED_VALUE_CACHE_SEMA.Release();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void addSpeedMeasurement(SpeedMeasurement measurement)
        {
            CHART_VALUES.Add(measurement);
            SetAxisLimits(measurement.DateTime);

            if (DateTime.Now.Subtract(CHART_VALUES.Last().DateTime).TotalSeconds > 60)
            {
                CHART_VALUES.RemoveAt(0);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // TIMER.Start();
        }

        private void TimerOnTick(object sender, object eventArgs)
        {
            Task.Run(async () =>
            {
                DateTime now = DateTime.Now;
                double sum = 0;

                SPEED_VALUE_CACHE_SEMA.Wait();
                if (SPEED_VALUE_CACHE.Count >= 0)
                {
                    foreach (double d in SPEED_VALUE_CACHE)
                    {
                        sum += d;
                    }
                    sum /= SPEED_VALUE_CACHE.Count;
                    SPEED_VALUE_CACHE.Clear();
                }
                SPEED_VALUE_CACHE_SEMA.Release();

                if(sum < 0)
                {
                    sum = 0;
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    addSpeedMeasurement(new SpeedMeasurement
                    {
                        DateTime = now,
                        Value = 0
                    });
                });
            });
        }

        #endregion
    }
}
