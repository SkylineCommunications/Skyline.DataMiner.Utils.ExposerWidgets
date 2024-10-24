namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering DOM instances.
	/// </summary>
	public class FindDomInstancesWithFiltersSection : FindItemsWithFiltersSection<DomInstance>
	{
		private readonly Label moduleIdLabel = new Label("DOM Module ID:");
		private readonly DropDown moduleIdDropDown;

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

		private readonly MultipleFiltersSection<DomInstance> idFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidStringFilterSection<DomInstance>(
			"Field",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
				{Comparers.Contains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Contains(fieldValue) },
				{Comparers.NotContains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotContains(fieldValue) },
			}, "Field ID", "Value"));

		private MultipleFiltersSection<DomInstance> selectableFieldValueFiltersSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDomInstancesWithFiltersSection"/>"/> class.
        /// </summary>
        public FindDomInstancesWithFiltersSection()
		{
			var moduleSettingsHelper = new ModuleSettingsHelper(Engine.SLNet.SendMessages);
			var allModuleIds = moduleSettingsHelper.ModuleSettings.ReadAll().Select(x => x.ModuleId).OrderBy(id => id).ToList();

			if (!allModuleIds.Any())
			{
				ItemTypeIsSupportedOnThisSystem = false;
				return;
			}

			moduleIdDropDown = new DropDown(allModuleIds, allModuleIds.FirstOrDefault() ?? throw new InvalidOperationException("No DOM modules defined on this system")) { IsDisplayFilterShown = true };
			DomHelper = new DomHelper(Engine.SLNet.SendMessages, moduleIdDropDown.Selected);

			moduleIdDropDown.Changed += ModuleIdDropDown_Changed;
			
			InitializeSelectableFilters();

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		/// <summary>
		/// Gets the DomHelper for the current DOM Module ID.
		/// </summary>
		public DomHelper DomHelper { get; private set; }

		private void ModuleIdDropDown_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
			if (!string.IsNullOrWhiteSpace(e.Selected)) 
			{
				DomHelper = new DomHelper(Engine.SLNet.SendMessages, e.Selected);

				InitializeSelectableFilters();
			}

			InvokeRegenerateUi();
        }

		private void InitializeSelectableFilters()
		{
			InitializeSelectableDomDefinitionFilterSection();

			var allSectionDefinitions = DomHelper.SectionDefinitions.ReadAll();
			var fieldDescriptorsPerSectionDefinition = allSectionDefinitions.ToDictionary(sd => sd, sd => sd.GetAllFieldDescriptors());

			InitializeSelectableSectionDefinitionFilterSection(allSectionDefinitions);

			InitializeSelectableFieldDescriptorFilterSection(fieldDescriptorsPerSectionDefinition);
		}

		private void InitializeSelectableFieldDescriptorFilterSection(Dictionary<SectionDefinition, IReadOnlyList<FieldDescriptor>> fieldDescriptorsPerSectionDefinition)
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

			selectableFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidStringFilterSection<DomInstance>(
			"Field",
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

			selectableFieldValueFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
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
			var allDomDefinitions = DomHelper.DomDefinitions.ReadAll();

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

		/// <summary>
		/// Adding filter sections on a row specified.
		/// </summary>
		/// <param name="row">Row position where new section should appear.</param>
		protected override void AddFilterSections(ref int row)
        {
            AddWidget(moduleIdLabel, ++row, 1);
            AddWidget(moduleIdDropDown, row, 4, 1, 2);

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

			AddSection(idFieldValueFiltersSection, new SectionLayout(row, 0));
			row += idFieldValueFiltersSection.RowCount;

			AddSection(selectableFieldValueFiltersSection, new SectionLayout(row, 0));
			row += selectableFieldValueFiltersSection.RowCount;
		}

        /// <summary>
        /// Retrieving all items in the system based on input values.
        /// </summary>
        /// <returns>Collection of dom instances.</returns>
        protected override IEnumerable<DomInstance> FindItemsWithFilters()
        {
            if (DomHelper == null) 
            {
				moduleIdDropDown.ValidationState = UIValidationState.Invalid;
				moduleIdDropDown.ValidationText = "Provide a valid DOM Module ID";
                return new List<DomInstance>();
            }
			else
			{
				moduleIdDropDown.ValidationState = UIValidationState.Valid;
			}

			return DomHelper.DomInstances.Read(GetCombinedFilterElement());
        }

        /// <summary>
        /// Retrieves name of dom instance.
        /// </summary>
        /// <returns>Name of dom instance.</returns>
        protected override string IdentifyItem(DomInstance item)
        {
            return $"{item.Name} [{item.ID.Id}]";
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isVisible"></param>
		protected override void SetWidgetsVisibility(bool isVisible)
		{
			base.SetWidgetsVisibility(isVisible);

			moduleIdLabel.IsVisible = isVisible;
			moduleIdDropDown.IsVisible = isVisible;
		}
	}
}
