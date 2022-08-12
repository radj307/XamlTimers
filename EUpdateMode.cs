namespace XamlTimers
{
    /// <summary>
    /// Binding update mode definitions
    /// </summary>
    public enum EUpdateMode : byte
    {
        /// <summary>
        /// Update the binding <b>target</b> from the source.
        /// </summary>
        UpdateTarget,
        /// <summary>
        /// Update the binding <b>source</b> from the target.
        /// </summary>
        UpdateSource,
    }
}
