namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents filter section with three inputs.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
	/// <typeparam name="FilterInputType1">Type of first filter that is used.</typeparam>
	/// <typeparam name="FilterInputType2">Type of second filter that is used.</typeparam>
	/// <typeparam name="FilterInputType3">Type of third filter that is used.</typeparam>
#pragma warning disable S2436 // Types and methods should not have too many generic parameters
	public abstract class FilterSectionThreeInputs<DataMinerObjectType, FilterInputType1, FilterInputType2, FilterInputType3> : FilterSectionBase<DataMinerObjectType>
#pragma warning restore S2436 // Types and methods should not have too many generic parameters
    {
        private Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>>> filterFunctions;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionThreeInputs{T, T, T, T}"/>
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		protected FilterSectionThreeInputs(string filterName, Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>>> filterFunctions, string tooltip = null) : base(filterName, tooltip)
		{
			Initialize(filterFunctions);
		}

		private void Initialize(Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>>> filterFunctions)
		{
			if (filterFunctions is null) throw new ArgumentNullException(nameof(filterFunctions));
			if (!filterFunctions.Any()) throw new ArgumentException("Collection is empty", nameof(filterFunctions));

			this.filterFunctions = filterFunctions;

			filterDropDown.Options = filterFunctions.Keys.Select(x => x.GetDescription()).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		protected FilterSectionThreeInputs(FilterSectionThreeInputs<DataMinerObjectType, FilterInputType1, FilterInputType2, FilterInputType3> other) : base(other)
		{
			Initialize(other.filterFunctions);
		}

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public override FilterElement<DataMinerObjectType> FilterElement => filterFunctions[filterDropDown.Selected.GetEnumValue<Comparers>()](FirstValue, SecondValue, ThirdValue);

		/// <summary>
		/// Gets or sets value of first filter.
		/// </summary>
		public abstract FilterInputType1 FirstValue { get; set; }

        /// <summary>
        /// Gets or sets value of second filter.
        /// </summary>
        public abstract FilterInputType2 SecondValue { get; set; }

        /// <summary>
        /// Gets or sets value of third filter.
        /// </summary>
        public abstract FilterInputType3 ThirdValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected abstract InteractiveWidget FirstInputWidget { get; }

		/// <summary>
		/// 
		/// </summary>
		protected abstract InteractiveWidget SecondInputWidget { get; }

		/// <summary>
		/// 
		/// </summary>
		protected abstract InteractiveWidget ThirdInputWidget { get; }

		/// <summary>
		/// Sets default values for filters.
		/// </summary>
		/// <param name="value">Default value for first filter.</param>
		/// <param name="secondValue">Default value for second filter.</param>
		/// <param name="thirdValue">Default value for third filter.</param>
		public void SetDefault(FilterInputType1 value, FilterInputType2 secondValue, FilterInputType3 thirdValue)
        {
            IsDefault = true;

            FirstValue = value;
            SecondValue = secondValue;
            ThirdValue = thirdValue;
        }

		protected override void GenerateUi()
		{
			Clear();

			int column = -1;

			if (!string.IsNullOrWhiteSpace(tooltipLabel.Tooltip)) AddWidget(tooltipLabel, 0, ++column, verticalAlignment: VerticalAlignment.Top);
			else ++column;

			AddWidget(filterNameCheckBox, 0, ++column, 1, 3);
			column += 3;

			if (filterFunctions.First().Key.GetComparerType() == ComparerType.Active)
			{
				AddWidget(FirstInputWidget, 0, column, 1, 3);
				column += 3;

				AddWidget(filterDropDown, 0, column, 1, 3);
				column += 3;

				AddWidget(SecondInputWidget, 0, column, 1, 3);
				AddWidget(ThirdInputWidget, 1, column, 1, 3);
			}
			else
			{
				AddWidget(FirstInputWidget, 0, column, 1, 3);
				column += 3;

				AddWidget(SecondInputWidget, 0, column, 1, 3);
				AddWidget(ThirdInputWidget, 1, column, 1, 3);
				column += 3;

				AddWidget(filterDropDown, 0, column, 1, 3);
			}
		}
	}

}
