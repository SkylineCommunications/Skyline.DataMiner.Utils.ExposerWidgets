namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;

    /// <summary>
    /// Represents filter section with one input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
    /// <typeparam name="FilterInputType">Type of filter that is used.</typeparam>
    public abstract class FilterSectionOneInput<DataMinerObjectType, FilterInputType> : FilterSectionBase<DataMinerObjectType>
    {
        private readonly Func<FilterInputType, FilterElement<DataMinerObjectType>> filterFunctionWithOneInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSectionOneInput{T, T}"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        protected FilterSectionOneInput(string filterName, Func<FilterInputType, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName)
        {
            this.filterFunctionWithOneInput = emptyFilter;
        }

        /// <summary>
        /// Value of filter that is being used.
        /// </summary>
        public abstract FilterInputType Value { get; set; }

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public override FilterElement<DataMinerObjectType> FilterElement => filterFunctionWithOneInput(Value);

        /// <summary>
        /// Sets default value for filter.
        /// </summary>
        /// <param name="value">Value of filter that will be used.</param>
        public void SetDefault(FilterInputType value)
        {
            IsDefault = true;

            Value = value;
        }
    }
}