namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    ///  Represents filter section with one numeric input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class IntegerFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Numeric input widget.
        /// </summary>
        protected readonly Numeric filterContentNumeric = new Numeric();

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegerFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		/// <param name="tooltip"></param>
		public IntegerFilterSection(string filterName, Dictionary<Comparers, Func<int, FilterElement<DataMinerObjectType>>> filterFunctions, string tooltip = null) : base(filterName, filterFunctions, tooltip)
        {
            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected IntegerFilterSection(IntegerFilterSection<DataMinerObjectType> other) : base(other)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if numeric value provided is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets numeric value of filter.
        /// </summary>
        public override int Value
        {
            get => (int)filterContentNumeric.Value;
            set => filterContentNumeric.Value = value;
        }

		/// <summary>
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget InputWidget => filterContentNumeric;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new IntegerFilterSection<DataMinerObjectType>(this);
		}
    }
}