namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Tickets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Ticketing;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Section for filtering tickets.
	/// </summary>
	public class TicketFiltersSection : SectionContainingDataMinerObjectFilters<Ticket>
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

		private readonly MultipleFiltersSection<Ticket> ticketUidFilterSection = new MultipleFiltersSection<Ticket>(new GuidFilterSection<Ticket>(
			"Unique ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<Ticket>>>
			{
				{Comparers.Equals,  x => TicketingExposers.UniqueID.Equal(x) },
				{Comparers.NotEquals,  x => TicketingExposers.UniqueID.NotEqual(x) },
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
			}, "Name"));

		private readonly MultipleFiltersSection<Ticket> integerPropertyFilterSections = new MultipleFiltersSection<Ticket>(new StringIntegerFilterSection<Ticket>(
			"Property",
			new Dictionary<Comparers, Func<string, int, FilterElement<Ticket>>>
			{
				{Comparers.Equals, (pName, pValue) => TicketingExposers.CustomTicketFields.DictField(pName).Equal(pValue) },
				{Comparers.NotEquals, (pName, pValue) => TicketingExposers.CustomTicketFields.DictField(pName).NotEqual<Ticket, int>(pValue) },
			}, "Name"));

		private readonly MultipleFiltersSection<Ticket> enumPropertyFilterSections = new MultipleFiltersSection<Ticket>(new TicketEnumFilterSection(
			"Property",
			new Dictionary<Comparers, Func<string, string, int, FilterElement<Ticket>>>
			{
				{Comparers.Equals, (pName, pValue1, pValue2) => TicketingExposers.CustomTicketFields.DictField(pName).Equal($"{pValue1}/{pValue2}") },
				{Comparers.NotEquals, (pName, pValue1, pValue2) => TicketingExposers.CustomTicketFields.DictField(pName).NotEqual<Ticket, string>($"{pValue1}/{pValue2}") },
			}, "Name", "Enum display value", "Enter the enum display value and the enum integer value"));


		/// <summary>
		/// Initializes a new instance of the <see cref="TicketFiltersSection"/>"/> class.
		/// </summary>
		public TicketFiltersSection()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<Ticket> Clone()
		{
			return new TicketFiltersSection();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(ticketIdFilterSection, new SectionLayout(++row, 0));
			row += ticketIdFilterSection.RowCount;

			AddSection(ticketUidFilterSection, new SectionLayout(++row, 0));
			row += ticketUidFilterSection.RowCount;

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
		}
	}
}
