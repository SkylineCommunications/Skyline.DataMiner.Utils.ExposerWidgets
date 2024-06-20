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
        private readonly Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions;

		/// <summary>
		/// 
		/// </summary>
		protected readonly Label firstValueExplanationLabel = new Label();

		/// <summary>
		/// 
		/// </summary>
		protected readonly Label secondValueExplanationLabel = new Label();

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionTwoInputs{T, T, T}"/>
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		protected FilterSectionTwoInputs(string filterName, Dictionary<Comparers, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName)
        {
			if (filterFunctions is null) throw new ArgumentNullException(nameof(filterFunctions));
			if (!filterFunctions.Any()) throw new ArgumentException("Collection is empty", nameof(filterFunctions));
			this.filterFunctions = filterFunctions;

			filterDropDown.Options = filterFunctions.Keys.Select(x => x.GetDescription()).OrderBy(name => name).ToList();
		}

		/// <summary>
		/// Filter that is created based on input values. Used in getting DataMiner objects in the system.
		/// </summary>
		public override FilterElement<DataMinerObjectType> FilterElement => filterFunctions[filterDropDown.Selected.GetEnumValue<Comparers>()](FirstValue, SecondValue);

		/// <summary>
		/// Gets or sets value of first filter.
		/// </summary>
		public abstract FilterInputType1 FirstValue { get; set; }

        /// <summary>
        /// Gets or sets value of second filter.
        /// </summary>
        public abstract FilterInputType2 SecondValue { get; set; }

		/// <summary>
		/// Sets default values for filters.
		/// </summary>
		/// <param name="value">Default value for first filter.</param>
		/// <param name="secondValue">Default value for second filter.</param>
		public void SetDefault(FilterInputType1 value, FilterInputType2 secondValue)
        {
            IsDefault = true;

            FirstValue = value;
            SecondValue = secondValue;
        }
    }
}
