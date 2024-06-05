namespace Skyline.DataMiner.Utils.ExposerWidgets.Interfaces
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    /// <summary>
    /// Interface of DataMiner object type filters.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Object type that is filtered.</typeparam>
    public interface IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Indicates if the filter is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Indicates if the filter is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets filter created based on input values of filter.
        /// </summary>
        FilterElement<DataMinerObjectType> FilterElement { get; }

        /// <summary>
        /// Resets filter values to default values.
        /// </summary>
        void Reset();
    }
}