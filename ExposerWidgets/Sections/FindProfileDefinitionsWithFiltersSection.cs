namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering profile definitions.
	/// </summary>
	public class FindProfileDefinitionsWithFiltersSection : FindItemsWithFiltersSection<ProfileDefinition>
    {
        private readonly MultipleFiltersSection<ProfileDefinition> idFilterSection = new MultipleFiltersSection<ProfileDefinition>(new GuidFilterSection<ProfileDefinition>(
            "ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<ProfileDefinition>>> 
            {
                {Comparers.Equals, x => ProfileDefinitionExposers.ID.Equal(x) },
                {Comparers.NotEquals, x => ProfileDefinitionExposers.ID.NotEqual(x) },
            }));

        private readonly MultipleFiltersSection<ProfileDefinition> nameFilterSection = new MultipleFiltersSection<ProfileDefinition>(new StringFilterSection<ProfileDefinition>(
            "Name",
            new Dictionary<Comparers, Func<string, FilterElement<ProfileDefinition>>> 
            {
                {Comparers.Equals, x => ProfileDefinitionExposers.Name.Equal(x) },
                {Comparers.NotEquals, x => ProfileDefinitionExposers.Name.NotEqual(x) },
                {Comparers.Contains, x => ProfileDefinitionExposers.Name.Contains(x) },
                {Comparers.NotContains, x => ProfileDefinitionExposers.Name.NotContains(x) },
            }));

		private readonly MultipleFiltersSection<ProfileDefinition> descriptionFilterSection = new MultipleFiltersSection<ProfileDefinition>(new StringFilterSection<ProfileDefinition>(
	        "Description",
	        new Dictionary<Comparers, Func<string, FilterElement<ProfileDefinition>>>
	        {
				    {Comparers.Equals, x => ProfileDefinitionExposers.Description.Equal(x) },
				    {Comparers.NotEquals, x => ProfileDefinitionExposers.Description.NotEqual(x) },
				    {Comparers.Contains, x => ProfileDefinitionExposers.Description.Contains(x) },
				    {Comparers.NotContains, x => ProfileDefinitionExposers.Description.NotContains(x) },
	        }));

		private readonly MultipleFiltersSection<ProfileDefinition> createdByFilterSection = new MultipleFiltersSection<ProfileDefinition>(new StringFilterSection<ProfileDefinition>(
	        "Created By",
	        new Dictionary<Comparers, Func<string, FilterElement<ProfileDefinition>>>
	        {
					{Comparers.Equals, x => ProfileDefinitionExposers.CreatedBy.Equal(x) },
					{Comparers.NotEquals, x => ProfileDefinitionExposers.CreatedBy.NotEqual(x) },
					{Comparers.Contains, x => ProfileDefinitionExposers.CreatedBy.Contains(x) },
					{Comparers.NotContains, x => ProfileDefinitionExposers.CreatedBy.NotContains(x) },
	        }));

		private readonly MultipleFiltersSection<ProfileDefinition> lastModifiedByFilterSection = new MultipleFiltersSection<ProfileDefinition>(new StringFilterSection<ProfileDefinition>(
			"Last Modified By",
			new Dictionary<Comparers, Func<string, FilterElement<ProfileDefinition>>>
			{
					{Comparers.Equals, x => ProfileDefinitionExposers.LastModifiedBy.Equal(x) },
					{Comparers.NotEquals, x => ProfileDefinitionExposers.LastModifiedBy.NotEqual(x) },
					{Comparers.Contains, x => ProfileDefinitionExposers.LastModifiedBy.Contains(x) },
					{Comparers.NotContains, x => ProfileDefinitionExposers.LastModifiedBy.NotContains(x) },
			}));

		private readonly MultipleFiltersSection<ProfileDefinition> lastModifiedAtFilterSection = new MultipleFiltersSection<ProfileDefinition>(new DateTimeFilterSection<ProfileDefinition>(
			"Last Modified At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<ProfileDefinition>>>
			{
					{Comparers.GreaterThan, x => ProfileDefinitionExposers.LastModifiedAt.GreaterThan(x) },
					{Comparers.LessThan, x => ProfileDefinitionExposers.LastModifiedAt.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<ProfileDefinition> createdAtFilterSection = new MultipleFiltersSection<ProfileDefinition>(new DateTimeFilterSection<ProfileDefinition>(
			"Created At",
			new Dictionary<Comparers, Func<DateTime, FilterElement<ProfileDefinition>>>
			{
					{Comparers.GreaterThan, x => ProfileDefinitionExposers.CreatedAt.GreaterThan(x) },
					{Comparers.LessThan, x => ProfileDefinitionExposers.CreatedAt.LessThan(x) },
			}));

		private readonly MultipleFiltersSection<ProfileDefinition> parameterFilterSection = new MultipleFiltersSection<ProfileDefinition>(new GuidFilterSection<ProfileDefinition>(
			"Parameter",
			new Dictionary<Comparers, Func<Guid, FilterElement<ProfileDefinition>>>
			{
					{Comparers.IsUsed, x => ProfileDefinitionExposers.ParameterIDs.Contains(x) },
					{Comparers.IsNotUsed, x => ProfileDefinitionExposers.ParameterIDs.NotContains(x) },
			}));

		private readonly ProfileHelper profileHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindProfileDefinitionsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindProfileDefinitionsWithFiltersSection() : base()
        {
            profileHelper = new ProfileHelper(Engine.SLNet.SendMessages);

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s,e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		/// <summary>
		/// Adding filter sections on a row specified.
		/// </summary>
		/// <param name="row">Row position where new section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddSection(idFilterSection, new SectionLayout(row, 0));
            row += idFilterSection.RowCount;

            AddSection(nameFilterSection, new SectionLayout(row, 0));
            row += nameFilterSection.RowCount;

			AddSection(descriptionFilterSection, new SectionLayout(row, 0));
			row += descriptionFilterSection.RowCount;

			AddSection(createdByFilterSection, new SectionLayout(row, 0));
			row += createdByFilterSection.RowCount;

			AddSection(createdAtFilterSection, new SectionLayout(row, 0));
			row += createdAtFilterSection.RowCount; 
			
			AddSection(lastModifiedByFilterSection, new SectionLayout(row, 0));
			row += lastModifiedByFilterSection.RowCount;

			AddSection(lastModifiedAtFilterSection, new SectionLayout(row, 0));
			row += lastModifiedAtFilterSection.RowCount;

			AddSection(parameterFilterSection, new SectionLayout(row, 0));
			row += parameterFilterSection.RowCount;

			firstAvailableColumn = ColumnCount + 1;
		}

        /// <summary>
        /// Retrieving all items in the system based on input values.
        /// </summary>
        /// <returns>Collection of profile definitions.</returns>
        protected override IEnumerable<ProfileDefinition> FindItemsWithFilters()
        {
            return profileHelper.ProfileDefinitions.Read(GetCombinedFilterElement());
        }

        /// <summary>
        /// Retrieves name of profile definition.
        /// </summary>
        /// <returns>Name of profile definition.</returns>
        protected override string GetItemIdentifier(ProfileDefinition item)
        {
            return item.Name;
        }
	}
}
