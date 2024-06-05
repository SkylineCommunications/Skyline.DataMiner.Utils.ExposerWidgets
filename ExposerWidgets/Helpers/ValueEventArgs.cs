namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
    using System;

    /// <summary>
    /// Event arguments for UI changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEventArgs{T}"/>"/> class.
        /// </summary>
        /// <param name="value">State of UI.</param>
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets statue value that UI should be in.
        /// </summary>
        public T Value { get; }
    }
}