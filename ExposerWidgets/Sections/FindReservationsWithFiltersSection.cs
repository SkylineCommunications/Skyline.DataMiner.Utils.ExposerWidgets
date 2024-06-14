namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ResourceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering reservations.
	/// </summary>
	public class FindReservationsWithFiltersSection : FindItemsWithFiltersSection<ReservationInstance>
    {
        private readonly FilterSectionBase<ReservationInstance> reservationIdFilterSection = new GuidFilterSection<ReservationInstance>("Reservation ID Equals", x => ReservationInstanceExposers.ID.Equal(x), x => ReservationInstanceExposers.ID.NotEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationIdIsPartOfFilterSection = new StringFilterSection<ReservationInstance>("Reservation ID is part of", x => new ORFilterElement<ReservationInstance>(x.Split(';').Select(split => Guid.Parse(split)).Select(guid => ReservationInstanceExposers.ID.Equal(guid)).ToArray()));

        private readonly FilterSectionBase<ReservationInstance> reservationServiceDefinitionIdFilterSection = new GuidFilterSection<ReservationInstance>("Service Definition ID Equals", x => ServiceReservationInstanceExposers.ServiceDefinitionID.Equal(x), x => ServiceReservationInstanceExposers.ServiceDefinitionID.NotEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationNameEqualsFilterSection = new StringFilterSection<ReservationInstance>("Reservation Name Equals", x => ReservationInstanceExposers.Name.Equal(x), x => ReservationInstanceExposers.Name.NotEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationNameContainsFilterSection = new StringFilterSection<ReservationInstance>("Reservation Name Contains", x => ReservationInstanceExposers.Name.Contains(x), x => ReservationInstanceExposers.Name.NotContains(x));

        private readonly FilterSectionBase<ReservationInstance> reservationStartFromFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Start From", x => ReservationInstanceExposers.Start.GreaterThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationStartUntilFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Start Until", x => ReservationInstanceExposers.Start.LessThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationEndFromFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation End From", x => ReservationInstanceExposers.End.GreaterThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationEndUntilFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation End Until", x => ReservationInstanceExposers.End.LessThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationCreatedAtFromFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Created At From", x => ReservationInstanceExposers.CreatedAt.GreaterThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationCreatedAtUntilFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Created At Until", x => ReservationInstanceExposers.CreatedAt.LessThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationLastModifiedAtFromFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Last Modified At From", x => ReservationInstanceExposers.LastModifiedAt.GreaterThanOrEqual(x));

        private readonly FilterSectionBase<ReservationInstance> reservationLastModifiedAtUntilFilterSection = new DateTimeFilterSection<ReservationInstance>("Reservation Last Modified At Until", x => ReservationInstanceExposers.LastModifiedAt.UncheckedLessThanOrEqual(x));

        private readonly List<FilterSectionBase<ReservationInstance>> resourceFilterSections = new List<FilterSectionBase<ReservationInstance>>();
        private readonly Button addResourceFilterButton = new Button("Add Resource Filter");

        private readonly List<FilterSectionBase<ReservationInstance>> propertyFilterSections = new List<FilterSectionBase<ReservationInstance>>();
        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");

        private readonly ResourceManagerHelper resourceManagerHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindReservationsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindReservationsWithFiltersSection() : base()
        {
			resourceManagerHelper = new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);

			addResourceFilterButton.Pressed += AddResourceFilterButton_Pressed;

            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;
        }

        /// <summary>
        /// Adds default input filter for custom properties.
        /// </summary>
        /// <param name="propertyName">Name of property on reservation.</param>
        /// <param name="propertyValue">Value of added property.</param>
        public void AddDefaultPropertyFilter(string propertyName, string propertyValue)
        {
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>("Property", (propName, propValue) => ReservationInstanceExposers.Properties.DictStringField(propName).Equal(propValue));

            propertyFilterSection.SetDefault(propertyName, propertyValue);

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi(); 
        }

        /// <summary>
        /// Resets filters in section to default values.
        /// </summary>
        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }

            resourceFilterSections.Clear();
            propertyFilterSections.Clear();
        }

        private void AddPropertyFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>("Property", (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).Equal(propertyValue), (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue));

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        private void AddResourceFilterButton_Pressed(object sender, EventArgs e)
        {
            var resourceFilterSection = new GuidFilterSection<ReservationInstance>("Uses Resource ID", (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.Contains(resourceId), (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.NotContains(resourceId));

            resourceFilterSections.Add(resourceFilterSection);

            InvokeRegenerateUi();
        }

        /// <summary>
        /// Filtering all reservation instances in system based on provided input.
        /// </summary>
        /// <returns>Collection of filtered reservation instances.</returns>
        protected override IEnumerable<ReservationInstance> FindItemsWithFilters()
        {
            return resourceManagerHelper.GetReservationInstances(GetCombinedFilterElement()).ToList();
        }

        /// <summary>
        /// Gets name of reservation instance.
        /// </summary>
        /// <param name="item">Reservation instance for which we want to retrieve name.</param>
        /// <returns>Name of reservation instance.</returns>
        protected override string GetItemIdentifier(ReservationInstance item)
        {
            return item.Name;
        }

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		/// <param name="row">Row on which section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddSection(reservationNameEqualsFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationNameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationIdFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationIdIsPartOfFilterSection, new SectionLayout(++row, 0));
            AddWidget(new Label("Provide ';' separated list of Guids"), ++row, 1);

            AddSection(reservationStartFromFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationStartUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationEndFromFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationEndUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationCreatedAtFromFilterSection, new SectionLayout(++row, 0));
            AddSection(reservationCreatedAtUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationLastModifiedAtFromFilterSection, new SectionLayout(++row, 0));
            AddSection(reservationLastModifiedAtUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationServiceDefinitionIdFilterSection, new SectionLayout(++row, 0));

            foreach (var resourceFilterSection in resourceFilterSections)
            {
                AddSection(resourceFilterSection, new SectionLayout(++row, 0));
            }

            foreach (var propertyFilterSection in propertyFilterSections)
            {
                AddSection(propertyFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addPropertyFilterButton, ++row, 0);
            AddWidget(addResourceFilterButton, ++row, 0);

            firstAvailableColumn = ColumnCount + 1;
        }
    }
}
