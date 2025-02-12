namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.DomInstances
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class DomInstanceFiltersSection : SectionContainingDataMinerObjectFilters<DomInstance>
	{
		private readonly MultipleFiltersSection<DomInstance> idFilterSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.Id.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.Id.NotEqual(x)},
			}));

		private readonly MultipleFiltersSection<DomInstance> nameFilterSection = new MultipleFiltersSection<DomInstance>(new StringFilterSection<DomInstance>(
			"Name",
			new Dictionary<Comparers, Func<string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.Name.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.Name.NotEqual(x)},
				{Comparers.Contains, x => DomInstanceExposers.Name.Contains(x)},
				{Comparers.NotContains, x => DomInstanceExposers.Name.NotContains(x)},
			}));

		private readonly MultipleFiltersSection<DomInstance> domDefinitionIdFilterSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"DOM Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.DomDefinitionId.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.DomDefinitionId.NotEqual(x) },
			}));

		private MultipleFiltersSection<DomInstance> selectableDomDefinitionFiltersection;

		private readonly MultipleFiltersSection<DomInstance> statusIdFilterSection = new MultipleFiltersSection<DomInstance>(new StringFilterSection<DomInstance>(
			"Status ID",
			new Dictionary<Comparers, Func<string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.StatusId.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.StatusId.NotEqual(x) },
				{Comparers.Contains, x => DomInstanceExposers.StatusId.Contains(x) },
				{Comparers.NotContains, x => DomInstanceExposers.StatusId.NotContains(x) },
			}));

		private readonly MultipleFiltersSection<DomInstance> sectionDefinitionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"Section Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId) },
				{Comparers.IsNotUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId) },
			}));

		private MultipleFiltersSection<DomInstance> selectableSectionDefinitionIdFiltersSection;

		private readonly MultipleFiltersSection<DomInstance> sectionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"Section ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (sectionId) => DomInstanceExposers.SectionIds.Contains(sectionId) },
				{Comparers.IsNotUsed, (sectionId) => DomInstanceExposers.SectionIds.NotContains(sectionId) }
			}));

		private readonly MultipleFiltersSection<DomInstance> idStringFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidStringFilterSection<DomInstance>(
			"String Field",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
				{Comparers.Contains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Contains(fieldValue) },
				{Comparers.NotContains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotContains(fieldValue) },
			}, "Field ID", "Value"));

		private MultipleFiltersSection<DomInstance> selectableStringFieldValueFiltersSection;

		private readonly MultipleFiltersSection<DomInstance> idIntegerFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidIntegerFilterSection<DomInstance>(
			"Integer Field",
			new Dictionary<Comparers, Func<Guid, int, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
			}, "Field ID"));
		private readonly DomHelper domHelper;
		private MultipleFiltersSection<DomInstance> selectableIntegerFieldValueFiltersSection;

		public DomInstanceFiltersSection(DomHelper domHelper) : base()
		{
			this.domHelper = domHelper ?? throw new ArgumentNullException(nameof(domHelper));

			InitializeSelectableFilters();

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<DomInstance> Clone()
		{
			return new DomInstanceFiltersSection(domHelper);
		}

		private void InitializeSelectableFilters()
		{
			InitializeSelectableDomDefinitionFilterSection();

			var allSectionDefinitions = domHelper.SectionDefinitions.ReadAll();
			var fieldDescriptorsPerSectionDefinition = allSectionDefinitions.ToDictionary(sd => sd, sd => sd.GetAllFieldDescriptors());

			InitializeSelectableSectionDefinitionFilterSection(allSectionDefinitions);

			InitializeSelectableStringFieldDescriptorFilterSection(fieldDescriptorsPerSectionDefinition);

			InitializeSelectableIntegerFieldDescriptorFilterSection(fieldDescriptorsPerSectionDefinition);
		}

		private void InitializeSelectableStringFieldDescriptorFilterSection(Dictionary<SectionDefinition, IReadOnlyList<FieldDescriptor>> fieldDescriptorsPerSectionDefinition)
		{
			var dropDownOptions = new List<DropDownOption<Guid>>();

			foreach (var fieldDescriptorCollection in fieldDescriptorsPerSectionDefinition)
			{
				foreach (var fieldDescriptor in fieldDescriptorCollection.Value)
				{
					string displayValue = $"{fieldDescriptorCollection.Key.GetName()}.{fieldDescriptor.Name}";
					Guid internalValue = fieldDescriptor.ID.Id;

					var existingDropDownOption = dropDownOptions.SingleOrDefault(ddo => ddo.DisplayValue == displayValue);
					if (existingDropDownOption != null)
					{
						existingDropDownOption.DisplayValue += $" ({existingDropDownOption.InternalValue})";
						displayValue += $" ({internalValue})";
					}

					dropDownOptions.Add(new DropDownOption<Guid>(displayValue, internalValue));
				}
			}

			selectableStringFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidStringFilterSection<DomInstance>(
			"String Field",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
				{Comparers.Contains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Contains(fieldValue) },
				{Comparers.NotContains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotContains(fieldValue) },
			},
			dropDownOptions,
			"Value",
			"Dropdown is populated with [Section Definition Name].[Field Descriptor Name]"));

			selectableStringFieldValueFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
		}

		private void InitializeSelectableIntegerFieldDescriptorFilterSection(Dictionary<SectionDefinition, IReadOnlyList<FieldDescriptor>> fieldDescriptorsPerSectionDefinition)
		{
			var dropDownOptions = new List<DropDownOption<Guid>>();

			foreach (var fieldDescriptorCollection in fieldDescriptorsPerSectionDefinition)
			{
				foreach (var fieldDescriptor in fieldDescriptorCollection.Value)
				{
					string displayValue = $"{fieldDescriptorCollection.Key.GetName()}.{fieldDescriptor.Name}";
					Guid internalValue = fieldDescriptor.ID.Id;

					var existingDropDownOption = dropDownOptions.SingleOrDefault(ddo => ddo.DisplayValue == displayValue);
					if (existingDropDownOption != null)
					{
						existingDropDownOption.DisplayValue += $" ({existingDropDownOption.InternalValue})";
						displayValue += $" ({internalValue})";
					}

					dropDownOptions.Add(new DropDownOption<Guid>(displayValue, internalValue));
				}
			}

			selectableIntegerFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidIntegerFilterSection<DomInstance>(
				"Integer Field",
				new Dictionary<Comparers, Func<Guid, int, FilterElement<DomInstance>>>
				{
					{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
					{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
				},
				dropDownOptions,
				"Dropdown is populated with [Section Definition Name].[Field Descriptor Name]"));

			selectableIntegerFieldValueFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
		}

		private void InitializeSelectableSectionDefinitionFilterSection(List<SectionDefinition> allSectionDefinitions)
		{
			var dropDownOptions = new List<DropDownOption<Guid>>();

			foreach (var sectionDefinition in allSectionDefinitions)
			{
				string displayValue = sectionDefinition.GetName();
				Guid internalValue = sectionDefinition.GetID().Id;

				var existingDropDownOption = dropDownOptions.SingleOrDefault(ddo => ddo.DisplayValue == displayValue);
				if (existingDropDownOption != null)
				{
					existingDropDownOption.DisplayValue += $" ({existingDropDownOption.InternalValue})";
					displayValue += $" ({internalValue})";
				}

				dropDownOptions.Add(new DropDownOption<Guid>(displayValue, internalValue));
			}

			selectableSectionDefinitionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidFilterSection<DomInstance>(
				"Section Definition",
				new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
				{
					{Comparers.IsUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId) },
					{Comparers.IsNotUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId) },
				},
				dropDownOptions));

			selectableSectionDefinitionIdFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
		}

		private void InitializeSelectableDomDefinitionFilterSection()
		{
			var allDomDefinitions = domHelper.DomDefinitions.ReadAll();

			var dropDownOptions = new List<DropDownOption<Guid>>();

			foreach (var domDefinition in allDomDefinitions)
			{
				string displayValue = domDefinition.Name;
				Guid internalValue = domDefinition.ID.Id;

				var existingDropDownOption = dropDownOptions.SingleOrDefault(ddo => ddo.DisplayValue == displayValue);
				if (existingDropDownOption != null)
				{
					existingDropDownOption.DisplayValue += $" ({existingDropDownOption.InternalValue})";
					displayValue += $" ({internalValue})";
				}

				dropDownOptions.Add(new DropDownOption<Guid>(displayValue, internalValue));
			}

			selectableDomDefinitionFiltersection = new MultipleFiltersSection<DomInstance>(new SelectableGuidFilterSection<DomInstance>(
			"DOM Definition",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (domDefinitionId) => DomInstanceExposers.DomDefinitionId.Equal(domDefinitionId) },
				{Comparers.IsNotUsed, (domDefinitionId) => DomInstanceExposers.DomDefinitionId.NotEqual(domDefinitionId) },
			},
			dropDownOptions));

			selectableDomDefinitionFiltersection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
		}

		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(idFilterSection, new SectionLayout(++row, 0));
			row += idFilterSection.RowCount;

			AddSection(nameFilterSection, new SectionLayout(row, 0));
			row += nameFilterSection.RowCount;

			AddSection(domDefinitionIdFilterSection, new SectionLayout(row, 0));
			row += domDefinitionIdFilterSection.RowCount;

			AddSection(selectableDomDefinitionFiltersection, new SectionLayout(row, 0));
			row += selectableDomDefinitionFiltersection.RowCount;

			AddSection(statusIdFilterSection, new SectionLayout(row, 0));
			row += statusIdFilterSection.RowCount;

			AddSection(sectionDefinitionIdFiltersSection, new SectionLayout(row, 0));
			row += sectionDefinitionIdFiltersSection.RowCount;

			AddSection(selectableSectionDefinitionIdFiltersSection, new SectionLayout(row, 0));
			row += selectableSectionDefinitionIdFiltersSection.RowCount;

			AddSection(sectionIdFiltersSection, new SectionLayout(row, 0));
			row += sectionIdFiltersSection.RowCount;

			AddSection(idStringFieldValueFiltersSection, new SectionLayout(row, 0));
			row += idStringFieldValueFiltersSection.RowCount;

			AddSection(selectableStringFieldValueFiltersSection, new SectionLayout(row, 0));
			row += selectableStringFieldValueFiltersSection.RowCount;

			AddSection(idIntegerFieldValueFiltersSection, new SectionLayout(row, 0));
			row += idIntegerFieldValueFiltersSection.RowCount;

			AddSection(selectableIntegerFieldValueFiltersSection, new SectionLayout(row, 0));
			row += selectableIntegerFieldValueFiltersSection.RowCount;
		}
	}
}
