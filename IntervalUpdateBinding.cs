using System.Windows;

namespace UpdateBindingOnInterval
{
    /// <summary>
    /// Behaviour that updates a specified dependency property's data binding on an interval.
    /// </summary>
    public class IntervalUpdateBinding : BaseIntervalBehavior
    {
        #region Properties
        /// <summary>
        /// Gets or sets the target <see cref="DependencyProperty"/> relative to this behaviour instance's <see cref="Microsoft.Xaml.Behaviors.Behavior.AssociatedObject"/>.
        /// </summary>
        public DependencyProperty? Property { get; set; }
        /// <summary>
        /// When <see langword="true"/>, <see cref="ArgumentNullException"/> is thrown by the update method when <see cref="Property"/> is set to <see langword="null"/>.<br/>
        /// Otherwise when <see langword="false"/>, no exception is thrown and the binding update silently fails.<br/><br/>
        /// This does <b>not</b> prevent <see cref="ArgumentOutOfRangeException"/>; if you want to disable that, see <see cref="ThrowWhenPropertyIsMissing"/>.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool ThrowWhenPropertyIsNull { get; set; } = true;
        /// <summary>
        /// When <see langword="true"/>, <see cref="ArgumentOutOfRangeException"/> is thrown by the update method when <see cref="Property"/> doesn't exist within the attached object.<br/>
        /// Otherwise when <see langword="false"/>, no exception is thrown and the binding update silently fails.<br/><br/>
        /// This does <b>not</b> prevent <see cref="ArgumentNullException"/>; if you want to disable that, see <see cref="ThrowWhenPropertyIsNull"/>.
        /// </summary>
        /// <remarks><b>Default: <see langword="true"/></b></remarks>
        public bool ThrowWhenPropertyIsMissing { get; set; } = true;
        #endregion Properties

        #region UpdateModeProperty
        private static void OnUpdateModePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is IntervalUpdateBinding behaviour && !behaviour.TimerIsNull())
                behaviour.SetTimerCallback(behaviour.GetCallback());
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> definition for the <see cref="UpdateMode"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateModeProperty
            = DependencyProperty.Register(
                nameof(UpdateMode),
                typeof(EUpdateMode),
                typeof(IntervalCallback),
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
        /// <summary>
        /// Gets the binding expression for the target <see cref="Property"/> from the AssociatedObject.
        /// </summary>
        /// <returns>The target <see cref="System.Windows.Data.BindingExpression"/> when <see cref="Property"/> is valid; otherwise <see langword="null"/> if an exception isn't thrown because it is disabled.</returns>
        /// <exception cref="ArgumentNullException"><see cref="Property"/> is <see langword="null"/> <b>and</b> <see cref="ThrowWhenPropertyIsNull"/> is <see langword="true"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="Property"/> doesn't exist on the AssociatedObject <b>and</b> <see cref="ThrowWhenPropertyIsMissing"/> is <see langword="true"/></exception>
        public System.Windows.Data.BindingExpression? GetBindingExpression()
        {
            if (this.Property is null)
            {
                if (ThrowWhenPropertyIsNull)
                    throw new ArgumentNullException(nameof(this.Property), $"{nameof(Property)} cannot be null!");
                else
                    return null;
            }

            var expr = System.Windows.Data.BindingOperations.GetBindingExpression(this.AssociatedObject, this.Property);

            if (ThrowWhenPropertyIsMissing && expr is null) //< throw if property is missing
                throw new ArgumentOutOfRangeException(nameof(this.Property), this.Property, $"'{this.Property.Name}' isn't a static {nameof(DependencyProperty)} for object of type '{this.AssociatedType.Name}'");

            return expr;
        }
        #endregion Methods

        #region EventHandlers
        private void Action_UpdateTarget(object? s, System.Timers.ElapsedEventArgs e) => GetBindingExpression()?.UpdateTarget();
        private void Action_UpdateSource(object? s, System.Timers.ElapsedEventArgs e) => GetBindingExpression()?.UpdateSource();
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            if (this.Property is null)
                throw new ArgumentNullException(nameof(this.Property), $"{nameof(this.Property)} cannot be null!");

            this.SetTimerCallback(this.GetCallback());

            base.OnAttached();
        }
        #endregion EventHandlers
    }
}
