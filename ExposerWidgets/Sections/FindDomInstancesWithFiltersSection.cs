﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering dom instances.
	/// </summary>
	public class FindDomInstancesWithFiltersSection : FindItemsWithFiltersSection<DomInstance>
    {
        private readonly Label moduleId = new Label("Module ID:");
		private readonly TextBox moduleIdTextBox = new TextBox(string.Empty);

		private readonly FilterSectionBase<DomInstance> idFilterSection = new GuidFilterSection<DomInstance>("Dom Instance ID Equals", x => DomInstanceExposers.Id.Equal(x), x => DomInstanceExposers.Id.NotEqual(x));

        private readonly FilterSectionBase<DomInstance> nameFilterSection = new StringFilterSection<DomInstance>("Dom Instance Name Equals", x => DomInstanceExposers.Name.Equal(x), x => DomInstanceExposers.Name.NotEqual(x));

        private readonly FilterSectionBase<DomInstance> nameContainsFilterSection = new StringFilterSection<DomInstance>("Dom Instance Name Contains", x => DomInstanceExposers.Name.Contains(x), x => DomInstanceExposers.Name.NotContains(x));

		private readonly FilterSectionBase<DomInstance> definitionIdFilterSection = new GuidFilterSection<DomInstance>("Dom Definition ID Equals", x => DomInstanceExposers.DomDefinitionId.Equal(x), x => DomInstanceExposers.DomDefinitionId.NotEqual(x));

		private readonly FilterSectionBase<DomInstance> statusIdFilterSection = new StringFilterSection<DomInstance>("Dom Status ID Equals", x => DomInstanceExposers.StatusId.Equal(x), x => DomInstanceExposers.StatusId.NotEqual(x));

		private readonly FilterSectionBase<DomInstance> createdAtFromFilterSection = new DateTimeFilterSection<DomInstance>("Dom Instance Created At From", x => DomInstanceExposers.CreatedAt.GreaterThanOrEqual(x));

		private readonly FilterSectionBase<DomInstance> createdAtUntilFilterSection = new DateTimeFilterSection<DomInstance>("Dom Instance Created At Until", x => DomInstanceExposers.CreatedAt.LessThanOrEqual(x));

		private readonly FilterSectionBase<DomInstance> lastModifiedAtFromFilterSection = new DateTimeFilterSection<DomInstance>("Dom Instance Last Modified At From", x => DomInstanceExposers.LastModified.GreaterThanOrEqual(x));

		private readonly FilterSectionBase<DomInstance> lastModifiedAtUntilFilterSection = new DateTimeFilterSection<DomInstance>("Dom Instance Last Modified At Until", x => DomInstanceExposers.LastModified.LessThanOrEqual(x));

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

		private void AddFieldValueFilterButton_Pressed(object sender, System.EventArgs e)
		{
			var fieldValueFilterSection = new StringStringFilterSection<DomInstance>("Field Equals", (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).Equal(fieldValue), (fieldName, fieldValue) => DomInstanceExposers.FieldValues.DomInstanceField(fieldName).NotEqual(fieldValue), "Name", "Value");

			fieldValueFilterSections.Add(fieldValueFilterSection);
			InvokeRegenerateUi();
		}

		private void AddSectionIdFilterButton_Pressed(object sender, System.EventArgs e)
		{
			var sectionIdFilterSection = new GuidFilterSection<DomInstance>("Uses Section ID", (sectionId) => DomInstanceExposers.SectionIds.Contains(sectionId), (sectionId) => DomInstanceExposers.SectionIds.NotContains(sectionId));

			sectionIdFilterSections.Add(sectionIdFilterSection);

			InvokeRegenerateUi();
		}

		private void AddSectionDefinitionIdFilterButton_Pressed(object sender, System.EventArgs e)
		{
			var sectionDefinitionIdFilterSection = new GuidFilterSection<DomInstance>("Uses Section Definition ID", (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.Contains(sectionDefinitionId), (sectionDefinitionId) => DomInstanceExposers.SectionDefinitionIds.NotContains(sectionDefinitionId));

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
            AddWidget(moduleIdTextBox, row, 1);

            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(definitionIdFilterSection, new SectionLayout(++row, 0));

            AddSection(statusIdFilterSection, new SectionLayout(++row, 0));

            AddSection(createdAtFromFilterSection, new SectionLayout(++row, 0));

            AddSection(createdAtUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(lastModifiedAtFromFilterSection, new SectionLayout(++row, 0));

            AddSection(lastModifiedAtUntilFilterSection, new SectionLayout(++row, 0));

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

			AddWidget(addSectionDefinitionIdFilterButton, ++row, 0);
			AddWidget(addSectionIdFilterButton, ++row, 0);
			AddWidget(addFieldValueFilterButton, ++row, 0);

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
                return new List<DomInstance>();
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
