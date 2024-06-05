namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Core.SRM;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.ResourceManager.Objects;
    using Skyline.DataMiner.Net.Serialization;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;

    public class FindReservationsWithFiltersSection : FindItemsWithFiltersSection<ReservationInstance>
    {
        private readonly FilterSectionBase<ReservationInstance> reservationIdFilterSection = new GuidFilterSection<ReservationInstance>("Reservation ID", x => ReservationInstanceExposers.ID.Equal(x));

        private readonly FilterSectionBase<ReservationInstance> reservationIdIsPartOfFilterSection = new StringFilterSection<ReservationInstance>("Reservation ID is part of", x => new ORFilterElement<ReservationInstance>((x).Split(';').Select(split => Guid.Parse(split)).Select(guid => ReservationInstanceExposers.ID.Equal(guid)).ToArray()));

        private readonly FilterSectionBase<ReservationInstance> reservationServiceDefinitionIdFilterSection = new GuidFilterSection<ReservationInstance>("Service Definition ID", x => ServiceReservationInstanceExposers.ServiceDefinitionID.Equal(x));

        private readonly FilterSectionBase<ReservationInstance> reservationNameEqualsFilterSection = new StringFilterSection<ReservationInstance>("Reservation Name Equals", x => ReservationInstanceExposers.Name.Equal(x));

        private readonly FilterSectionBase<ReservationInstance> reservationNameContainsFilterSection = new StringFilterSection<ReservationInstance>("Reservation Name Contains", x => ReservationInstanceExposers.Name.Contains(x));

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

        public FindReservationsWithFiltersSection() : base()
        {
            addResourceFilterButton.Pressed += AddResourceFilterButton_Pressed;

            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;
        }

        public void AddDefaultPropertyFilter(string propertyName, string propertyValue)
        {
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>("Property", (propName, propValue) => ReservationInstanceExposers.Properties.DictStringField(propName).Equal(propValue));

            propertyFilterSection.SetDefault(propertyName, propertyValue);

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

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
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>("Property", (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).Equal(propertyValue));

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        private void AddResourceFilterButton_Pressed(object sender, EventArgs e)
        {
            var resourceFilterSection = new GuidFilterSection<ReservationInstance>("Uses Resource ID", (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.Contains((Guid)resourceId));

            resourceFilterSections.Add(resourceFilterSection);

            InvokeRegenerateUi();
        }

        protected override void HandleVisibilityAndEnabledUpdate(bool isVisible, bool isEnabled)
        {
            resourceFilterSections.ForEach(f => f.IsEnabled = isEnabled);

            propertyFilterSections.ForEach(f => f.IsEnabled = isEnabled);
        }

        protected override IEnumerable<ReservationInstance> FindItemsWithFilters()
        {
            return SrmManagers.ResourceManager.GetReservationInstances(GetCombinedFilterElement()).ToList();
        }

        protected override string GetItemIdentifier(ReservationInstance item)
        {
            return item.Name;
        }

        protected override void AddFilterSections(ref int row)
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
        }
    }
}
