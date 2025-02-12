namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.ServiceDefinitions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ServiceManager.Objects;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Section for filtering service definitions.
	/// </summary>
	public class ServiceDefinitionsFiltersSection : SectionContainingDataMinerObjectFilters<ServiceDefinition>
	{
		private readonly MultipleFiltersSection<ServiceDefinition> nameFilterSection = new MultipleFiltersSection<ServiceDefinition>(new StringFilterSection<ServiceDefinition>(
			"Name",
			new Dictionary<Comparers, Func<string, FilterElement<ServiceDefinition>>>
			{
				{Comparers.Equals, name => ServiceDefinitionExposers.Name.Equal(name) },
				{Comparers.NotEquals, name => ServiceDefinitionExposers.Name.NotEqual(name) },
				{Comparers.Contains, name => ServiceDefinitionExposers.Name.Contains(name) },
				{Comparers.NotContains, name => ServiceDefinitionExposers.Name.NotContains(name) },
			}));

		private readonly MultipleFiltersSection<ServiceDefinition> idFilterSection = new MultipleFiltersSection<ServiceDefinition>(new GuidFilterSection<ServiceDefinition>(
			"ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<ServiceDefinition>>>
			{
				{Comparers.Equals, id => ServiceDefinitionExposers.ID.Equal(id) },
				{Comparers.NotEquals, id => ServiceDefinitionExposers.ID.NotEqual(id) },
			}));

		private readonly MultipleFiltersSection<ServiceDefinition> isTemplateFilterSection = new MultipleFiltersSection<ServiceDefinition>(new BooleanFilterSection<ServiceDefinition>(
			"Is Template",
			new Dictionary<Comparers, Func<bool, FilterElement<ServiceDefinition>>>
			{
				{Comparers.Equals, boolean => ServiceDefinitionExposers.IsTemplate.Equal(boolean) }
			}));

		private readonly MultipleFiltersSection<ServiceDefinition> nodeFunctionIdFilterSection = new MultipleFiltersSection<ServiceDefinition>(new GuidFilterSection<ServiceDefinition>(
			"Function ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<ServiceDefinition>>>
			{
				{Comparers.IsUsed,  guid => ServiceDefinitionExposers.NodeFunctionIDs.Contains(guid) },
				{Comparers.IsNotUsed, guid => ServiceDefinitionExposers.NodeFunctionIDs.NotContains(guid) },
			}));

		private readonly MultipleFiltersSection<ServiceDefinition> propertyFilterSection = new MultipleFiltersSection<ServiceDefinition>(new StringStringFilterSection<ServiceDefinition>(
			"Property",
			new Dictionary<Comparers, Func<string, string, FilterElement<ServiceDefinition>>>
			{
				{Comparers.Equals, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).Equal(propertyValue) },
				{Comparers.NotEquals, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).NotEqual(propertyValue) },
				{Comparers.Contains, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).Contains(propertyValue) },
				{Comparers.NotContains, (propertyName, propertyValue) => ServiceDefinitionExposers.Properties.DictStringField(propertyName).NotContains(propertyValue) },
			}, "Name", "Value"));

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDefinitionsFiltersSection"/>"/> class.
		/// </summary>
		public ServiceDefinitionsFiltersSection()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<ServiceDefinition> Clone()
		{
			return new ServiceDefinitionsFiltersSection();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(nameFilterSection, new SectionLayout(++row, 0));
			row += nameFilterSection.RowCount;

			AddSection(idFilterSection, new SectionLayout(row, 0));
			row += idFilterSection.RowCount;

			AddSection(isTemplateFilterSection, new SectionLayout(row, 0));
			row += isTemplateFilterSection.RowCount;

			AddSection(nodeFunctionIdFilterSection, new SectionLayout(row, 0));
			row += nodeFunctionIdFilterSection.RowCount;

			AddSection(propertyFilterSection, new SectionLayout(row, 0));
			row += propertyFilterSection.RowCount;
		}
	}
}
