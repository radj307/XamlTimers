using System.Windows;

namespace XamlTimers
{
    /// <summary>
    /// Behaviour that executes a configurable callback function on an interval.
    /// </summary>
    public class IntervalCallbackBehavior : BaseIntervalBehavior
    {
        #region TimerCallbackProperty
        private static void OnTimerCallbackPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is IntervalCallbackBehavior behavior)
                behavior.SetTimerCallback((System.Timers.ElapsedEventHandler)e.NewValue);
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="TimerCallback"/> property.
        /// </summary>
        public static readonly DependencyProperty TimerCallbackProperty
            = DependencyProperty.Register(
                nameof(TimerCallback),
                typeof(System.Timers.ElapsedEventHandler),
                typeof(IntervalCallbackBehavior),
                new PropertyMetadata(null, OnTimerCallbackPropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="BaseIntervalBehavior.EnableTimerProperty"/>.
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
    }
}
