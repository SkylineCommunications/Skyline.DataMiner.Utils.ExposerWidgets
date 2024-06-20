namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering profile definitions.
	/// </summary>
	public class FindProfileDefinitionsWithFiltersSection : FindItemsWithFiltersSection<ProfileDefinition>
    {
        private readonly FilterSectionBase<ProfileDefinition> idFilterSection = new GuidFilterSection<ProfileDefinition>("Profile Definition ID", x => ProfileDefinitionExposers.ID.Equal(x));

        private readonly FilterSectionBase<ProfileDefinition> nameFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Equals", x => ProfileDefinitionExposers.Name.Equal(x));

        private readonly FilterSectionBase<ProfileDefinition> nameContainsFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Contains", x => ProfileDefinitionExposers.Name.Contains(x));

        private readonly FilterSectionBase<ProfileDefinition> nameDoesntContainFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Doesn't Contain", x => ProfileDefinitionExposers.Name.NotContains(x));

        private readonly ProfileHelper profileHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindProfileDefinitionsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindProfileDefinitionsWithFiltersSection() : base()
        {
            profileHelper = new ProfileHelper(Engine.SLNet.SendMessages);
			GenerateUi();
		}

		/// <summary>
		/// Adding filter sections on a row specified.
		/// </summary>
		/// <param name="row">Row position where new section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(nameDoesntContainFilterSection, new SectionLayout(++row, 0));

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

        /// <summary>
        /// Resets filters in section to default values.
        /// </summary>
        protected override void ResetFilters()
        {
            // nothing
        }
    }
}
