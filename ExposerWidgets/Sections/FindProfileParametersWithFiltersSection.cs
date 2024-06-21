﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
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
        private readonly MultipleFiltersSection<Parameter> idFilterSection = new MultipleFiltersSection<Parameter>(new GuidFilterSection<Parameter>(
            "ID",
            new Dictionary<Comparers, Func<Guid, FilterElement<Parameter>>> 
            {
                {Comparers.Equals,  x => ParameterExposers.ID.Equal(x) },
                {Comparers.NotEquals,  x => ParameterExposers.ID.NotEqual(x) },
            }
        ));

        private readonly MultipleFiltersSection<Parameter> nameFilterSection = new MultipleFiltersSection<Parameter>(new StringFilterSection<Parameter>(
            "Name", 
            new Dictionary<Comparers, Func<string, FilterElement<Parameter>>> 
            {
                {Comparers.Equals, x => ParameterExposers.Name.Equal(x) },
                {Comparers.NotEquals, x => ParameterExposers.Name.NotEqual(x) },
                {Comparers.Contains, x => ParameterExposers.Name.Contains(x) },
                {Comparers.NotContains, x => ParameterExposers.Name.NotContains(x) },
            }
        ));

		private readonly MultipleFiltersSection<Parameter> typeFilterSection = new MultipleFiltersSection<Parameter>(new IntegerFilterSection<Parameter>(
	        "Type",
	        new Dictionary<Comparers, Func<int, FilterElement<Parameter>>>
	        {
				        {Comparers.Equals, x => ParameterExposers.Type.Equal(x) },
				        {Comparers.NotEquals, x => ParameterExposers.Type.NotEqual(x) },
	        }
            ,$"{Parameter.ParameterType.Undefined}={(int)Parameter.ParameterType.Undefined}\n{nameof(Parameter.ParameterType.Number)}={(int)Parameter.ParameterType.Number}\n{nameof(Parameter.ParameterType.Text)}={(int)Parameter.ParameterType.Text}\n{nameof(Parameter.ParameterType.Discrete)}={(int)Parameter.ParameterType.Discrete}\n"));

		private readonly MultipleFiltersSection<Parameter> discreetFilterSections = new MultipleFiltersSection<Parameter>(new StringFilterSection<Parameter>(
			"Discreet",
			new Dictionary<Comparers, Func<string, FilterElement<Parameter>>>
			{
				{Comparers.Exists,  discreet => ParameterExposers.Discretes.Contains(discreet) },
				{Comparers.NotExists,  discreet => ParameterExposers.Discretes.NotContains(discreet) },
			}
		));

        private readonly ProfileHelper profileHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindProfileParametersWithFiltersSection"/>"/> class.
        /// </summary>
        public FindProfileParametersWithFiltersSection() : base()
        {
            profileHelper = new ProfileHelper(Engine.SLNet.SendMessages);

			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
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
            AddSection(idFilterSection, new SectionLayout(++row, 0));
            row += idFilterSection.RowCount;

            AddSection(nameFilterSection, new SectionLayout(row, 0));
            row += nameFilterSection.RowCount;

			AddSection(typeFilterSection, new SectionLayout(row, 0));
			row += typeFilterSection.RowCount;

			AddSection(discreetFilterSections, new SectionLayout(row, 0));
            row += discreetFilterSections.RowCount;

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
	}
}