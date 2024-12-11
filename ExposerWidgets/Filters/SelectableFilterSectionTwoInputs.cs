namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	/// <typeparam name="FilterInputType1"></typeparam>
	/// <typeparam name="FilterInputType2"></typeparam>
	public abstract class SelectableFilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2> : FilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2>
	{
		private readonly DropDown firstDropDown = new DropDown() { IsDisplayFilterShown = true };
		private readonly IEnumerable<IDropDownOption<FilterInputType1>> _dropDownOptions;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="dropDownOptions"></param>
		/// <param name="tooltip"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		protected SelectableFilterSectionTwoInputs(string filterName, Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions, IEnumerable<IDropDownOption<FilterInputType1>> dropDownOptions, string tooltip = null) : base(filterName, filterFunctions, tooltip)
		{
			this._dropDownOptions = dropDownOptions ?? throw new ArgumentNullException(nameof(dropDownOptions));
			var dropDownValuesWithSameDisplayValue = dropDownOptions.GroupBy(dv => dv.DisplayValue).Where(group => group.Count() > 1).ToList();
			if (dropDownValuesWithSameDisplayValue.Any()) throw new ArgumentException($"Multiple dropdown values have same display values: {string.Join(", ", dropDownValuesWithSameDisplayValue.Select(x => x.Key))}", nameof(dropDownOptions));

			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.MinWidth = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected SelectableFilterSectionTwoInputs(SelectableFilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2> other) : base(other)
		{
			this._dropDownOptions = other._dropDownOptions;

			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.MinWidth = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;
		}

		/// <summary>
		/// 
		/// </summary>
		public override FilterInputType1 FirstValue
		{
			get => (_dropDownOptions.SingleOrDefault(dv => dv.DisplayValue == firstDropDown.Selected) ?? throw new InvalidOperationException($"No internal value found for selected option '{firstDropDown.Selected}'")).InternalValue;
			set => firstDropDown.Selected = (_dropDownOptions.SingleOrDefault(dv => dv.InternalValue.Equals(value)) ?? throw new InvalidOperationException($"No display value found for internal value '{value}'")).DisplayValue;
		}

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => firstDropDown;
	}
}
