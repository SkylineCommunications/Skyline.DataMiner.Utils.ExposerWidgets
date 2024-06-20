namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
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
	public class FindReservationsWithFiltersSection : FindItemsWithFiltersSection<ReservationInstance>
    {
        private readonly FilterSectionBase<ReservationInstance> reservationIdFilterSection = new GuidFilterSection<ReservationInstance>(
            "Reservation ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>> 
            {
                {Comparers.Equals, x => ReservationInstanceExposers.ID.Equal(x) },
                {Comparers.NotEquals, x => ReservationInstanceExposers.ID.NotEqual(x) },
            });

        private readonly FilterSectionBase<ReservationInstance> reservationServiceDefinitionIdFilterSection = new GuidFilterSection<ReservationInstance>(
            "Service Definition ID", 
            new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>> 
            {
                {Comparers.Equals, x => ServiceReservationInstanceExposers.ServiceDefinitionID.Equal(x)},
                {Comparers.NotEquals, x => ServiceReservationInstanceExposers.ServiceDefinitionID.NotEqual(x) },
            });

        private readonly FilterSectionBase<ReservationInstance> reservationNameFilterSection = new StringFilterSection<ReservationInstance>(
            "Reservation Name",
            new Dictionary<Comparers, Func<string, FilterElement<ReservationInstance>>> 
            {
				{Comparers.Equals, x => ReservationInstanceExposers.Name.Equal(x) },
				{Comparers.NotEquals, x => ReservationInstanceExposers.Name.NotEqual(x)},
				{Comparers.Contains, x => ReservationInstanceExposers.Name.Contains(x)},
				{Comparers.NotContains, x => ReservationInstanceExposers.Name.NotContains(x)},
			});

        private readonly FilterSectionBase<ReservationInstance> reservationStartFilterSection = new DateTimeFilterSection<ReservationInstance>(
            "Reservation Start",
            new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>> 
            {
                {Comparers.GreaterThan, x => ReservationInstanceExposers.Start.GreaterThan(x) },
                {Comparers.LessThan, x => ReservationInstanceExposers.Start.LessThan(x) },
            });

		private readonly FilterSectionBase<ReservationInstance> reservationEndFilterSection = new DateTimeFilterSection<ReservationInstance>(
		   "Reservation End",
		   new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
		   {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.End.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.End.LessThan(x) },
		   });

		private readonly FilterSectionBase<ReservationInstance> reservationCreatedAtFilterSection = new DateTimeFilterSection<ReservationInstance>(
           "Reservation Created At",
           new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
           {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.CreatedAt.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.CreatedAt.LessThan(x) },
           });

		private readonly FilterSectionBase<ReservationInstance> reservationLastModifiedAtFilterSection = new DateTimeFilterSection<ReservationInstance>(
           "Reservation Last Modified At",
           new Dictionary<Comparers, Func<DateTime, FilterElement<ReservationInstance>>>
           {
				{Comparers.GreaterThan, x => ReservationInstanceExposers.LastModifiedAt.GreaterThan(x) },
				{Comparers.LessThan, x => ReservationInstanceExposers.LastModifiedAt.LessThan(x) },
           });

		private readonly List<FilterSectionBase<ReservationInstance>> resourceFilterSections = new List<FilterSectionBase<ReservationInstance>>();
        private readonly Button addResourceFilterButton = new Button("Add Resource Filter");

        private readonly List<FilterSectionBase<ReservationInstance>> propertyFilterSections = new List<FilterSectionBase<ReservationInstance>>();
        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");

        private readonly ResourceManagerHelper resourceManagerHelper = new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);

		/// <summary>
		/// Initializes a new instance of the <see cref="FindReservationsWithFiltersSection"/>"/> class.
		/// </summary>
		public FindReservationsWithFiltersSection() : base()
        {
			addResourceFilterButton.Pressed += AddResourceFilterButton_Pressed;

            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;
			GenerateUi();
		}

        /// <summary>
        /// Adds default input filter for custom properties.
        /// </summary>
        /// <param name="propertyName">Name of property on reservation.</param>
        /// <param name="propertyValue">Value of added property.</param>
        public void AddDefaultPropertyFilter(string propertyName, string propertyValue)
        {
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>(
				"Property",
				new Dictionary<Comparers, Func<string, string, FilterElement<ReservationInstance>>>
				{
					{Comparers.Equals, (propName, propValue) => ReservationInstanceExposers.Properties.DictStringField(propName).Equal(propValue) },
					{Comparers.NotEquals, (propName, propValue) => ReservationInstanceExposers.Properties.DictStringField(propName).NotEqual(propValue) },
				});

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
            var propertyFilterSection = new StringStringFilterSection<ReservationInstance>(
                "Property",
                new Dictionary<Comparers, Func<string, string, FilterElement<ReservationInstance>>> 
                {
                    {Comparers.Equals, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).Equal(propertyValue) },
                    {Comparers.NotEquals, (propertyName, propertyValue) => ReservationInstanceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue) },
                });

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        private void AddResourceFilterButton_Pressed(object sender, EventArgs e)
        {
            var resourceFilterSection = new GuidFilterSection<ReservationInstance>(
                "Resource ID",
                new Dictionary<Comparers, Func<Guid, FilterElement<ReservationInstance>>>
                {
                    {Comparers.Exists, (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.Contains(resourceId) },
                    {Comparers.NotExists, (resourceId) => ReservationInstanceExposers.ResourceIDsInReservationInstance.NotContains(resourceId) },
                });

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
            AddSection(reservationNameFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationIdFilterSection, new SectionLayout(++row, 0));

			AddSection(reservationServiceDefinitionIdFilterSection, new SectionLayout(++row, 0));

			AddSection(reservationStartFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationEndFilterSection, new SectionLayout(++row, 0));

            AddSection(reservationCreatedAtFilterSection, new SectionLayout(++row, 0));
            
            AddSection(reservationLastModifiedAtFilterSection, new SectionLayout(++row, 0));
           
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
