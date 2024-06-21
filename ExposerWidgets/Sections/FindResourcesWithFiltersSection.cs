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
	public class FindResourcesWithFiltersSection : FindItemsWithFiltersSection<FunctionResource>
    {
        private readonly MultipleFiltersSection<FunctionResource> functionGuidFilterSection = new MultipleFiltersSection<FunctionResource>(new GuidFilterSection<FunctionResource>(
            "Function ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => FunctionResourceExposers.FunctionGUID.Equal(x).CAST<Resource, FunctionResource>() },
                {Comparers.NotEquals, x => FunctionResourceExposers.FunctionGUID.NotEqual(x).CAST<Resource, FunctionResource>() },
            }));

        private readonly MultipleFiltersSection<FunctionResource> resourcePoolFilterSection;

        private readonly MultipleFiltersSection<FunctionResource> resourceNameFilterSection = new MultipleFiltersSection<FunctionResource>(new StringFilterSection<FunctionResource>(
            "Name",
            new Dictionary<Comparers, Func<string, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.Name.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.Name.NotEqual(x).CAST<Resource, FunctionResource>()},
                {Comparers.Contains, x => ResourceExposers.Name.Contains(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotContains, x => ResourceExposers.Name.NotContains(x).CAST<Resource, FunctionResource>()},
            }));

        private readonly MultipleFiltersSection<FunctionResource> resourceIdFilterSection = new MultipleFiltersSection<FunctionResource>(new GuidFilterSection<FunctionResource>(
            "ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ID.Equal(x).CAST<Resource, FunctionResource>()},
                { Comparers.NotEquals, x => ResourceExposers.ID.NotEqual(x).CAST < Resource, FunctionResource >() },
            }));

        private readonly MultipleFiltersSection<FunctionResource> dmaIdFilterSection = new MultipleFiltersSection<FunctionResource>(new IntegerFilterSection<FunctionResource>(
            "DMA ID",
            new Dictionary<Comparers, Func<int, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.DmaID.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.DmaID.NotEqual(x).CAST<Resource, FunctionResource>()},
            }));

        private readonly MultipleFiltersSection<FunctionResource> elementIdFilterSection = new MultipleFiltersSection<FunctionResource>(new IntegerFilterSection<FunctionResource>(
            "Element ID",
            new Dictionary<Comparers, Func<int, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ElementID.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.ElementID.NotEqual(x).CAST<Resource, FunctionResource>()},
            }));

        private readonly MultipleFiltersSection<FunctionResource> propertyFilterSections = new MultipleFiltersSection<FunctionResource>(new StringStringFilterSection<FunctionResource>(
			"Property",
			new Dictionary<Comparers, Func<string, string, FilterElement<FunctionResource>>>
			{
				{Comparers.Equals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).Equal(propertyValue).CAST<Resource, FunctionResource>() },
				{Comparers.NotEquals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue).CAST<Resource, FunctionResource>() },
			}));

        private readonly MultipleFiltersSection<FunctionResource> capabilityFilterSections = new MultipleFiltersSection<FunctionResource>(new GuidStringFilterSection<FunctionResource>(
			"Discrete Capability",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<FunctionResource>>>
			{
				{Comparers.Contains, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).Contains(cValue).CAST<Resource, FunctionResource>()},
				{Comparers.NotContains, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).NotContains(cValue).CAST<Resource, FunctionResource>()},
			}));

        private readonly ResourceManagerHelper resourceManagerHelper = new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);

		/// <summary>
		/// Initializes a new instance of the <see cref="FindResourcesWithFiltersSection"/>"/> class.
		/// </summary>
		public FindResourcesWithFiltersSection() : base()
        {
            var resourcePools = resourceManagerHelper.GetResourcePools() ?? new ResourcePool[0];

			resourcePoolFilterSection = new MultipleFiltersSection<FunctionResource>(new ResourcePoolFilterSection(
                "Resource Pool",
                new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
                {
                    {Comparers.Equals,  x => ResourceExposers.PoolGUIDs.Contains(x).CAST<Resource, FunctionResource>() }
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
        protected override IEnumerable<FunctionResource> FindItemsWithFilters()
        {
            return resourceManagerHelper.GetResources(this.GetCombinedFilterElement().CAST<FunctionResource, Resource>()).Cast<FunctionResource>().ToList();
        }

        /// <summary>
        /// Gets name of resource.
        /// </summary>
        /// <param name="item">Resource for which we want to retrieve name.</param>
        /// <returns>Name of resource.</returns>
        protected override string GetItemIdentifier(FunctionResource item)
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
			AddSection(resourceIdFilterSection, new SectionLayout(row, 0));
            row += resourceIdFilterSection.RowCount;

			AddSection(resourceNameFilterSection, new SectionLayout(row, 0));
            row += resourceNameFilterSection.RowCount;

			AddSection(functionGuidFilterSection, new SectionLayout(row, 0));
            row += functionGuidFilterSection.RowCount;

            AddSection(resourcePoolFilterSection, new SectionLayout(row, 0));
			row += resourcePoolFilterSection.RowCount;

			AddSection(dmaIdFilterSection, new SectionLayout(row, 0));
			row += dmaIdFilterSection.RowCount;

			AddSection(elementIdFilterSection, new SectionLayout(row, 0));
			row += elementIdFilterSection.RowCount;

			AddSection(propertyFilterSections, new SectionLayout(row, 0));
            row += propertyFilterSections.RowCount;

			AddSection(capabilityFilterSections, new SectionLayout(row, 0));
			row += capabilityFilterSections.RowCount;

			firstAvailableColumn = ColumnCount + 1;
		}
	}
}
