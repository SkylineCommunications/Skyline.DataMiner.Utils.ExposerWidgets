namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public interface IDataMinerObjectFinder<DataMinerObjectType>
	{
		bool SupportsCountingObjects { get; }

		string IdentifyObject(DataMinerObjectType dataMinerObject);

		IEnumerable<DataMinerObjectType> FindObjects(FilterElement<DataMinerObjectType> filterElement);
		
		long CountObjects(FilterElement<DataMinerObjectType> filterElement);
	}
}
