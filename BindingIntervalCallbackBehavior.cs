using System.Windows;

namespace UpdateBindingOnInterval
{
    /// <summary>
    /// Behaviour that executes a configurable callback function on an interval.
    /// </summary>
    /// <remarks><b>This is based on <see href="https://stackoverflow.com/a/44253691/8705305">this stackoverflow answer</see>.</b></remarks>
    public class BindingIntervalCallbackBehavior : Microsoft.Xaml.Behaviors.Behavior<DependencyObject>
    {
        #region DependencyProperties
        #region IntervalProperty
        // change the timer's update interval to the new value of IntervalProperty
        private static void OnIntervalPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BindingIntervalCallbackBehavior behaviour && behaviour._timer is not null)
                behaviour._timer.Interval = GetInterval(dependencyObject);
        }

        /// <inheritdoc cref="Interval"/>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(
                nameof(Interval),
                typeof(double),
                typeof(BindingIntervalCallbackBehavior),
                new PropertyMetadata((double)Timeout.Infinite, OnIntervalPropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="IntervalProperty"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        public static double GetInterval(DependencyObject o) => (double)o.GetValue(IntervalProperty);

        /// <summary>
        /// Sets the value of <see cref="IntervalProperty"/> to <paramref name="interval_ms"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <param name="interval_ms">Timer interval in milliseconds.<br/>See <see cref="System.Timers.Timer.Interval"/></param>
        public static void SetInterval(DependencyObject o, double interval_ms) => o.SetValue(IntervalProperty, interval_ms);

        /// <summary>
        /// Gets or sets the timer interval in milliseconds.
        /// </summary>
        public double Interval
        {
            get => GetInterval(this);
            set => SetInterval(this, value);
        }
        #endregion IntervalProperty

        #region EnableTimerProperty
        // enable or disable the timer depending on the new value of EnableTimerProperty; this is triggered from both the non-static EnableTimer property and from databindings
        private static void OnEnableTimerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BindingIntervalCallbackBehavior behaviour && behaviour._timer is not null)
            {
                behaviour._timer.Enabled = GetEnableTimer(dependencyObject);
            }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="EnableTimer"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableTimerProperty
            = DependencyProperty.Register(
                nameof(EnableTimer),
                typeof(bool),
                typeof(BindingIntervalCallbackBehavior),
                new PropertyMetadata(true, OnEnableTimerPropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="EnableTimerProperty"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <returns><see langword="true"/> when the timer is enabled; otherwise <see langword="false"/></returns>
        public static bool GetEnableTimer(DependencyObject o) => (bool)o.GetValue(EnableTimerProperty);

        /// <summary>
        /// Sets the value of <see cref="EnableTimerProperty"/> to <paramref name="state"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <param name="state"><see langword="true"/> enables the timer; <see langword="false"/> disables the timer.<br/>See <see cref="System.Timers.Timer.Enabled"/></param>
        public static void SetEnableTimer(DependencyObject o, bool state) => o.SetValue(EnableTimerProperty, state);

        /// <summary>
        /// Gets or sets whether the timer is enabled or not.
        /// </summary>
        public bool EnableTimer
        {
            get => GetEnableTimer(this);
            set => SetEnableTimer(this, value);
        }
        #endregion EnableTimerProperty

        #region TimerCallbackProperty
        private static void OnTimerCallbackPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BindingIntervalCallbackBehavior behavior)
                behavior._timerCallback = (System.Timers.ElapsedEventHandler)e.NewValue;
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="TimerCallback"/> property.
        /// </summary>
        public static readonly DependencyProperty TimerCallbackProperty 
            = DependencyProperty.Register(
                nameof(TimerCallback), 
                typeof(System.Timers.ElapsedEventHandler), 
                typeof(BindingIntervalCallbackBehavior), 
                new PropertyMetadata(null, OnTimerCallbackPropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="EnableTimerProperty"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <returns><see cref="System.Timers.ElapsedEventHandler"/></returns>
        public static System.Timers.ElapsedEventHandler? GetTimerCallback(DependencyObject o) => (System.Timers.ElapsedEventHandler?)o.GetValue(TimerCallbackProperty);

        /// <summary>
        /// Sets the value of <see cref="TimerCallbackProperty"/> to <paramref name="callback"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <param name="callback">A callback <see langword="delegate"/> of type <see cref="System.Timers.ElapsedEventHandler"/>.<br/>See <see cref="System.Timers.Timer.Elapsed"/></param>
        public static void SetTimerCallback(DependencyObject o, System.Timers.ElapsedEventHandler? callback) => o.SetValue(TimerCallbackProperty, callback);

        /// <summary>
        /// Gets or sets the timer callback method.
        /// </summary>
        public System.Timers.ElapsedEventHandler? TimerCallback
        {
            get => GetTimerCallback(this);
            set => SetTimerCallback(this, value);
        }
        #endregion TimerCallbackProperty
        #endregion DependencyProperties

        #region Fields
        private System.Timers.Timer? _timer;
        private System.Timers.ElapsedEventHandler? _timerCallback;
        #endregion Fields

        #region Methods
        private void InvokeTimerCallback(object? s, System.Timers.ElapsedEventArgs e)
        {
            if (_timerCallback is null)
                return;
            try
            {
                _ = this.Dispatcher.Invoke(_timerCallback, this, e);
            }
            catch (TaskCanceledException) { } //< this occurs when a task is cancelled, such as when the application is shutting down.
        }
        #endregion Methods

        #region EventHandlers
        /// <summary>
        /// Checks if the timer has been initialized by the <see cref="OnAttached"/> method.<br/>
        /// Dependency property changed event handlers should return immediately when the timer hasn't been initialized.
        /// </summary>
        /// <returns><see langword="true"/> when the timer has been initialized; otherwise <see langword="false"/>.</returns>
        protected bool TimerIsNull() => _timer is null;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            var interval = this.Interval;
            if (interval <= 0) throw new ArgumentOutOfRangeException(nameof(this.Interval), this.Interval, $"Property {nameof(this.Interval)} must be greater than 0!");
            _timer = new(interval) { AutoReset = true };

            _timer.Elapsed += this.InvokeTimerCallback;

            base.OnAttached();

            _timer.Enabled = this.EnableTimer;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            _timer?.Dispose();
            this.TimerCallback = null;
            base.OnDetaching();
        }
        #endregion EventHandlers
    }
}
