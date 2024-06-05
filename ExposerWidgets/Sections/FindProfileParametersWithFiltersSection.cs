namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using Skyline.DataMiner.Core.SRM;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Profiles;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parameter = Skyline.DataMiner.Net.Profiles.Parameter;

    public class FindProfileParametersWithFiltersSection : FindItemsWithFiltersSection<Parameter>
    {
        private readonly FilterSectionBase<Parameter> idFilterSection = new GuidFilterSection<Parameter>("Profile Parameter ID", x => ParameterExposers.ID.Equal((Guid)x));

        private readonly FilterSectionBase<Parameter> nameFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Equals", x => ParameterExposers.Name.Equal((string)x));

        private readonly FilterSectionBase<Parameter> nameContainsFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Contains", x => ParameterExposers.Name.Contains((string)x));

        private readonly FilterSectionBase<Parameter> nameDoesntContainFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Doesn't Contain", x => ParameterExposers.Name.NotContains((string)x));

        private readonly List<FilterSectionBase<Parameter>> discreetFilterSections = new List<FilterSectionBase<Parameter>>();
        private readonly Button addDiscreetFilterButton = new Button("Add Discreet Filter");

        public FindProfileParametersWithFiltersSection() : base()
        {
            addDiscreetFilterButton.Pressed += AddDiscreetFilterButton_Pressed;
        }

        private void AddDiscreetFilterButton_Pressed(object sender, EventArgs e)
        {
            var discreetFilterSection = new StringFilterSection<Parameter>("Has Discreet", discreet => ParameterExposers.Discretes.Contains((string)discreet));

            discreetFilterSections.Add(discreetFilterSection);

            InvokeRegenerateUi();
        }

        protected override void AddFilterSections(ref int row)
        {
            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(nameDoesntContainFilterSection, new SectionLayout(++row, 0));

            foreach (var discreetFilterSection in discreetFilterSections)
            {
                AddSection(discreetFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addDiscreetFilterButton, ++row, 0);
        }

        protected override IEnumerable<Parameter> FindItemsWithFilters()
        {
            return SrmManagers.ProfileManager.GetParametersWithFilter(GetCombinedFilterElement());
        }

        protected override string GetItemIdentifier(Parameter item)
        {
            return item.Name;
        }

        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }

            discreetFilterSections.Clear();
        }
    }
}