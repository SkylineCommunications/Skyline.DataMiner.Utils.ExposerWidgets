namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Ticketing;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	/// <summary>
	/// Section for filtering tickets.
	/// </summary>
	public class FindTicketsWithFiltersSection : FindItemsWithFiltersSection<Ticket>
    {
        private readonly MultipleFiltersSection<Ticket> ticketIdFilterSection = new MultipleFiltersSection<Ticket>(new StringFilterSection<Ticket>(
            "ID",
            new Dictionary<Comparers, Func<string, FilterElement<Ticket>>>
            {
                {Comparers.Equals,  x => TicketingExposers.FullID.Equal(x) },
                {Comparers.NotEquals,  x => TicketingExposers.FullID.NotEqual(x) },
                {Comparers.Contains,  x => TicketingExposers.FullID.Contains(x) },
                {Comparers.NotContains,  x => TicketingExposers.FullID.NotContains(x) },
            }));

        private readonly MultipleFiltersSection<Ticket> ticketCreationDateFilterSection = new MultipleFiltersSection<Ticket>(new DateTimeFilterSection<Ticket>(
            "Creation Date", 
            new Dictionary<Comparers, Func<DateTime, FilterElement<Ticket>>>
            {
                {Comparers.GreaterThan, x => TicketingExposers.CreationDate.GreaterThan(x) },
                {Comparers.LessThan, x => TicketingExposers.CreationDate.LessThan(x) },
            }));

        private readonly MultipleFiltersSection<Ticket> ticketDomainFilterSection = new MultipleFiltersSection<Ticket>(new GuidFilterSection<Ticket>(
            "Ticket Domain ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<Ticket>>>
            {
                {Comparers.Equals, x => TicketingExposers.ResolverID.Equal(x)  },
                {Comparers.NotEquals, x => TicketingExposers.ResolverID.NotEqual(x) },
            }));

        private readonly MultipleFiltersSection<Ticket> stringPropertyFilterSections = new MultipleFiltersSection<Ticket>(new StringStringFilterSection<Ticket>(
			"Property",
			new Dictionary<Comparers, Func<string, string, FilterElement<Ticket>>>
			{
				{Comparers.Equals, (pName, pValue) => TicketingExposers.CustomTicketFields.DictStringField(pName).Equal(pValue) },
				{Comparers.NotEquals, (pName, pValue) => TicketingExposers.CustomTicketFields.DictStringField(pName).NotEqual(pValue) },
				{Comparers.Contains, (pName, pValue) => TicketingExposers.CustomTicketFields.DictStringField(pName).Contains(pValue) },
				{Comparers.NotContains, (pName, pValue) => TicketingExposers.CustomTicketFields.DictStringField(pName).NotContains(pValue)},
			}));

        private readonly MultipleFiltersSection<Ticket> integerPropertyFilterSections = new MultipleFiltersSection<Ticket>(new StringIntegerFilterSection<Ticket>(
			"Property",
			new Dictionary<Comparers, Func<string, int, FilterElement<Ticket>>>
			{
				{Comparers.Equals, (pName, pValue) => TicketingExposers.CustomTicketFields.DictField(pName).Equal(pValue) },
			}));

        private readonly MultipleFiltersSection<Ticket> enumPropertyFilterSections = new MultipleFiltersSection<Ticket>(new TicketEnumFilterSection(
			"Property",
			new Dictionary<Comparers, Func<string, string, int, FilterElement<Ticket>>>
			{
				{Comparers.Equals, (pName, pValue1, pValue2) => TicketingExposers.CustomTicketFields.DictField(pName).Equal($"{pValue1}/{pValue2}") },
			}));

        private readonly TicketingGatewayHelper ticketingHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindTicketsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindTicketsWithFiltersSection() : base()
        {            
            ticketingHelper = new TicketingGatewayHelper { HandleEventsAsync = false };
            ticketingHelper.RequestResponseEvent += (sender, args) => args.responseMessage = Skyline.DataMiner.Automation.Engine.SLNet.SendSingleResponseMessage(args.requestMessage);

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

        /// <summary>
        /// Filtering all tickets in system based on provided input.
        /// </summary>
        /// <returns>Collection of filtered tickets.</returns>
        protected override IEnumerable<Ticket> FindItemsWithFilters()
        {
            return new HashSet<Ticket>(ticketingHelper.GetTickets(null, GetCombinedFilterElement(), false).ToList());
        }

        /// <summary>
        /// Gets name of ticket.
        /// </summary>
        /// <param name="item">Ticket for which we want to retrieve name.</param>
        /// <returns>Name of ticket.</returns>
        protected override string GetItemIdentifier(Ticket item)
        {
            return item.ID.ToString();
        }

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		/// <param name="row">Row on which section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddSection(ticketIdFilterSection, new SectionLayout(++row, 0));
			row += ticketIdFilterSection.RowCount;

			AddSection(ticketCreationDateFilterSection, new SectionLayout(row, 0));
			row += ticketCreationDateFilterSection.RowCount;

			AddSection(ticketDomainFilterSection, new SectionLayout(row, 0));
            row += ticketDomainFilterSection.RowCount;

            AddSection(stringPropertyFilterSections, new SectionLayout(row, 0));
            row += stringPropertyFilterSections.RowCount;

			AddSection(integerPropertyFilterSections, new SectionLayout(row, 0));
			row += integerPropertyFilterSections.RowCount;

			AddSection(enumPropertyFilterSections, new SectionLayout(row, 0));
			row += enumPropertyFilterSections.RowCount;

			firstAvailableColumn = ColumnCount + 1;
		}
	}
}
