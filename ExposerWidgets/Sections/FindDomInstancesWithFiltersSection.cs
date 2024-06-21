namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
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
		private readonly Label moduleId = new Label("DOM Module ID:");
		private readonly TextBox moduleIdTextBox = new TextBox(string.Empty) { PlaceHolder = nameof(Guid)};

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

		private readonly MultipleFiltersSection<DomInstance> createdAtFilterSection = new MultipleFiltersSection<DomInstance>(new DateTimeFilterSection<DomInstance>(
			"Created At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<DomInstance>>>
			{
				{Comparers.GreaterThan, x => DomInstanceExposers.CreatedAt.GreaterThan(x) },
				{Comparers.LessThan, x => DomInstanceExposers.CreatedAt.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<DomInstance> lastModifiedAtFilterSection = new MultipleFiltersSection<DomInstance>(new DateTimeFilterSection<DomInstance>(
			"Last Modified At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<DomInstance>>>
			{
				{Comparers.GreaterThan, x => DomInstanceExposers.LastModified.GreaterThan(x) },
				{Comparers.LessThan, x => DomInstanceExposers.LastModified.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<DomInstance> sectionDefinitionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"Section Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId) },
				{Comparers.IsNotUsed, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId) },
			}));

		private readonly MultipleFiltersSection<DomInstance> sectionIdFiltersSection = new MultipleFiltersSection<DomInstance>(new GuidFilterSection<DomInstance>(
			"Section ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.IsUsed, (sectionId) => DomInstanceExposers.SectionIds.Contains(sectionId) },
				{Comparers.IsNotUsed, (sectionId) => DomInstanceExposers.SectionIds.NotContains(sectionId) }
			}));

		private readonly MultipleFiltersSection<DomInstance> fieldValueFiltersSection = new MultipleFiltersSection<DomInstance>(new StringStringFilterSection<DomInstance>(
			"Field",
			new Dictionary<Comparers, Func<string, string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).Equal(fieldValue) },
				{Comparers.NotEquals, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).NotEqual(fieldValue) },
				{Comparers.Contains, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).Contains(fieldValue) },
				{Comparers.NotContains, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).NotContains(fieldValue) },
			},"Name", "Value"));

		private DomHelper domHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDomInstancesWithFiltersSection"/>"/> class.
        /// </summary>
        public FindDomInstancesWithFiltersSection() : base()
        {
            moduleIdTextBox.FocusLost += ModuleIdTextBox_FocusLost;

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		/// <summary>
		/// 
		/// </summary>
		public string DomModuleId => moduleIdTextBox.Text;

		private void ModuleIdTextBox_FocusLost(object sender, TextBox.TextBoxFocusLostEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Value)) domHelper = new DomHelper(Engine.SLNet.SendMessages, e.Value);
        }

		/// <summary>
		/// Adding filter sections on a row specified.
		/// </summary>
		/// <param name="row">Row position where new section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddWidget(moduleId, ++row, 1);
            AddWidget(moduleIdTextBox, row, 3);

            AddSection(idFilterSection, new SectionLayout(++row, 0));
			row += idFilterSection.RowCount;

            AddSection(nameFilterSection, new SectionLayout(row, 0));
			row += nameFilterSection.RowCount;

			AddSection(definitionIdFilterSection, new SectionLayout(row, 0));
			row += definitionIdFilterSection.RowCount;

			AddSection(statusIdFilterSection, new SectionLayout(row, 0));
			row += statusIdFilterSection.RowCount;

			AddSection(createdAtFilterSection, new SectionLayout(row, 0));
			row += createdAtFilterSection.RowCount;

			AddSection(lastModifiedAtFilterSection, new SectionLayout(row, 0));
			row += lastModifiedAtFilterSection.RowCount;

			AddSection(sectionDefinitionIdFiltersSection, new SectionLayout(row, 0));
			row += sectionDefinitionIdFiltersSection.RowCount;

			AddSection(sectionIdFiltersSection, new SectionLayout(row, 0));
			row += sectionIdFiltersSection.RowCount;

			AddSection(fieldValueFiltersSection, new SectionLayout(row, 0));
			row += fieldValueFiltersSection.RowCount;

			firstAvailableColumn = ColumnCount + 1;
		}

        /// <summary>
        /// Retrieving all items in the system based on input values.
        /// </summary>
        /// <returns>Collection of dom instances.</returns>
        protected override IEnumerable<DomInstance> FindItemsWithFilters()
        {
            if (domHelper == null) 
            {
				moduleIdTextBox.ValidationState = UIValidationState.Invalid;
				moduleIdTextBox.ValidationText = "Provide a valid DOM Module ID";
                return new List<DomInstance>();
            }
			else
			{
				moduleIdTextBox.ValidationState = UIValidationState.Valid;
			}

			return domHelper.DomInstances.Read(GetCombinedFilterElement());
        }

        /// <summary>
        /// Retrieves name of dom instance.
        /// </summary>
        /// <returns>Name of dom instance.</returns>
        protected override string GetItemIdentifier(DomInstance item)
        {
            return item.Name;
        }
	}
}
