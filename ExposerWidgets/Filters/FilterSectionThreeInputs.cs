﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

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
        private readonly Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> filterFunctionWithThreeInputs;
        private readonly Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> invertedFilterFunctionWithThreeInputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSectionThreeInputs{T, T, T, T}"/>
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        protected FilterSectionThreeInputs(string filterName, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> emptyFilter, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null) : base(filterName)
        {
            filterFunctionWithThreeInputs = emptyFilter;
            invertedFilterFunctionWithThreeInputs = invertedEmptyFilter;
        }

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public override FilterElement<DataMinerObjectType> FilterElement => IsInverted ? invertedFilterFunctionWithThreeInputs(FirstValue, SecondValue, ThirdValue) : filterFunctionWithThreeInputs(FirstValue, SecondValue, ThirdValue);

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

		protected override bool Invertible => invertedFilterFunctionWithThreeInputs != null;

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
    }

}
