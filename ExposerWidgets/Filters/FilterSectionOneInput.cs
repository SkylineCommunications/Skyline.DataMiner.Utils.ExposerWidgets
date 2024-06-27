namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents filter section with one input.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
	/// <typeparam name="FilterInputType">Type of filter that is used.</typeparam>
	public abstract class FilterSectionOneInput<DataMinerObjectType, FilterInputType> : FilterSectionBase<DataMinerObjectType>
    {
		private Dictionary<Comparers, Func<FilterInputType, FilterElement<DataMinerObjectType>>> filterFunctions;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionOneInput{T, T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		/// <param name="tooltip"></param>
		protected FilterSectionOneInput(string filterName, Dictionary<Comparers, Func<FilterInputType, FilterElement<DataMinerObjectType>>> filterFunctions, string tooltip = null) : base(filterName, tooltip)
		{
			Initialize(filterFunctions);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected FilterSectionOneInput(FilterSectionOneInput<DataMinerObjectType, FilterInputType> other) : base(other)
		{
			Initialize(other.filterFunctions);
		}

		/// <summary>
		/// Value of filter that is being used.
		/// </summary>
		public abstract FilterInputType Value { get; set; }

		/// <summary>
		/// Filter that is created based on input values. Used in getting DataMiner objects in the system.
		/// </summary>
		public override FilterElement<DataMinerObjectType> FilterElement => filterFunctions[comparerDropDown.Selected.GetEnumValue<Comparers>()](Value);

		/// <summary>
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected abstract InteractiveWidget InputWidget { get; }

		/// <summary>
		/// Adds the widgets to the Section.
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
				AddWidget(comparerDropDown, 0, column, 1, 3);
				column += 3;
				AddWidget(InputWidget, 0, column, 1, 3);
			}
			else
			{
				AddWidget(InputWidget, 0, column, 1, 3);
				column += 3;
				AddWidget(comparerDropDown, 0, column, 1, 3);
			}
		}

		private void Initialize(Dictionary<Comparers, Func<FilterInputType, FilterElement<DataMinerObjectType>>> filterFunctions)
		{
			if (filterFunctions is null) throw new ArgumentNullException(nameof(filterFunctions));
			if (!filterFunctions.Any()) throw new ArgumentException("Collection is empty", nameof(filterFunctions));

			this.filterFunctions = filterFunctions;

			comparerDropDown.Options = filterFunctions.Keys.Select(k => k.GetDescription()).ToList();
		}
	}
}