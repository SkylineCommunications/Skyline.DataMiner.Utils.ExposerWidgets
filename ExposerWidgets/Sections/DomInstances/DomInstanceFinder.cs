namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.DomInstances
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class DomInstanceFinder : IDataMinerObjectFinder<DomInstance>
	{

		public DomInstanceFinder(DomHelper domHelper)
		{
			DomHelper = domHelper ?? throw new ArgumentNullException(nameof(domHelper));
		}

		public DomHelper DomHelper { get; }

		public bool SupportsCountingObjects { get; } = true;

		public long CountObjects(FilterElement<DomInstance> filterElement)
		{
			return DomHelper.DomInstances.Count(filterElement);
		}

		public IEnumerable<DomInstance> FindObjects(FilterElement<DomInstance> filterElement)
		{
			return DomHelper.DomInstances.Read(filterElement);
		}

		public string IdentifyObject(DomInstance dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID.Id}]";
		}
	}
}
