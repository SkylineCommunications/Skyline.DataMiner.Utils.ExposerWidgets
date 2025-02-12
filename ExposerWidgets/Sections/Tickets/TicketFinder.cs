namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Tickets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Ticketing;
	using Skyline.DataMiner.Net.Tickets;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class TicketFinder : IDataMinerObjectFinder<Ticket>
	{
		private readonly TicketingGatewayHelper ticketingGatewayHelper;

		public TicketFinder(TicketingGatewayHelper ticketingGatewayHelper = null)
		{
			if (ticketingGatewayHelper == null)
			{
				this.ticketingGatewayHelper = new TicketingGatewayHelper { HandleEventsAsync = false };
				this.ticketingGatewayHelper.RequestResponseEvent += (sender, args) => args.responseMessage = Automation.Engine.SLNet.SendSingleResponseMessage(args.requestMessage);
			}
			else
			{
				this.ticketingGatewayHelper = ticketingGatewayHelper;
			}
		}

		public bool SupportsCountingObjects { get; } = false;

		public long CountObjects(FilterElement<Ticket> filterElement)
		{
			throw new NotSupportedException();
		}

		public IEnumerable<Ticket> FindObjects(FilterElement<Ticket> filterElement)
		{
			return ticketingGatewayHelper.GetTickets(null, filterElement, false).ToList();
		}

		public string IdentifyObject(Ticket dataMinerObject)
		{
			return dataMinerObject.ID.ToString();
		}
	}
}
