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
        private readonly FilterSectionBase<FunctionResource> functionGuidFilterSection = new GuidFilterSection<FunctionResource>(
            "Function Guid",
            new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => FunctionResourceExposers.FunctionGUID.Equal(x).CAST<Resource, FunctionResource>() },
                {Comparers.NotEquals, x => FunctionResourceExposers.FunctionGUID.NotEqual(x).CAST<Resource, FunctionResource>() },
            });

        private readonly FilterSectionBase<FunctionResource> resourcePoolFilterSection;

        private readonly FilterSectionBase<FunctionResource> resourceNameFilterSection = new StringFilterSection<FunctionResource>(
            "Resource Name",
            new Dictionary<Comparers, Func<string, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.Name.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.Name.NotEqual(x).CAST<Resource, FunctionResource>()},
                {Comparers.Contains, x => ResourceExposers.Name.Contains(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotContains, x => ResourceExposers.Name.NotContains(x).CAST<Resource, FunctionResource>()},
            });

        private readonly FilterSectionBase<FunctionResource> resourceIdFilterSection = new GuidFilterSection<FunctionResource>(
            "Resource ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ID.Equal(x).CAST<Resource, FunctionResource>()},
                { Comparers.NotEquals, x => ResourceExposers.ID.NotEqual(x).CAST < Resource, FunctionResource >() },
            });

        private readonly FilterSectionBase<FunctionResource> dmaIdFilterSection = new IntegerFilterSection<FunctionResource>(
            "DMA ID",
            new Dictionary<Comparers, Func<int, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.DmaID.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.DmaID.NotEqual(x).CAST<Resource, FunctionResource>()},
            });

        private readonly FilterSectionBase<FunctionResource> elementIdFilterSection = new IntegerFilterSection<FunctionResource>(
            "Element ID",
            new Dictionary<Comparers, Func<int, FilterElement<FunctionResource>>>
            {
                {Comparers.Equals, x => ResourceExposers.ElementID.Equal(x).CAST<Resource, FunctionResource>()},
                {Comparers.NotEquals, x => ResourceExposers.ElementID.NotEqual(x).CAST<Resource, FunctionResource>()},
            });

        private readonly List<FilterSectionBase<FunctionResource>> propertyFilterSections = new List<FilterSectionBase<FunctionResource>>();
        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");
        private readonly Button addPropertyExistenceFilterButton = new Button("Add Property Existence Filter");

        private readonly List<FilterSectionBase<FunctionResource>> capabilityFilterSections = new List<FilterSectionBase<FunctionResource>>();
        private readonly Button addCapabilityContainsFilterButton = new Button("Add 'Capability Contains' Filter");
        private readonly Button addCapabilityExistenceFilterButton = new Button("Add 'Capability Exists' Filter");

        private readonly ResourceManagerHelper resourceManagerHelper = new ResourceManagerHelper(Engine.SLNet.SendSingleResponseMessage);

		/// <summary>
		/// Initializes a new instance of the <see cref="FindResourcesWithFiltersSection"/>"/> class.
		/// </summary>
		public FindResourcesWithFiltersSection() : base()
        {
            var resourcePools = resourceManagerHelper.GetResourcePools() ?? new ResourcePool[0];

			resourcePoolFilterSection = new ResourcePoolFilterSection(
                "Resource Pool",
                new Dictionary<Comparers, Func<Guid, FilterElement<FunctionResource>>>
                {
                    {Comparers.Equals,  x => ResourceExposers.PoolGUIDs.Contains(x).CAST<Resource, FunctionResource>() }
                }, 
                resourcePools);

			addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;

            addPropertyExistenceFilterButton.Pressed += AddPropertyExistenceFilterButton_Pressed;

            addCapabilityContainsFilterButton.Pressed += AddCapabilityContainsFilterButton_Pressed;

            addCapabilityExistenceFilterButton.Pressed += AddCapabilityExistenceFilterButton_Pressed;

			GenerateUi();
		}

		private void AddCapabilityContainsFilterButton_Pressed(object sender, EventArgs e)
        {
            var capabilityFilterSection = new GuidStringFilterSection<FunctionResource>(
                "Discrete Capability Contains",
                new Dictionary<Comparers, Func<Guid, string, FilterElement<FunctionResource>>>
                {
                    {Comparers.Exists, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).Contains(cValue).CAST<Resource, FunctionResource>()},
                    {Comparers.NotExists, (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(cId).NotContains(cValue).CAST<Resource, FunctionResource>()},
                });

            capabilityFilterSections.Add(capabilityFilterSection);

            InvokeRegenerateUi();
        }

        private void AddCapabilityExistenceFilterButton_Pressed(object sender, EventArgs e)
        {
            var capabilityExistenceFilterSection = new StringFilterSection<FunctionResource>(
                "Discrete Capability", 
                new Dictionary<Comparers, Func<string, FilterElement<FunctionResource>>>
                {
                    {Comparers.Exists,  (cId) => ResourceExposers.Capabilities.DiscreteCapability(Guid.Parse(cId)).NotContains("random value").CAST<Resource, FunctionResource>() },
                });

            capabilityFilterSections.Add(capabilityExistenceFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyExistenceFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyExistenceFilterSection = new StringFilterSection<FunctionResource>(
                "Property",
                new Dictionary<Comparers, Func<string, FilterElement<FunctionResource>>> 
                {
                    {Comparers.Exists, (pName) => ResourceExposers.Properties.DictStringField(pName).NotEqual("random value that will never be used as a property value").CAST<Resource, FunctionResource>() }
                });

            propertyFilterSections.Add(propertyExistenceFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyFilterSection = new StringStringFilterSection<FunctionResource>(
                "Property", 
                new Dictionary<Comparers, Func<string, string, FilterElement<FunctionResource>>>
                {
                    {Comparers.Equals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).Equal(propertyValue).CAST<Resource, FunctionResource>() },
                    {Comparers.NotEquals, (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue).CAST<Resource, FunctionResource>() },
                });

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
            AddSection(functionGuidFilterSection, new SectionLayout(++row, 0));

            AddSection(resourcePoolFilterSection, new SectionLayout(++row, 0));

            AddSection(resourceNameFilterSection, new SectionLayout(++row, 0));

            AddSection(resourceIdFilterSection, new SectionLayout(++row, 0));

            AddSection(dmaIdFilterSection, new SectionLayout(++row, 0));

            AddSection(elementIdFilterSection, new SectionLayout(++row, 0));

            foreach (var propertyFilterSection in propertyFilterSections)
            {
                AddSection(propertyFilterSection, new SectionLayout(++row, 0));
            }

            foreach (var capabilityFilterSection in capabilityFilterSections)
            {
                AddSection(capabilityFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addPropertyFilterButton, ++row, 0, 1, ColumnCount);
            AddWidget(addPropertyExistenceFilterButton, ++row, 0, 1 , ColumnCount);
            AddWidget(addCapabilityContainsFilterButton, ++row, 0, 1, ColumnCount);
            AddWidget(addCapabilityExistenceFilterButton, ++row, 0, 1, ColumnCount);

			firstAvailableColumn = ColumnCount + 1;
		}
    }
}
