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
	public class SelectableGuidFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, Guid>, IDataMinerObjectFilter<DataMinerObjectType>
    {
		private readonly DropDown firstDropDown = new DropDown() { IsDisplayFilterShown = true };
		private readonly IEnumerable<IDropDownOption<Guid>> _dropDownOptions;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="dropDownOptions"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public SelectableGuidFilterSection(string filterName, Dictionary<Comparers, Func<Guid, FilterElement<DataMinerObjectType>>> filterFunctions, IEnumerable<IDropDownOption<Guid>> dropDownOptions) : base(filterName, filterFunctions)
		{
			this._dropDownOptions = dropDownOptions ?? throw new ArgumentNullException(nameof(dropDownOptions));
			var dropDownValuesWithSameDisplayValue = dropDownOptions.GroupBy(dv => dv.DisplayValue).Where(group => group.Count() > 1).ToList();
			if (dropDownValuesWithSameDisplayValue.Any()) throw new ArgumentException($"Multiple dropdown values have same display values: {string.Join(", ", dropDownValuesWithSameDisplayValue.Select(x => x.Key))}", nameof(dropDownOptions));

			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.Width = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected SelectableGuidFilterSection(SelectableGuidFilterSection<DataMinerObjectType> other) : base(other)
		{
			this._dropDownOptions = other._dropDownOptions;
			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.Width = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Indicates if provided guid is valid or not.
		/// </summary>
		public override bool IsValid => true;

		/// <summary>
		/// Gets or sets custom property name value.
		/// </summary>
		public override Guid Value
		{
			get => (_dropDownOptions.SingleOrDefault(dv => dv.DisplayValue == firstDropDown.Selected) ?? throw new InvalidOperationException($"No internal value found for selected option '{firstDropDown.Selected}'")).InternalValue;
			set => firstDropDown.Selected = (_dropDownOptions.SingleOrDefault(dv => dv.InternalValue == value) ?? throw new InvalidOperationException($"No display value found for internal value '{value}'")).DisplayValue;
		}

		/// <summary>
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget InputWidget => firstDropDown;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new SelectableGuidFilterSection<DataMinerObjectType>(this);
		}
	}
}
