namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Resources
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class ResourceFinder : IDataMinerObjectFinder<Resource>
	{
		private readonly ResourceManagerHelper resourceManagerHelper;

		public ResourceFinder(ResourceManagerHelper resourceManagerHelper = null)
		{
			this.resourceManagerHelper = resourceManagerHelper ?? new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);
		}

		public bool SupportsCountingObjects { get; } = false;

		public long CountObjects(FilterElement<Resource> filterElement)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<Resource> FindObjects(FilterElement<Resource> filterElement)
		{
			return resourceManagerHelper.GetResources(filterElement).ToList();
		}

		public string IdentifyObject(Resource dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID}]";
		}
	}
}
