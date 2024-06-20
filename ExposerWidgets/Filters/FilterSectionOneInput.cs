namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents filter section with one input.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
	/// <typeparam name="FilterInputType">Type of filter that is used.</typeparam>
	public abstract class FilterSectionOneInput<DataMinerObjectType, FilterInputType> : FilterSectionBase<DataMinerObjectType>
    {
        private readonly Func<FilterInputType, FilterElement<DataMinerObjectType>> filterFunctionWithOneInput;
        private readonly Func<FilterInputType, FilterElement<DataMinerObjectType>> invertedFilterFunctionWithOneInput;

		/// <summary>
		/// 
		/// </summary>
		protected readonly Label valueExplanationLabel = new Label();

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionOneInput{T, T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="emptyFilter">Filter that will be applied.</param>
		/// <param name="invertedEmptyFilter"></param>
		protected FilterSectionOneInput(string filterName, Func<FilterInputType, FilterElement<DataMinerObjectType>> emptyFilter, Func<FilterInputType, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null) : base(filterName)
        {
            this.filterFunctionWithOneInput = emptyFilter;
            this.invertedFilterFunctionWithOneInput = invertedEmptyFilter;
        }

        /// <summary>
        /// Value of filter that is being used.
        /// </summary>
        public abstract FilterInputType Value { get; set; }

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public override FilterElement<DataMinerObjectType> FilterElement => IsInverted ? invertedFilterFunctionWithOneInput(Value) : filterFunctionWithOneInput(Value);

		/// <summary>
		/// Indicates if this filter section can be inverted.
		/// </summary>
		protected override bool Invertible => invertedFilterFunctionWithOneInput != null;

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