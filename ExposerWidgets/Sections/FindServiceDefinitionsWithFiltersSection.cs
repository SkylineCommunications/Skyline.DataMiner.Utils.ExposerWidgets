namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.ServiceManager.Objects;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Core.SRM;

    public class FindServiceDefinitionsWithFiltersSection : FindItemsWithFiltersSection<ServiceDefinition>
    {
        private readonly StringFilterSection<ServiceDefinition> nameFilterSection = new StringFilterSection<ServiceDefinition>("Name", name => ServiceDefinitionExposers.Name.Equal(name));

        private readonly StringFilterSection<ServiceDefinition> nameContainsFilterSection = new StringFilterSection<ServiceDefinition>("Name Contains", name => ServiceDefinitionExposers.Name.Contains(name));

        private readonly GuidFilterSection<ServiceDefinition> idFilterSection = new GuidFilterSection<ServiceDefinition>("ID", id => ServiceDefinitionExposers.ID.Equal(id));

        private readonly BooleanFilterSection<ServiceDefinition> isTemplateFilterSection = new BooleanFilterSection<ServiceDefinition>("Is Template", boolean => ServiceDefinitionExposers.IsTemplate.Equal(boolean));

        private readonly List<FilterSectionBase<ServiceDefinition>> nodeFunctionIdFilterSections = new List<FilterSectionBase<ServiceDefinition>>();

        private readonly Button addNodeFunctionIdFilterButton = new Button("Add Node Function ID Filter");

        private readonly List<FilterSectionBase<ServiceDefinition>> propertyFilterSections = new List<FilterSectionBase<ServiceDefinition>>();

        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");

        public FindServiceDefinitionsWithFiltersSection() : base()
        {
            addNodeFunctionIdFilterButton.Pressed += AddNodeFunctionIdFilterButton_Pressed;

            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;
        }

        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }

            propertyFilterSections.Clear();
        }

        private void AddNodeFunctionIdFilterButton_Pressed(object sender, EventArgs e)
        {
            var nodeFunctionIdFilterSection = new GuidFilterSection<ServiceDefinition>("Uses Function ID", guid => ServiceDefinitionExposers.NodeFunctionIDs.Contains(guid));

            nodeFunctionIdFilterSections.Add(nodeFunctionIdFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyFilterSection = new StringStringFilterSection<ServiceDefinition>("Property", (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).Equal(propertyValue));

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        protected override IEnumerable<ServiceDefinition> FindItemsWithFilters()
        {
            if (!TryGetCombinedFilterElement(out var combinedFilter))
            {
                combinedFilter = new ANDFilterElement<ServiceDefinition>(ServiceDefinitionExposers.Name.NotEqual(string.Empty));
            }

            return SrmManagers.ServiceManager.GetServiceDefinitions(combinedFilter);
        }

        protected override string GetItemIdentifier(ServiceDefinition item)
        {
            return item.Name;
        }

        protected override void AddFilterSections(ref int row)
        {
            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(isTemplateFilterSection, new SectionLayout(++row, 0));

            foreach (var nodeFunctionIdFilterSection in nodeFunctionIdFilterSections)
            {
                AddSection(nodeFunctionIdFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addNodeFunctionIdFilterButton, ++row, 0);

            foreach (var propertyFilterSection in propertyFilterSections)
            {
                AddSection(propertyFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addPropertyFilterButton, ++row, 0);
        }
    }
}
