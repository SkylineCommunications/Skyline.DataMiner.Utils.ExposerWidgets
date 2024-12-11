namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	public class SelectableGuidIntegerFilterSection<DataMinerObjectType> : SelectableFilterSectionTwoInputs<DataMinerObjectType, Guid, int>, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly Numeric numeric = new Numeric(0) { Decimals = 0, StepSize = 0 };

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidStringFilterSection{DataMinerObjectType}"/>"/> class.
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="dropDownOptions"></param>
		/// <param name="tooltip"></param>
		public SelectableGuidIntegerFilterSection(string filterName, Dictionary<Comparers, Func<Guid, int, FilterElement<DataMinerObjectType>>> filterFunctions, IEnumerable<IDropDownOption<Guid>> dropDownOptions, string tooltip = null) : base(filterName, filterFunctions, dropDownOptions, tooltip)
		{
			numeric.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected SelectableGuidIntegerFilterSection(SelectableGuidIntegerFilterSection<DataMinerObjectType> other) : base(other)
		{
			numeric.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Indicates if provided guid is valid or not.
		/// </summary>
		public override bool IsValid => true;

		/// <summary>
		/// Gets or sets string filter value for custom property.
		/// </summary>
		public override int SecondValue
		{
			get => (int)numeric.Value;
			set => numeric.Value = value;
		}

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => numeric;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new SelectableGuidIntegerFilterSection<DataMinerObjectType>(this);
		}
	}
}
