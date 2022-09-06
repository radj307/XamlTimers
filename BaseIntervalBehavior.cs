using System.Windows;

namespace XamlTimers
{
    /// <summary>
    /// <see langword="abstract"/> base of the <see cref="IntervalCallbackBehavior"/> &amp; <see cref="IntervalUpdateBinding"/> behaviors.
    /// </summary>
    public abstract class BaseIntervalBehavior : Microsoft.Xaml.Behaviors.Behavior<DependencyObject>
    {
        #region IntervalProperty
        // change the timer's update interval to the new value of IntervalProperty
        private static void OnIntervalPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BaseIntervalBehavior behaviour && behaviour._timer is not null)
                behaviour._timer.Interval = GetInterval(dependencyObject);
        }

        /// <inheritdoc cref="Interval"/>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(
                nameof(Interval),
                typeof(double),
                typeof(BaseIntervalBehavior),
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
            if (dependencyObject is BaseIntervalBehavior behaviour && behaviour._timer is not null)
                behaviour._timer.Enabled = GetEnableTimer(dependencyObject);
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="EnableTimer"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableTimerProperty
            = DependencyProperty.Register(
                nameof(EnableTimer),
                typeof(bool),
                typeof(BaseIntervalBehavior),
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

        #region IsEventSenderAssociatedObjectProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="IsEventSenderAssociatedObject"/> property.
        /// </summary>
        public static readonly DependencyProperty IsEventSenderAssociatedObjectProperty 
            = DependencyProperty.Register(
                nameof(IsEventSenderAssociatedObject),
                typeof(bool),
                typeof(BaseIntervalBehavior),
                new PropertyMetadata(false)
                );

        /// <summary>
        /// Gets the value of <see cref="IsEventSenderAssociatedObjectProperty"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <returns><see langword="true"/> when the 'sender' parameter on event callbacks triggered from this behavior is replaced with the <see cref="DependencyObject"/> that the behavior is attached to; <see langword="false"/> when the 'sender' parameter is set to the behavior instance that triggered the event.</returns>
        public static bool GetIsEventSenderAssociatedObject(DependencyObject o) => (bool)o.GetValue(IsEventSenderAssociatedObjectProperty);

        /// <summary>
        /// Sets the value of <see cref="IsEventSenderAssociatedObjectProperty"/> to <paramref name="state"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <param name="state"><see langword="true"/> replaces the 'sender' parameter on event callbacks triggered from this behavior with the <see cref="DependencyObject"/> it is attached to; <see langword="false"/> does not replace the 'sender' parameter of events (The 'sender' parameter in this case is set to the behavior instance that triggered the event).</param>
        public static void SetIsEventSenderAssociatedObject(DependencyObject o, bool state) => o.SetValue(IsEventSenderAssociatedObjectProperty, state);

        /// <summary>
        /// Gets or sets whether the <see cref="TimerCallback"/>  "sender" parameter 
        /// </summary>
        public bool IsEventSenderAssociatedObject
        {
            get => GetIsEventSenderAssociatedObject(this);
            set => SetIsEventSenderAssociatedObject(this, value);
        }
        #endregion IsEventSenderAssociatedObjectProperty

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
                object? sender = null;
                this.Dispatcher.Invoke(() =>
                {
                    sender = IsEventSenderAssociatedObject ? AssociatedObject : this;
                });
                _ = this.Dispatcher.Invoke(_timerCallback, sender, e);
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
        /// <summary>
        /// Sets the timer callback to trigger when the timer interval has elapsed.
        /// </summary>
        /// <param name="callback">A <see cref="System.Timers.ElapsedEventHandler"/> delegate to use as the callback method.</param>
        protected void SetTimerCallback(System.Timers.ElapsedEventHandler? callback) => _timerCallback = callback;

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
            _timerCallback = null;
            base.OnDetaching();
        }
        #endregion EventHandlers
    }
}
