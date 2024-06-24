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
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering resources.
	/// </summary>
	public class FindResourcesWithFiltersSection : FindItemsWithFiltersSection<Resource>
    {
        private readonly MultipleFiltersSection<Resource> functionGuidFilterSection = new MultipleFiltersSection<Resource>(new GuidFilterSection<Resource>(
            "Function ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<Resource>>>
            {
                {Comparers.Equals, x => FunctionResourceExposers.FunctionGUID.Equal(x) },
                {Comparers.NotEquals, x => FunctionResourceExposers.FunctionGUID.NotEqual(x) },
            }));

		private readonly MultipleFiltersSection<Resource> resourcePoolGuidFilterSection = new MultipleFiltersSection<Resource>(new GuidFilterSection<Resource>(
			"Resource Pool ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<Resource>>>
			{
				{Comparers.IsUsed, x => ResourceExposers.PoolGUIDs.Contains(x) },
				{Comparers.IsNotUsed, x => ResourceExposers.PoolGUIDs.NotContains(x) },
			}));

		private readonly MultipleFiltersSection<Resource> resourcePoolFilterSection;

        private readonly MultipleFiltersSection<Resource> nameFilterSection = new MultipleFiltersSection<Resource>(new StringFilterSection<Resource>(
            "Name",
            new Dictionary<Comparers, Func<string, FilterElement<Resource>>>
            {
                {Comparers.Equals, x => ResourceExposers.Name.Equal(x)},
                {Comparers.NotEquals, x => ResourceExposers.Name.NotEqual(x)},
                {Comparers.Contains, x => ResourceExposers.Name.Contains(x)},
                {Comparers.NotContains, x => ResourceExposers.Name.NotContains(x)},
            }));

		private readonly MultipleFiltersSection<Resource> descriptionFilterSection = new MultipleFiltersSection<Resource>(new StringFilterSection<Resource>(
			"Description",
			new Dictionary<Comparers, Func<string, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => ResourceExposers.Description.Equal(x)},
					{Comparers.NotEquals, x => ResourceExposers.Description.NotEqual(x)},
					{Comparers.Contains, x => ResourceExposers.Description.Contains(x)},
					{Comparers.NotContains, x => ResourceExposers.Description.NotContains(x)},
			}));

		private readonly MultipleFiltersSection<Resource> idFilterSection = new MultipleFiltersSection<Resource>(new GuidFilterSection<Resource>(
            "ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<Resource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ID.Equal(x)},
                { Comparers.NotEquals, x => ResourceExposers.ID.NotEqual(x) },
            }));

        private readonly MultipleFiltersSection<Resource> dmaIdFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
            "DMA ID",
            new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
            {
                {Comparers.Equals, x => ResourceExposers.DmaID.Equal(x)},
                {Comparers.NotEquals, x => ResourceExposers.DmaID.NotEqual(x)},
            }));

        private readonly MultipleFiltersSection<Resource> elementIdFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
            "Element ID",
            new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ElementID.Equal(x)},
                {Comparers.NotEquals, x => ResourceExposers.ElementID.NotEqual(x)},
            }));

		private readonly MultipleFiltersSection<Resource> mainDveDmaIdFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
	        "Main DVE DMA ID",
	        new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
	        {
				    {Comparers.Equals, x => FunctionResourceExposers.MainDVEDmaID.Equal(x)},
				    {Comparers.NotEquals, x => FunctionResourceExposers.MainDVEDmaID.NotEqual(x)},
	        }));

		private readonly MultipleFiltersSection<Resource> mainDveElementIdFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
			"Main DVE Element ID",
			new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => FunctionResourceExposers.MainDVEElementID.Equal(x)},
					{Comparers.NotEquals, x => FunctionResourceExposers.MainDVEElementID.NotEqual(x)},
			}));

		private readonly MultipleFiltersSection<Resource> modeFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
			"Mode",
			new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => ResourceExposers.Mode.Equal(x)},
					{Comparers.NotEquals, x => ResourceExposers.Mode.NotEqual(x)},
			}, string.Join("\n", Enum.GetValues(typeof(ResourceMode)).Cast<ResourceMode>().Select(x => $"{x}={(int)x}"))));

		private readonly MultipleFiltersSection<Resource> maxConcurrencyIdFilterSection = new MultipleFiltersSection<Resource>(new IntegerFilterSection<Resource>(
			"Max Concurrency",
			new Dictionary<Comparers, Func<int, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => ResourceExposers.MaxConcurrency.Equal(x)},
					{Comparers.NotEquals, x => ResourceExposers.MaxConcurrency.NotEqual(x)},
					{Comparers.GreaterThan, x => ResourceExposers.MaxConcurrency.GreaterThan(x)},
					{Comparers.LessThan, x => ResourceExposers.MaxConcurrency.LessThan(x)},
			}));

		private readonly MultipleFiltersSection<Resource> propertyFilterSections = new MultipleFiltersSection<Resource>(new StringStringFilterSection<Resource>(
			"Property",
			new Dictionary<Comparers, Func<string, string, FilterElement<Resource>>>
			{
				{Comparers.Equals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).Equal(propertyValue) },
				{Comparers.NotEquals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue) },
			}, "Name", "Value"));

        private readonly MultipleFiltersSection<Resource> capabilityFilterSections = new MultipleFiltersSection<Resource>(new GuidStringFilterSection<Resource>(
			"Discrete Capability",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<Resource>>>
			{
				{Comparers.Contains, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).Contains(cValue)},
				{Comparers.NotContains, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).NotContains(cValue)},
			}, "Guid", "Value", "ID of the capability is ID of the Profile Parameter"));


		private readonly MultipleFiltersSection<Resource> createdByFilterSection = new MultipleFiltersSection<Resource>(new StringFilterSection<Resource>(
			"Created By",
			new Dictionary<Comparers, Func<string, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => ResourceExposers.CreatedBy.Equal(x) },
					{Comparers.NotEquals, x => ResourceExposers.CreatedBy.NotEqual(x) },
					{Comparers.Contains, x => ResourceExposers.CreatedBy.Contains(x) },
					{Comparers.NotContains, x => ResourceExposers.CreatedBy.NotContains(x) },
			}));

		private readonly MultipleFiltersSection<Resource> lastModifiedByFilterSection = new MultipleFiltersSection<Resource>(new StringFilterSection<Resource>(
			"Last Modified By",
			new Dictionary<Comparers, Func<string, FilterElement<Resource>>>
			{
					{Comparers.Equals, x => ResourceExposers.LastModifiedBy.Equal(x) },
					{Comparers.NotEquals, x => ResourceExposers.LastModifiedBy.NotEqual(x) },
					{Comparers.Contains, x => ResourceExposers.LastModifiedBy.Contains(x) },
					{Comparers.NotContains, x => ResourceExposers.LastModifiedBy.NotContains(x) },
			}));

		private readonly MultipleFiltersSection<Resource> lastModifiedAtFilterSection = new MultipleFiltersSection<Resource>(new DateTimeFilterSection<Resource>(
			"Last Modified At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<Resource>>>
			{
					{Comparers.GreaterThan, x => ResourceExposers.LastModifiedAt.GreaterThan(x) },
					{Comparers.LessThan, x => ResourceExposers.LastModifiedAt.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<Resource> createdAtFilterSection = new MultipleFiltersSection<Resource>(new DateTimeFilterSection<Resource>(
			"Created At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<Resource>>>
			{
					{Comparers.GreaterThan, x => ResourceExposers.CreatedAt.GreaterThan(x) },
					{Comparers.LessThan, x => ResourceExposers.CreatedAt.LessThan(x) },
			}));

		private readonly ResourceManagerHelper resourceManagerHelper = new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);

		/// <summary>
		/// Initializes a new instance of the <see cref="FindResourcesWithFiltersSection"/>"/> class.
		/// </summary>
		public FindResourcesWithFiltersSection() : base()
        {
            var resourcePools = resourceManagerHelper.GetResourcePools() ?? new ResourcePool[0];

			resourcePoolFilterSection = new MultipleFiltersSection<Resource>(new ResourcePoolFilterSection(
                "Resource Pool",
                new Dictionary<Comparers, Func<Guid, FilterElement<Resource>>>
                {
                    {Comparers.Equals,  x => ResourceExposers.PoolGUIDs.Contains(x) }
                }, 
                resourcePools));

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

        /// <summary>
        /// Filtering all resources in system based on provided input.
        /// </summary>
        /// <returns>Collection of filtered resources.</returns>
        protected override IEnumerable<Resource> FindItemsWithFilters()
        {
            return resourceManagerHelper.GetResources(this.GetCombinedFilterElement()).ToList();
        }

        /// <summary>
        /// Gets name of resource.
        /// </summary>
        /// <param name="item">Resource for which we want to retrieve name.</param>
        /// <returns>Name of resource.</returns>
        protected override string IdentifyItem(Resource item)
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
			AddSection(idFilterSection, new SectionLayout(++row, 0));
            row += idFilterSection.RowCount;

			AddSection(nameFilterSection, new SectionLayout(row, 0));
            row += nameFilterSection.RowCount;

			AddSection(descriptionFilterSection, new SectionLayout(row, 0));
			row += descriptionFilterSection.RowCount;

			AddSection(functionGuidFilterSection, new SectionLayout(row, 0));
            row += functionGuidFilterSection.RowCount;

            AddSection(resourcePoolFilterSection, new SectionLayout(row, 0));
			row += resourcePoolFilterSection.RowCount;

			AddSection(resourcePoolGuidFilterSection, new SectionLayout(row, 0));
			row += resourcePoolGuidFilterSection.RowCount;

			AddSection(mainDveDmaIdFilterSection, new SectionLayout(row, 0));
			row += mainDveDmaIdFilterSection.RowCount;

			AddSection(mainDveElementIdFilterSection, new SectionLayout(row, 0));
			row += mainDveElementIdFilterSection.RowCount;

			AddSection(dmaIdFilterSection, new SectionLayout(row, 0));
			row += dmaIdFilterSection.RowCount;

			AddSection(elementIdFilterSection, new SectionLayout(row, 0));
			row += elementIdFilterSection.RowCount;

			AddSection(modeFilterSection, new SectionLayout(row, 0));
			row += modeFilterSection.RowCount;

			AddSection(maxConcurrencyIdFilterSection, new SectionLayout(row, 0));
			row += maxConcurrencyIdFilterSection.RowCount;

			AddSection(propertyFilterSections, new SectionLayout(row, 0));
            row += propertyFilterSections.RowCount;

			AddSection(capabilityFilterSections, new SectionLayout(row, 0));
			row += capabilityFilterSections.RowCount;

			AddSection(createdByFilterSection, new SectionLayout(row, 0));
			row += createdByFilterSection.RowCount;

			AddSection(createdAtFilterSection, new SectionLayout(row, 0));
			row += createdAtFilterSection.RowCount;

			AddSection(lastModifiedByFilterSection, new SectionLayout(row, 0));
			row += lastModifiedByFilterSection.RowCount;

			AddSection(lastModifiedAtFilterSection, new SectionLayout(row, 0));
			row += lastModifiedAtFilterSection.RowCount;

			firstAvailableColumn = ColumnCount + 1;
		}
	}
}
