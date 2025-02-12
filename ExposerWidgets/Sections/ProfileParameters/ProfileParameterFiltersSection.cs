namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.ProfileParameters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Parameter = Net.Profiles.Parameter;

	/// <summary>
	/// 
	/// </summary>
	public class ProfileParameterFiltersSection : SectionContainingDataMinerObjectFilters<Parameter>
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
			, string.Join("\n", Enum.GetValues(typeof(Parameter.ParameterType)).Cast<Parameter.ParameterType>().Select(x => $"{x}={(int)x}"))));

		private readonly MultipleFiltersSection<Parameter> discreetFilterSections = new MultipleFiltersSection<Parameter>(new StringFilterSection<Parameter>(
			"Discreet",
			new Dictionary<Comparers, Func<string, FilterElement<Parameter>>>
			{
				{Comparers.Exists,  discreet => ParameterExposers.Discretes.Contains(discreet) },
				{Comparers.NotExists,  discreet => ParameterExposers.Discretes.NotContains(discreet) },
			}, explanation: "Value"));

		/// <summary>
		/// 
		/// </summary>
		public ProfileParameterFiltersSection() : base()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<Parameter> Clone()
		{
			return new ProfileParameterFiltersSection();
		}

		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(idFilterSection, new SectionLayout(++row, 0));
			row += idFilterSection.RowCount;

			AddSection(nameFilterSection, new SectionLayout(row, 0));
			row += nameFilterSection.RowCount;

			AddSection(typeFilterSection, new SectionLayout(row, 0));
			row += typeFilterSection.RowCount;

			AddSection(discreetFilterSections, new SectionLayout(row, 0));
			row += discreetFilterSections.RowCount;
		}
	}
}
