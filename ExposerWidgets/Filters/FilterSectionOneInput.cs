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
		private readonly Dictionary<Comparers, Func<FilterInputType, FilterElement<DataMinerObjectType>>> filterFunctions;

		/// <summary>
		/// 
		/// </summary>
		protected readonly Label valueExplanationLabel = new Label();

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionOneInput{T, T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		protected FilterSectionOneInput(string filterName, Dictionary<Comparers, Func<FilterInputType, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName)
        {
			if(filterFunctions is null) throw new ArgumentNullException(nameof(filterFunctions));
			if(!filterFunctions.Any()) throw new ArgumentException("Collection is empty", nameof(filterFunctions));
            this.filterFunctions = filterFunctions;

			filterDropDown.Options = filterFunctions.Keys.Select(k => k.GetDescription()).OrderBy(name => name).ToList();
		}

        /// <summary>
        /// Value of filter that is being used.
        /// </summary>
        public abstract FilterInputType Value { get; set; }

		/// <summary>
		/// Filter that is created based on input values. Used in getting DataMiner objects in the system.
		/// </summary>
		public override FilterElement<DataMinerObjectType> FilterElement => filterFunctions[filterDropDown.Selected.GetEnumValue<Comparers>()](Value);

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