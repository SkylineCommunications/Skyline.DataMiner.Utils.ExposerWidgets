namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;
	using Parameter = Skyline.DataMiner.Net.Profiles.Parameter;

	/// <summary>
	/// Section for filtering profile parameters.
	/// </summary>
	public class FindProfileParametersWithFiltersSection : FindItemsWithFiltersSection<Parameter>
    {
        private readonly FilterSectionBase<Parameter> idFilterSection = new GuidFilterSection<Parameter>("Profile Parameter ID", x => ParameterExposers.ID.Equal(x));

        private readonly FilterSectionBase<Parameter> nameFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Equals", x => ParameterExposers.Name.Equal(x));

        private readonly FilterSectionBase<Parameter> nameContainsFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Contains", x => ParameterExposers.Name.Contains(x));

        private readonly FilterSectionBase<Parameter> nameDoesntContainFilterSection = new StringFilterSection<Parameter>("Profile Parameter Name Doesn't Contain", x => ParameterExposers.Name.NotContains(x));

        private readonly List<FilterSectionBase<Parameter>> discreetFilterSections = new List<FilterSectionBase<Parameter>>();
        private readonly Button addDiscreetFilterButton = new Button("Add Discreet Filter");

        private readonly ProfileHelper profileHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindProfileParametersWithFiltersSection"/>"/> class.
        /// </summary>
        public FindProfileParametersWithFiltersSection() : base()
        {
            profileHelper = new ProfileHelper(Engine.SLNet.SendMessages);

            addDiscreetFilterButton.Pressed += AddDiscreetFilterButton_Pressed;
        }

        private void AddDiscreetFilterButton_Pressed(object sender, EventArgs e)
        {
            var discreetFilterSection = new StringFilterSection<Parameter>("Has Discreet", discreet => ParameterExposers.Discretes.Contains(discreet));

            discreetFilterSections.Add(discreetFilterSection);

            InvokeRegenerateUi();
        }

        /// <summary>
        /// Adding filter sections on a row specified.
        /// </summary>
        /// <param name="row">Row position where new section should appear.</param>
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

        /// <summary>
        /// Retrieving all items in the system based on input values.
        /// </summary>
        /// <returns>Collection of profile parameters.</returns>
        protected override IEnumerable<Parameter> FindItemsWithFilters()
        {
            return profileHelper.ProfileParameters.Read(GetCombinedFilterElement());
        }

        /// <summary>
        /// Retrieves name of profile parameter.
        /// </summary>
        /// <returns>Name of profile parameter.</returns>
        protected override string GetItemIdentifier(Parameter item)
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

            discreetFilterSections.Clear();
        }
    }
}