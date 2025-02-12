namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Reservations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ResourceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering reservations.
	/// </summary>
	public class ReservationFiltersSection : SectionContainingDataMinerObjectFilters<ReservationInstance>
	{
		private readonly MultipleFiltersSection<ReservationInstance> reservationIdFilterSection = new MultipleFiltersSection<ReservationInstance>(new GuidFilterSection<ReservationInstance>(
			"ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>>
			{
				{Comparers.Equals, x => ReservationInstanceExposers.ID.Equal(x) },
				{Comparers.NotEquals, x => ReservationInstanceExposers.ID.NotEqual(x) },
			}));

		private readonly MultipleFiltersSection<ReservationInstance> reservationServiceDefinitionIdFilterSection = new MultipleFiltersSection<ReservationInstance>(new GuidFilterSection<ReservationInstance>(
			"Service Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>>
			{
				{Comparers.Equals, x => ServiceReservationInstanceExposers.ServiceDefinitionID.Equal(x)},
				{Comparers.NotEquals, x => ServiceReservationInstanceExposers.ServiceDefinitionID.NotEqual(x) },
			}));

		private readonly MultipleFiltersSection<ReservationInstance> reservationNameFilterSection = new MultipleFiltersSection<ReservationInstance>(new StringFilterSection<ReservationInstance>(
			"Name",
			new Dictionary<Comparers, Func<string, FilterElement<ReservationInstance>>>
			{
				{Comparers.Equals, x => ReservationInstanceExposers.Name.Equal(x) },
				{Comparers.NotEquals, x => ReservationInstanceExposers.Name.NotEqual(x)},
				{Comparers.Contains, x => ReservationInstanceExposers.Name.Contains(x)},
				{Comparers.NotContains, x => ReservationInstanceExposers.Name.NotContains(x)},
			}));

		private readonly MultipleFiltersSection<ReservationInstance> reservationStartFilterSection = new MultipleFiltersSection<ReservationInstance>(new DateTimeFilterSection<ReservationInstance>(
			"Start",
			new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
			{
				{Comparers.GreaterThan, x => ReservationInstanceExposers.Start.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.Start.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<ReservationInstance> reservationEndFilterSection = new MultipleFiltersSection<ReservationInstance>(new DateTimeFilterSection<ReservationInstance>(
		   "End",
		   new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
		   {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.End.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.End.LessThan(x) },
		   }));

		private readonly MultipleFiltersSection<ReservationInstance> statusFilterSection = new MultipleFiltersSection<ReservationInstance>(new IntegerFilterSection<ReservationInstance>(
		   "Status",
		   new Dictionary<Comparers, Func<int, FilterElement<ReservationInstance>>>
		   {
				{Comparers.Equals, x => ReservationInstanceExposers.Status.Equal(x) },
				{Comparers.NotEquals, x => ReservationInstanceExposers.Status.NotEqual(x) },
		   },
		   string.Join("\n", Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>().Select(x => $"{x}={(int)x}"))));

		private readonly MultipleFiltersSection<ReservationInstance> hostingAgentIdFilterSection = new MultipleFiltersSection<ReservationInstance>(new IntegerFilterSection<ReservationInstance>(
		   "Hosting Agent ID",
		   new Dictionary<Comparers, Func<int, FilterElement<ReservationInstance>>>
		   {
				{Comparers.Equals, x => ReservationInstanceExposers.HostingAgentID.Equal(x) },
				{Comparers.NotEquals, x => ReservationInstanceExposers.HostingAgentID.NotEqual(x) },
		   }));

		private readonly MultipleFiltersSection<ReservationInstance> reservationCreatedAtFilterSection = new MultipleFiltersSection<ReservationInstance>(new DateTimeFilterSection<ReservationInstance>(
		   "Created At",
		   new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
		   {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.CreatedAt.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.CreatedAt.LessThan(x) },
		   }));

		private readonly MultipleFiltersSection<ReservationInstance> reservationLastModifiedAtFilterSection = new MultipleFiltersSection<ReservationInstance>(new DateTimeFilterSection<ReservationInstance>(
		   "Last Modified At",
		   new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
		   {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.LastModifiedAt.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.LastModifiedAt.LessThan(x) },
		   }));

		private readonly MultipleFiltersSection<ReservationInstance> resourceFiltersSection = new MultipleFiltersSection<ReservationInstance>(new GuidFilterSection<ReservationInstance>(
			"Resource",
			new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>>
			{
				{Comparers.IsUsed, (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.Contains(resourceId) },
				{Comparers.IsNotUsed, (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.NotContains(resourceId) },
			}));

		private readonly MultipleFiltersSection<ReservationInstance> propertyFiltersSection = new MultipleFiltersSection<ReservationInstance>(new StringStringFilterSection<ReservationInstance>(
			"Property",
			new Dictionary<Comparers, Func<string, string, FilterElement<ReservationInstance>>>
			{
				{Comparers.ExistsAndEquals, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).Equal(propertyValue) },
				{Comparers.ExistsAndNotEquals, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue) },
				{Comparers.ExistsAndContains, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).Contains(propertyValue) },
				{Comparers.ExistsAndNotContains, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).NotContains(propertyValue) },
			}, "Name", "Value"));

		private readonly MultipleFiltersSection<ReservationInstance> isQuarantinedFilterSection = new MultipleFiltersSection<ReservationInstance>(new BooleanFilterSection<ReservationInstance>(
		   "Is Quarantined",
		   new Dictionary<Comparers, Func<bool, FilterElement<ReservationInstance>>>
		   {
				{Comparers.Equals, x => ReservationInstanceExposers.IsQuarantined.Equal(x) },
		   }));

		/// <summary>
		/// Initializes a new instance of the <see cref="ReservationFiltersSection"/>"/> class.
		/// </summary>
		public ReservationFiltersSection()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<ReservationInstance> Clone()
		{
			return new ReservationFiltersSection();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(reservationNameFilterSection, new SectionLayout(++row, 0));
			row += reservationNameFilterSection.RowCount;

			AddSection(reservationIdFilterSection, new SectionLayout(row, 0));
			row += reservationIdFilterSection.RowCount;

			AddSection(reservationServiceDefinitionIdFilterSection, new SectionLayout(row, 0));
			row += reservationServiceDefinitionIdFilterSection.RowCount;

			AddSection(reservationStartFilterSection, new SectionLayout(row, 0));
			row += reservationStartFilterSection.RowCount;

			AddSection(reservationEndFilterSection, new SectionLayout(row, 0));
			row += reservationEndFilterSection.RowCount;

			AddSection(statusFilterSection, new SectionLayout(row, 0));
			row += statusFilterSection.RowCount;

			AddSection(hostingAgentIdFilterSection, new SectionLayout(row, 0));
			row += hostingAgentIdFilterSection.RowCount;

			AddSection(reservationCreatedAtFilterSection, new SectionLayout(row, 0));
			row += reservationCreatedAtFilterSection.RowCount;

			AddSection(reservationLastModifiedAtFilterSection, new SectionLayout(row, 0));
			row += reservationLastModifiedAtFilterSection.RowCount;

			AddSection(resourceFiltersSection, new SectionLayout(row, 0));
			row += resourceFiltersSection.RowCount;

			AddSection(propertyFiltersSection, new SectionLayout(row, 0));
			row += propertyFiltersSection.RowCount;

			AddSection(isQuarantinedFilterSection, new SectionLayout(row, 0));
			row += isQuarantinedFilterSection.RowCount;
		}
	}
}
