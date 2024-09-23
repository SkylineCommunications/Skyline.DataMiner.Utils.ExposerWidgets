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

		private readonly MultipleFiltersSection<DomInstance> definitionIdFilterSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"DOM Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.DomDefinitionId.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.DomDefinitionId.NotEqual(x) },
			}));

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

		private DomHelper domHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDomInstancesWithFiltersSection"/>"/> class.
        /// </summary>
        public FindDomInstancesWithFiltersSection() : base()
		{
			var moduleSettingsHelper = new ModuleSettingsHelper(Engine.SLNet.SendMessages);
			var allModuleIds = moduleSettingsHelper.ModuleSettings.ReadAll().Select(x => x.ModuleId).OrderBy(id => id).ToList();

			moduleIdDropDown = new DropDown(allModuleIds, allModuleIds.FirstOrDefault() ?? throw new InvalidOperationException("No DOM modules defined on this system")) { IsDisplayFilterShown = true };
			domHelper = new DomHelper(Engine.SLNet.SendMessages, moduleIdDropDown.Selected);

			moduleIdDropDown.Changed += ModuleIdDropDown_Changed;
			
			InitializeSelectableFilters();

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		/// <summary>
		/// Exposes the DOM module ID entered by the user.
		/// </summary>
		public string DomModuleId => moduleIdDropDown.Selected;

		/// <summary>
		/// Event invoked when the DOM Module ID was changed by the user.
		/// </summary>
		public event EventHandler DomModuleIdChanged;

		private void ModuleIdDropDown_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
			if (!string.IsNullOrWhiteSpace(e.Selected)) 
			{
				domHelper = new DomHelper(Engine.SLNet.SendMessages, e.Selected);

				InitializeSelectableFilters();
			}

			DomModuleIdChanged?.Invoke(this, e);
			InvokeRegenerateUi();
        }

		private void InitializeSelectableFilters()
		{
			var allSectionDefinitions = domHelper.SectionDefinitions.ReadAll();
			
			selectableSectionDefinitionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidFilterSection<DomInstance>(
			"Section Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId) },
				{Comparers.IsNotUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId) },
			},
			allSectionDefinitions.Select(sd => new DropDownOption<Guid>(sd.GetName(), sd.GetID().Id))));

			selectableSectionDefinitionIdFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();

			var fieldDescriptorsPerSectionDefinition = allSectionDefinitions.ToDictionary(sd => sd, sd => sd.GetAllFieldDescriptors());

			selectableFieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new SelectableGuidStringFilterSection<DomInstance>(
			"Field",
			new Dictionary<Comparers, Func<Guid, string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotEqual(fieldValue) },
				{Comparers.Contains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).Contains(fieldValue) },
				{Comparers.NotContains, (fieldId, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(fieldId)).NotContains(fieldValue) },
			}, 
			fieldDescriptorsPerSectionDefinition.SelectMany(x => x.Value.Select(fd => new DropDownOption<Guid>($"{x.Key.GetName()}.{fd.Name}", fd.ID.Id))), 
			"Value", 
			"Dropdown is populated with [Section Definition Name].[Field Descriptor Name]"));

			selectableFieldValueFiltersSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
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

			AddSection(definitionIdFilterSection, new SectionLayout(row, 0));
			row += definitionIdFilterSection.RowCount;

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
            if (domHelper == null) 
            {
				moduleIdDropDown.ValidationState = UIValidationState.Invalid;
				moduleIdDropDown.ValidationText = "Provide a valid DOM Module ID";
                return new List<DomInstance>();
            }
			else
			{
				moduleIdDropDown.ValidationState = UIValidationState.Valid;
			}

			return domHelper.DomInstances.Read(GetCombinedFilterElement());
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
