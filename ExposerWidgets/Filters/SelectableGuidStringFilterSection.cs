﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
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
	public class SelectableGuidStringFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, Guid, string>, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly DropDown firstDropDown = new DropDown() { IsDisplayFilterShown = true };
		private readonly IEnumerable<IDropDownOption<Guid>> _dropDownOptions;

		private readonly TextBox secondTextBox = new TextBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidStringFilterSection{DataMinerObjectType}"/>"/> class.
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="dropDownOptions"></param>
		/// <param name="secondValueExplanation"></param>
		/// <param name="tooltip"></param>
		public SelectableGuidStringFilterSection(string filterName, Dictionary<Comparers, Func<Guid, string, FilterElement<DataMinerObjectType>>> filterFunctions, IEnumerable<IDropDownOption<Guid>> dropDownOptions, string secondValueExplanation = null, string tooltip = null) : base(filterName, filterFunctions, tooltip)
		{
			this._dropDownOptions = dropDownOptions ?? throw new ArgumentNullException(nameof(dropDownOptions));
			var dropDownValuesWithSameDisplayValue = dropDownOptions.GroupBy(dv => dv.DisplayValue).Where(group => group.Count() > 1).ToList();
			if (dropDownValuesWithSameDisplayValue.Any()) throw new ArgumentException($"Multiple dropdown values have same display values: {string.Join(", ", dropDownValuesWithSameDisplayValue.Select(x => x.Key))}", nameof(dropDownOptions));

			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.MinWidth = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;

			secondTextBox.PlaceHolder = secondValueExplanation ?? string.Empty;
			secondTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected SelectableGuidStringFilterSection(SelectableGuidStringFilterSection<DataMinerObjectType> other) : base(other)
		{
			this._dropDownOptions = other._dropDownOptions;
			firstDropDown.Options = _dropDownOptions.Select(dv => dv.DisplayValue).OrderBy(x => x).ToList();
			firstDropDown.MinWidth = firstDropDown.Options.Any() ? 7 * firstDropDown.Options.Max(x => x.Length) : firstDropDown.MinWidth;
			firstDropDown.Changed += (s, e) => isIncludedCheckBox.IsChecked = true;

			secondTextBox.PlaceHolder = other.secondTextBox.PlaceHolder;
			secondTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Indicates if provided guid is valid or not.
		/// </summary>
		public override bool IsValid => true;

		/// <summary>
		/// Gets or sets custom property name value.
		/// </summary>
		public override Guid FirstValue
		{
			get => (_dropDownOptions.SingleOrDefault(dv => dv.DisplayValue == firstDropDown.Selected) ?? throw new InvalidOperationException($"No internal value found for selected option '{firstDropDown.Selected}'")).InternalValue;
			set => firstDropDown.Selected = (_dropDownOptions.SingleOrDefault(dv => dv.InternalValue == value) ?? throw new InvalidOperationException($"No display value found for internal value '{value}'")).DisplayValue;
		}

		/// <summary>
		/// Gets or sets string filter value for custom property.
		/// </summary>
		public override string SecondValue
		{
			get => secondTextBox.Text;
			set => secondTextBox.Text = value;
		}

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => firstDropDown;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => secondTextBox;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new SelectableGuidStringFilterSection<DataMinerObjectType>(this);
		}
	}
}
