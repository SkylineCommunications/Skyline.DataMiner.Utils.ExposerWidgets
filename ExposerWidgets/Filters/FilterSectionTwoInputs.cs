namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

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
        private readonly Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> filterFunctionWithTwoInputs;
        private readonly Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> invertedFilterFunctionWithTwoInputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSectionTwoInputs{T, T, T}"/>
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        protected FilterSectionTwoInputs(string filterName, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> emptyFilter, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null) : base(filterName)
        {
            this.filterFunctionWithTwoInputs = emptyFilter;
            this.invertedFilterFunctionWithTwoInputs = invertedEmptyFilter;
        }

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public override FilterElement<DataMinerObjectType> FilterElement => IsInverted ? invertedFilterFunctionWithTwoInputs(FirstValue, SecondValue) : filterFunctionWithTwoInputs(FirstValue, SecondValue);

        /// <summary>
        /// Gets or sets value of first filter.
        /// </summary>
        public abstract FilterInputType1 FirstValue { get; set; }

        /// <summary>
        /// Gets or sets value of second filter.
        /// </summary>
        public abstract FilterInputType2 SecondValue { get; set; }

		/// <summary>
		/// Indicates if this filter section can be inverted.
		/// </summary>
		protected override bool Invertible => invertedFilterFunctionWithTwoInputs != null;

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
