namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
    using System;

    public class ValueEventArgs<T> : EventArgs
    {
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}