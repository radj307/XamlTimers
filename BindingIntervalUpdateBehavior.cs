using System.Windows;

namespace UpdateBindingOnInterval
{
    /// <summary>
    /// Behaviour that updates a specified dependency property's data binding on an interval.
    /// </summary>
    /// <remarks><b>This is based on <see href="https://stackoverflow.com/a/44253691/8705305">this stackoverflow answer</see>.</b></remarks>
    public class BindingIntervalUpdateBehavior : BindingIntervalCallbackBehavior
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="BindingIntervalUpdateBehavior"/> instance.
        /// </summary>
        static BindingIntervalUpdateBehavior() => TimerCallbackProperty.OverrideMetadata(typeof(BindingIntervalUpdateBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, OnTimerCallbackPropertyChanged));
        #endregion Constructor

        #region Properties
        /// <summary>Gets or sets the target <see cref="DependencyProperty"/> relative to this behaviour instance's <see cref="Microsoft.Xaml.Behaviors.Behavior.AssociatedObject"/>.</summary>
        public DependencyProperty? Property { get; set; }
        #endregion Properties

        #region UpdateModeProperty
        private static void OnUpdateModePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is BindingIntervalUpdateBehavior behaviour && !behaviour.TimerIsNull())
                behaviour.TimerCallback = behaviour.GetCallback();
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="UpdateMode"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateModeProperty
            = DependencyProperty.Register(
                nameof(UpdateMode),
                typeof(EUpdateMode),
                typeof(BindingIntervalCallbackBehavior),
                new PropertyMetadata(EUpdateMode.UpdateTarget, OnUpdateModePropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="UpdateModeProperty"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <returns><see cref="EUpdateMode"/></returns>
        public static EUpdateMode GetUpdateMode(DependencyObject o) => (EUpdateMode)o.GetValue(UpdateModeProperty);

        /// <summary>
        /// Sets the value of <see cref="UpdateModeProperty"/> to <paramref name="updateMode"/>.
        /// </summary>
        /// <param name="o"><see cref="DependencyObject"/> instance.</param>
        /// <param name="updateMode"><see cref="EUpdateMode.UpdateTarget"/> updates the binding <b>target</b> from the binding source, and requires a readable binding source;<br/>
        /// <see cref="EUpdateMode.UpdateSource"/> updates the binding <b>source</b> from the binding target, and requires a writable binding source.</param>
        public static void SetUpdateMode(DependencyObject o, EUpdateMode updateMode) => o.SetValue(UpdateModeProperty, updateMode);

        /// <summary>
        /// Gets or sets the binding update mode, which determines whether to update the binding target or the binding source.
        /// </summary>
        /// <remarks>
        /// <see cref="EUpdateMode.UpdateTarget"/> updates the binding <b>target</b> from the binding source, and requires a readable binding source;<br/>
        /// <see cref="EUpdateMode.UpdateSource"/> updates the binding <b>source</b> from the binding target, and requires a writable binding source.
        /// </remarks>
        public EUpdateMode UpdateMode
        {
            get => GetUpdateMode(this);
            set => SetUpdateMode(this, value);
        }
        #endregion UpdateModeProperty

        #region Methods
        private System.Timers.ElapsedEventHandler GetCallback() => this.UpdateMode switch
        {
            EUpdateMode.UpdateTarget => this.Action_UpdateTarget,
            EUpdateMode.UpdateSource => this.Action_UpdateSource,
            _ => throw new ArgumentOutOfRangeException(nameof(this.UpdateMode), this.UpdateMode, $"{nameof(this.UpdateMode)} must be '{EUpdateMode.UpdateTarget:G}' or '{EUpdateMode.UpdateTarget:G}'"),
        };
        #endregion Methods

        #region EventHandlers
        private static void OnTimerCallbackPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) { return; }

        private void Action_UpdateTarget(object? s, System.Timers.ElapsedEventArgs e) => System.Windows.Data.BindingOperations.GetBindingExpression(this.AssociatedObject, this.Property)?.UpdateTarget();
        private void Action_UpdateSource(object? s, System.Timers.ElapsedEventArgs e) => System.Windows.Data.BindingOperations.GetBindingExpression(this.AssociatedObject, this.Property)?.UpdateSource();
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            if (this.Property is null)
                throw new ArgumentNullException(nameof(this.Property), $"{nameof(this.Property)} cannot be null!");

            this.TimerCallback = this.GetCallback();

            base.OnAttached();
        }
        #endregion EventHandlers
    }
}
