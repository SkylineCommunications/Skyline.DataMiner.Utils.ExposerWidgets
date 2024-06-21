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
		public IntegerFilterSection(string filterName, Dictionary<Comparers, Func<int, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
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
        /// 
        /// </summary>
		protected override InteractiveWidget InputWidget => filterContentNumeric;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new IntegerFilterSection<DataMinerObjectType>(this);
		}

		/// <summary>
		/// Resets filter to default values.
		/// </summary>
		public override void Reset()
        {
            IsIncluded = false;
            Value = 0;
        }

        /// <summary>
        /// Handles default update of filter.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterContentNumeric.IsEnabled = !IsDefault;
        }
    }
}