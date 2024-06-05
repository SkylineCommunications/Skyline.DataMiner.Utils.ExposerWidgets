namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Core.SRM;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Profiles;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;

    internal class FindProfileDefinitionsWithFiltersSection : FindItemsWithFiltersSection<ProfileDefinition>
    {
        private readonly FilterSectionBase<ProfileDefinition> idFilterSection = new GuidFilterSection<ProfileDefinition>("Profile Definition ID", x => ProfileDefinitionExposers.ID.Equal((Guid)x));

        private readonly FilterSectionBase<ProfileDefinition> nameFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Equals", x => ProfileDefinitionExposers.Name.Equal((string)x));

        private readonly FilterSectionBase<ProfileDefinition> nameContainsFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Contains", x => ProfileDefinitionExposers.Name.Contains((string)x));

        private readonly FilterSectionBase<ProfileDefinition> nameDoesntContainFilterSection = new StringFilterSection<ProfileDefinition>("Profile Definition Name Doesn't Contain", x => ProfileDefinitionExposers.Name.NotContains((string)x));

        public FindProfileDefinitionsWithFiltersSection() : base()
        {
        }

        protected override void AddFilterSections(ref int row)
        {
            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(nameDoesntContainFilterSection, new SectionLayout(++row, 0));
        }

        protected override IEnumerable<ProfileDefinition> FindItemsWithFilters()
        {
            return SrmManagers.ProfileManager.GetProfileDefinitionsWithFilter(GetCombinedFilterElement());
        }

        protected override string GetItemIdentifier(ProfileDefinition item)
        {
            return item.Name;
        }

        protected override void ResetFilters()
        {
            // nothing
        }
    }
}
