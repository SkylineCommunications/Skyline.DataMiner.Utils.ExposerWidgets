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
		private readonly TextBox moduleIdTextBox = new TextBox(string.Empty);

		private readonly FilterSectionBase<DomInstance> idFilterSection = new GuidFilterSection<DomInstance>(
			"DOM Instance ID", 
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.Id.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.Id.NotEqual(x)}, 
			}
		);

        private readonly FilterSectionBase<DomInstance> nameFilterSection = new StringFilterSection<DomInstance>(
			"DOM Instance Name", 
			new Dictionary<Comparers, Func<string, FilterElement<DomInstance>>> 
			{
				{Comparers.Equals, x => DomInstanceExposers.Name.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.Name.NotEqual(x)},
				{Comparers.Contains, x => DomInstanceExposers.Name.Contains(x)},
				{Comparers.NotContains, x => DomInstanceExposers.Name.NotContains(x)},
			}
		);

		private readonly FilterSectionBase<DomInstance> definitionIdFilterSection = new GuidFilterSection<DomInstance>(
			"DOM Definition ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.DomDefinitionId.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.DomDefinitionId.NotEqual(x) },
			}
		);

		private readonly FilterSectionBase<DomInstance> statusIdFilterSection = new StringFilterSection<DomInstance>(
			"DOM Status ID",
			new Dictionary<Comparers, Func<string, FilterElement<DomInstance>>>
			{
				{Comparers.Equals, x => DomInstanceExposers.StatusId.Equal(x) },
				{Comparers.NotEquals, x => DomInstanceExposers.StatusId.NotEqual(x) },
			}
		);

		private readonly FilterSectionBase<DomInstance> createdAtFilterSection = new DateTimeFilterSection<DomInstance>(
			"DOM Instance Created At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<DomInstance>>>
			{
				{Comparers.GreaterThan, x => DomInstanceExposers.CreatedAt.GreaterThan(x) },
				{Comparers.LessThan, x => DomInstanceExposers.CreatedAt.LessThan(x) },
			}
		);

		private readonly FilterSectionBase<DomInstance> lastModifiedAtFilterSection = new DateTimeFilterSection<DomInstance>(
			"DOM Instance Last Modified At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<DomInstance>>>
			{
				{Comparers.GreaterThan, x => DomInstanceExposers.LastModified.GreaterThan(x) },
				{Comparers.LessThan, x => DomInstanceExposers.LastModified.LessThan(x) },
			}
		);

		private readonly List<FilterSectionBase<DomInstance>> sectionDefinitionIdFilterSections = new List<FilterSectionBase<DomInstance>>();
		private readonly Button addSectionDefinitionIdFilterButton = new Button("Add Section Definition Filter");

		private readonly List<FilterSectionBase<DomInstance>> sectionIdFilterSections = new List<FilterSectionBase<DomInstance>>();
		private readonly Button addSectionIdFilterButton = new Button("Add Section Filter");

		private readonly List<FilterSectionBase<DomInstance>> fieldValueFilterSections = new List<FilterSectionBase<DomInstance>>();
		private readonly Button addFieldValueFilterButton = new Button("Add Field Filter");

		private DomHelper domHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDomInstancesWithFiltersSection"/>"/> class.
        /// </summary>
        public FindDomInstancesWithFiltersSection() : base()
        {
            moduleIdTextBox.FocusLost += ModuleIdTextBox_FocusLost;

			addSectionDefinitionIdFilterButton.Pressed += AddSectionDefinitionIdFilterButton_Pressed;

			addSectionIdFilterButton.Pressed += AddSectionIdFilterButton_Pressed;

			addFieldValueFilterButton.Pressed += AddFieldValueFilterButton_Pressed;

			GenerateUi();
		}

		/// <summary>
		/// 
		/// </summary>
		public string DomModuleId => moduleIdTextBox.Text;

		private void AddFieldValueFilterButton_Pressed(object sender, EventArgs e)
		{
			var fieldValueFilterSection = new StringStringFilterSection<DomInstance>(
				"Field",
				new Dictionary<Comparers, Func<string, string, FilterElement<DomInstance>>> 
				{
					{Comparers.Equals, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).Equal(fieldValue) },
					{Comparers.NotEquals, (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).NotEqual(fieldValue) },
				}
			);

			fieldValueFilterSections.Add(fieldValueFilterSection);
			
			InvokeRegenerateUi();
		}

		private void AddSectionIdFilterButton_Pressed(object sender, EventArgs e)
		{
			var sectionIdFilterSection = new GuidFilterSection<DomInstance>(
				"Section ID",
				new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>> 
				{
					{Comparers.Exists, (sectionId) => DomInstanceExposers.SectionIds.Contains(sectionId) },
					{Comparers.NotExists, (sectionId) => DomInstanceExposers.SectionIds.NotContains(sectionId) }
				}
			);

			sectionIdFilterSections.Add(sectionIdFilterSection);

			InvokeRegenerateUi();
		}

		private void AddSectionDefinitionIdFilterButton_Pressed(object sender, EventArgs e)
		{
			var sectionDefinitionIdFilterSection = new GuidFilterSection<DomInstance>(
				"Section Definition ID",
				new Dictionary<Comparers, Func<Guid, FilterElement<DomInstance>>> 
				{
					{Comparers.Exists, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId) },
					{Comparers.NotExists, (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId) },
				}
			);

			sectionDefinitionIdFilterSections.Add(sectionDefinitionIdFilterSection);

			InvokeRegenerateUi();
		}

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
            AddWidget(moduleId, ++row, 0);
            AddWidget(moduleIdTextBox, row, 2);

            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(definitionIdFilterSection, new SectionLayout(++row, 0));

            AddSection(statusIdFilterSection, new SectionLayout(++row, 0));

            AddSection(createdAtFilterSection, new SectionLayout(++row, 0));

            AddSection(lastModifiedAtFilterSection, new SectionLayout(++row, 0));

			foreach (var sectionDefinitionIdFilterSection in sectionDefinitionIdFilterSections)
			{
				AddSection(sectionDefinitionIdFilterSection, new SectionLayout(++row, 0));
			}

			foreach (var sectionIdFilterSection in sectionIdFilterSections)
			{
				AddSection(sectionIdFilterSection, new SectionLayout(++row, 0));
			}

			foreach (var fieldValueFilterSection in fieldValueFilterSections)
			{
				AddSection(fieldValueFilterSection, new SectionLayout(++row, 0));
			}

			AddWidget(addSectionDefinitionIdFilterButton, ++row, 0, 1, ColumnCount);
			AddWidget(addSectionIdFilterButton, ++row, 0, 1, ColumnCount);
			AddWidget(addFieldValueFilterButton, ++row, 0, 1, ColumnCount);

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
    }
}
