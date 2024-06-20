namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ServiceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering service definitions.
	/// </summary>
	public class FindServiceDefinitionsWithFiltersSection : FindItemsWithFiltersSection<ServiceDefinition>
    {
        private readonly StringFilterSection<ServiceDefinition> nameFilterSection = new StringFilterSection<ServiceDefinition>(
            "Name",
            new Dictionary<Comparers, Func<string, FilterElement<ServiceDefinition>>>
            {
                {Comparers.Equals, name => ServiceDefinitionExposers.Name.Equal(name) },
                {Comparers.NotEquals, name => ServiceDefinitionExposers.Name.NotEqual(name) },
                {Comparers.Contains, name => ServiceDefinitionExposers.Name.Contains(name) },
                {Comparers.NotContains, name => ServiceDefinitionExposers.Name.NotContains(name) },
            });

        private readonly GuidFilterSection<ServiceDefinition> idFilterSection = new GuidFilterSection<ServiceDefinition>(
            "ID", 
            new Dictionary<Comparers, Func<Guid, FilterElement<ServiceDefinition>>> 
            {
                {Comparers.Equals, id => ServiceDefinitionExposers.ID.Equal(id) },
                {Comparers.NotEquals, id => ServiceDefinitionExposers.ID.NotEqual(id) },
            });

        private readonly BooleanFilterSection<ServiceDefinition> isTemplateFilterSection = new BooleanFilterSection<ServiceDefinition>(
            "Is Template",
            new Dictionary<Comparers, Func<bool, FilterElement<ServiceDefinition>>>
            {
                {Comparers.Equals, boolean => ServiceDefinitionExposers.IsTemplate.Equal(boolean) }
            });

        private readonly List<FilterSectionBase<ServiceDefinition>> nodeFunctionIdFilterSections = new List<FilterSectionBase<ServiceDefinition>>();

        private readonly Button addNodeFunctionIdFilterButton = new Button("Add Node Function ID Filter");

        private readonly List<FilterSectionBase<ServiceDefinition>> propertyFilterSections = new List<FilterSectionBase<ServiceDefinition>>();

        private readonly Button addPropertyFilterButton = new Button("Add Property Filter");

        private readonly ServiceManagerHelper serviceManagerHelper = new ServiceManagerHelper();

		/// <summary>
		/// Initializes a new instance of the <see cref="FindServiceDefinitionsWithFiltersSection"/>"/> class.
		/// </summary>
		public FindServiceDefinitionsWithFiltersSection() : base()
        {
            serviceManagerHelper.RequestResponseEvent += (s, e) => e.responseMessage = Engine.SLNet.SendSingleResponseMessage(e.requestMessage);

			addNodeFunctionIdFilterButton.Pressed += AddNodeFunctionIdFilterButton_Pressed;

            addPropertyFilterButton.Pressed += AddPropertyFilterButton_Pressed;
			GenerateUi();
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

            propertyFilterSections.Clear();
        }

        private void AddNodeFunctionIdFilterButton_Pressed(object sender, EventArgs e)
        {
            var nodeFunctionIdFilterSection = new GuidFilterSection<ServiceDefinition>(
                "Function ID",
                new Dictionary<Comparers, Func<Guid, FilterElement<ServiceDefinition>>> 
                {
                    {Comparers.Exists,  guid => ServiceDefinitionExposers.NodeFunctionIDs.Contains(guid) },
                    {Comparers.NotExists, guid => ServiceDefinitionExposers.NodeFunctionIDs.NotContains(guid) },
                });

            nodeFunctionIdFilterSections.Add(nodeFunctionIdFilterSection);

            InvokeRegenerateUi();
        }

        private void AddPropertyFilterButton_Pressed(object sender, EventArgs e)
        {
            var propertyFilterSection = new StringStringFilterSection<ServiceDefinition>(
                "Property",
                new Dictionary<Comparers, Func<string, string, FilterElement<ServiceDefinition>>>
                {
                    {Comparers.Equals, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).Equal(propertyValue) },
                    {Comparers.NotEquals, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue) },
                    {Comparers.Contains, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).Contains(propertyValue) },
                    {Comparers.NotContains, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).NotContains(propertyValue) },
                });

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        /// <summary>
        /// Filtering all service definitions in system based on provided input.
        /// </summary>
        /// <returns>Collection of filtered service definitions.</returns>
        protected override IEnumerable<ServiceDefinition> FindItemsWithFilters()
        {
            if (!TryGetCombinedFilterElement(out var combinedFilter))
            {
                combinedFilter = new ANDFilterElement<ServiceDefinition>(ServiceDefinitionExposers.Name.NotEqual(string.Empty));
            }

            return serviceManagerHelper.GetServiceDefinitions(combinedFilter);
        }

        /// <summary>
        /// Gets name of service definition.
        /// </summary>
        /// <param name="item">Service definition for which we want to retrieve name.</param>
        /// <returns>Name of service definition.</returns>
        protected override string GetItemIdentifier(ServiceDefinition item)
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
            AddSection(nameFilterSection, new SectionLayout(++row, 0));

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

			firstAvailableColumn = ColumnCount + 1;
		}
    }
}
