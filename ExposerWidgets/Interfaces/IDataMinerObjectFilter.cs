namespace Skyline.DataMiner.Utils.ExposerWidgets.Interfaces
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    public interface IDataMinerObjectFilter<DataMinerObjectType>
    {
        bool IsActive { get; }

        bool IsValid { get; }

        FilterElement<DataMinerObjectType> FilterElement { get; }

        void Reset();
    }
}