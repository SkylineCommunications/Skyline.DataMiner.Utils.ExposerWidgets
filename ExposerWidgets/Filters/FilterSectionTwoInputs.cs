namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents filter section with two input.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
	/// <typeparam name="FilterInputType1">Type of first filter that is used.</typeparam>
	/// <typeparam name="FilterInputType2">Type of second filter that is used.</typeparam>
#pragma warning disable S2436 // Types and methods should not have too many generic parameters
	public abstract class FilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2> : FilterSectionBase<DataMinerObjectType>
#pragma warning restore S2436 // Types and methods should not have too many generic parameters
    {
        private Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionTwoInputs{T, T, T}"/>
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		/// <param name="tooltip"></param>
		protected FilterSectionTwoInputs(string filterName, Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions, string tooltip = null) : base(filterName, tooltip)
		{
			Initialize(filterFunctions);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected FilterSectionTwoInputs(FilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2> other) : base(other)
		{
			Initialize(other.filterFunctions);
		}

		/// <summary>
		/// Filter that is created based on input values. Used in getting DataMiner objects in the system.
		/// </summary>
		public override FilterElement<DataMinerObjectType> FilterElement => filterFunctions[comparerDropDown.Selected.GetEnumValue<Comparers>()](FirstValue, SecondValue);

		/// <summary>
		/// Gets or sets value of first filter.
		/// </summary>
		public abstract FilterInputType1 FirstValue { get; set; }

        /// <summary>
        /// Gets or sets value of second filter.
        /// </summary>
        public abstract FilterInputType2 SecondValue { get; set; }

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected abstract InteractiveWidget FirstInputWidget { get; }

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected abstract InteractiveWidget SecondInputWidget { get; }

		/// <summary>
		/// Adds the widgets to the section.
		/// </summary>
		protected override void GenerateUi()
		{
			Clear();

			int column = -1;

			if (!string.IsNullOrWhiteSpace(tooltipLabel.Tooltip)) AddWidget(tooltipLabel, 0, ++column, verticalAlignment: VerticalAlignment.Top);
			else ++column;

			AddWidget(isIncludedCheckBox, 0, ++column, 1, 3);
			column += 3;

			if (filterFunctions.First().Key.GetComparerType() == ComparerType.Active)
			{
				AddWidget(FirstInputWidget, 0, column, 1, 3);
				column += 3;

				AddWidget(comparerDropDown, 0, column, 1, 3);
				column += 3;

				AddWidget(SecondInputWidget, 0, column, 1, 3);
			}
			else
			{
				AddWidget(FirstInputWidget, 0, column, 1, 3);
				column += 3;

				AddWidget(SecondInputWidget, 0, column, 1, 3);
				column += 3;

				AddWidget(comparerDropDown, 0, column, 1, 3);
			}
		}

		private void Initialize(Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions)
		{
			if (filterFunctions is null) throw new ArgumentNullException(nameof(filterFunctions));
			if (!filterFunctions.Any()) throw new ArgumentException("Collection is empty", nameof(filterFunctions));

			this.filterFunctions = filterFunctions;

			comparerDropDown.Options = filterFunctions.Keys.Select(x => x.GetDescription()).ToList();
		}
	}
}
