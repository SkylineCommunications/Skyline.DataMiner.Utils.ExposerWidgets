namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.ServiceDefinitions
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ServiceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class ServiceDefinitionFinder : IDataMinerObjectFinder<ServiceDefinition>
	{
		private readonly ServiceManagerHelper serviceManagerHelper;

		public ServiceDefinitionFinder(ServiceManagerHelper serviceManagerHelper = null)
		{
			if (serviceManagerHelper == null)
			{
				this.serviceManagerHelper = new ServiceManagerHelper();
				this.serviceManagerHelper.RequestResponseEvent += (s, e) => e.responseMessage = Engine.SLNet.SendSingleResponseMessage(e.requestMessage);
			}
			else
			{
				this.serviceManagerHelper = serviceManagerHelper;
			}
		}

		public bool SupportsCountingObjects { get; } = false;

		public long CountObjects(FilterElement<ServiceDefinition> filterElement)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<ServiceDefinition> FindObjects(FilterElement<ServiceDefinition> filterElement)
		{
			return serviceManagerHelper.GetServiceDefinitions(filterElement);
		}

		public string IdentifyObject(ServiceDefinition dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID}]";
		}
	}
}
