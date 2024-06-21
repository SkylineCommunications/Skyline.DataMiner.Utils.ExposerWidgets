namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;
	using Parameter = Skyline.DataMiner.Net.Profiles.Parameter;

	/// <summary>
	/// Section for filtering profile parameters.
	/// </summary>
	public class FindProfileParametersWithFiltersSection : FindItemsWithFiltersSection<Parameter>
    {
        private readonly FilterSectionBase<Parameter> idFilterSection = new GuidFilterSection<Parameter>(
            "Profile Parameter ID",
            new Dictionary<Helpers.Comparers, Func<Guid, FilterElement<Parameter>>> 
            {
                {Comparers.Equals,  x => ParameterExposers.ID.Equal(x) },
                {Comparers.NotEquals,  x => ParameterExposers.ID.NotEqual(x) },
            }
        );

        private readonly FilterSectionBase<Parameter> nameFilterSection = new StringFilterSection<Parameter>(
            "Profile Parameter Name", 
            new Dictionary<Comparers, Func<string, FilterElement<Parameter>>> 
            {
                {Comparers.Equals, x => ParameterExposers.Name.Equal(x) },
                {Comparers.NotEquals, x => ParameterExposers.Name.NotEqual(x) },
                {Comparers.Contains, x => ParameterExposers.Name.Contains(x) },
                {Comparers.NotContains, x => ParameterExposers.Name.NotContains(x) },
            }
        );

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
			GenerateUi();
		}

        private void AddDiscreetFilterButton_Pressed(object sender, EventArgs e)
        {
            var discreetFilterSection = new StringFilterSection<Parameter>(
                "Discreet", 
                new Dictionary<Comparers, Func<string, FilterElement<Parameter>>> 
                {
                    {Comparers.Exists,  discreet => ParameterExposers.Discretes.Contains(discreet) },
                    {Comparers.NotExists,  discreet => ParameterExposers.Discretes.NotContains(discreet) },
                }
            );

            discreetFilterSections.Add(discreetFilterSection);

            InvokeRegenerateUi();
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

            foreach (var discreetFilterSection in discreetFilterSections)
            {
                AddSection(discreetFilterSection, new SectionLayout(++row, 0));
            }

            AddWidget(addDiscreetFilterButton, ++row, 0);

			firstAvailableColumn = ColumnCount + 1;
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

		/// <summary>
		/// 
		/// </summary>
		protected override void RegenerateFilterSections()
		{
			//TODO complete
		}
	}
}