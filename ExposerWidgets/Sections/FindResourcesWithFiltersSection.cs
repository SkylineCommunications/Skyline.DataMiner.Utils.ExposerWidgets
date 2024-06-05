﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Core.SRM;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.ResourceManager.Objects;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;

    public class FindResourcesWithFiltersSection : FindItemsWithFiltersSection<FunctionResource>
    {
        private readonly FilterSectionBase<FunctionResource> functionGuidFilterSection = new GuidFilterSection<FunctionResource>("Function Guid", x => FunctionResourceExposers.FunctionGUID.Equal((Guid)x).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> resourcePoolFilterSection = new ResourcePoolFilterSection("Resource Pool", x => ResourceExposers.PoolGUIDs.Contains((Guid)x).CAST<Resource, FunctionResource>(), SrmManagers.ResourceManager.GetResourcePools());

        private readonly FilterSectionBase<FunctionResource> resourceNameEqualsFilterSection = new StringFilterSection<FunctionResource>("Resource Name Equals", x => ResourceExposers.Name.Equal((string)x).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> resourceNameContainsFilterSection = new StringFilterSection<FunctionResource>("Resource Name Contains", x => ResourceExposers.Name.Contains((string)x).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> resourceNameDoesNotContainFilterSection = new StringFilterSection<FunctionResource>("Resource Name Does Not Contain", x => ResourceExposers.Name.NotContains((string)x).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> resourceIdFilterSection = new GuidFilterSection<FunctionResource>("Resource ID", x => ResourceExposers.ID.Equal((Guid)x).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> dmaIdFilterSection = new IntegerFilterSection<FunctionResource>("DMA ID", x => ResourceExposers.DmaID.Equal(Convert.ToInt32(x)).CAST<Resource, FunctionResource>());

        private readonly FilterSectionBase<FunctionResource> elementIdFilterSection = new IntegerFilterSection<FunctionResource>("Element ID", x => ResourceExposers.ElementID.Equal(Convert.ToInt32(x)).CAST<Resource, FunctionResource>());

        private readonly List<FilterSectionBase<FunctionResource>> propertyFilterSections = new List<FilterSectionBase<FunctionResource>>();
        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");
        private readonly Button addPropertyExistenceFilterButton = new Button("Add Property Existence Filter");

        private readonly List<FilterSectionBase<FunctionResource>> capabilityFilterSections = new List<FilterSectionBase<FunctionResource>>();
        private readonly Button addCapabilityContainsFilterButton = new Button("Add 'Capability Contains' Filter");
        private readonly Button addCapabilityDoesntContainFilterButton = new Button("Add 'Capability Does Not Contain' Filter");
        private readonly Button addCapabilityExistenceFilterButton = new Button("Add 'Capability Exists' Filter");


        public FindResourcesWithFiltersSection() : base()
        {
            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;

            addPropertyExistenceFilterButton.Pressed += AddPropertyExistenceFilterButton_Pressed;

            addCapabilityContainsFilterButton.Pressed += AddCapabilityContainsFilterButton_Pressed;

            addCapabilityDoesntContainFilterButton.Pressed += AddCapabilityDoesntContainFilterButton_Pressed;

            addCapabilityExistenceFilterButton.Pressed += AddCapabilityExistenceFilterButton_Pressed;
        }

        private void AddCapabilityDoesntContainFilterButton_Pressed(object sender, EventArgs e)
        {
            var capabilityFilterSection = new StringStringFilterSection<FunctionResource>("Discrete Capability Does Not Contain", (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(Guid.Parse(cId)).NotContains(cValue).CAST<Resource, FunctionResource>());

            capabilityFilterSections.Add(capabilityFilterSection);

            InvokeRegenerateUi();
        }

        private void AddCapabilityContainsFilterButton_Pressed(object sender, EventArgs e)
        {
            var capabilityFilterSection = new StringStringFilterSection<FunctionResource>("Discrete Capability Contains", (cId, cValue) => ResourceExposers.Capabilities.DiscreteCapability(Guid.Parse(cId)).Contains(cValue).CAST<Resource, FunctionResource>());

            capabilityFilterSections.Add(capabilityFilterSection);

            InvokeRegenerateUi();
        }

        private void AddCapabilityExistenceFilterButton_Pressed(object sender, EventArgs e)
        {
            var capabilityExistenceFilterSection = new StringFilterSection<FunctionResource>("Discrete Capability Exists", (cId) => ResourceExposers.Capabilities.DiscreteCapability(Guid.Parse((string)cId)).NotContains("random value").CAST<Resource, FunctionResource>());

            capabilityFilterSections.Add(capabilityExistenceFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyExistenceFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyExistenceFilterSection = new StringFilterSection<FunctionResource>("Property Exists", (pName) => ResourceExposers.Properties.DictStringField((string)pName).NotEqual("random value that will never be used as a property value").CAST<Resource, FunctionResource>());

            propertyFilterSections.Add(propertyExistenceFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyFilterSection = new StringStringFilterSection<FunctionResource>("Property", (propertyName, propertyValue) => ResourceExposers.Properties.DictStringField(propertyName).Equal(propertyValue).CAST<Resource, FunctionResource>());

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }
        }

        protected override void HandleVisibilityAndEnabledUpdate(bool isVisible, bool isEnabled)
        {
            // no actions required
        }

        protected override IEnumerable<FunctionResource> FindItemsWithFilters()
        {
            return SrmManagers.ResourceManager.GetResources(this.GetCombinedFilterElement().CAST<FunctionResource, Resource>()).Cast<FunctionResource>().ToList();
        }

        protected override string GetItemIdentifier(FunctionResource item)
        {
            return item.Name;
        }

        protected override void AddFilterSections(ref int row)
        {
            AddSection(functionGuidFilterSection, new SectionLayout(++row, 0));

            AddSection(resourcePoolFilterSection, new SectionLayout(++row, 0));

            AddSection(resourceNameEqualsFilterSection, new SectionLayout(++row, 0));

            AddSection(resourceNameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(resourceNameDoesNotContainFilterSection, new SectionLayout(++row, 0));

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

            AddWidget(addPropertyFilterButton, ++row, 0);
            AddWidget(addPropertyExistenceFilterButton, ++row, 0);

            AddWidget(addCapabilityContainsFilterButton, ++row, 0);
            AddWidget(addCapabilityDoesntContainFilterButton, ++row, 0);
            AddWidget(addCapabilityExistenceFilterButton, ++row, 0);
        }
    }
}