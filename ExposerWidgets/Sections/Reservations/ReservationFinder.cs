namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Reservations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ResourceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class ReservationFinder : IDataMinerObjectFinder<ReservationInstance>
	{
		private readonly ResourceManagerHelper resourceManagerHelper;

		public ReservationFinder(ResourceManagerHelper resourceManagerHelper = null)
		{
			this.resourceManagerHelper = resourceManagerHelper ?? new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);
		}
		public bool SupportsCountingObjects { get; } = false;

		public long CountObjects(FilterElement<ReservationInstance> filterElement)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<ReservationInstance> FindObjects(FilterElement<ReservationInstance> filterElement)
		{
			return resourceManagerHelper.GetReservationInstances(filterElement).ToList();
		}

		public string IdentifyObject(ReservationInstance dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID}]";
		}
	}
}
