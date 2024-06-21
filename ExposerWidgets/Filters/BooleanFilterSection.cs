namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one checkbox input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class BooleanFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, bool>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private readonly CheckBox filterValueCheckBox = new CheckBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		public BooleanFilterSection(string filterName, Dictionary<Comparers, Func<bool, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
        {
            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected BooleanFilterSection(BooleanFilterSection<DataMinerObjectType> other) : base(other)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if filter is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets value of checkbox.
        /// </summary>
        public override bool Value
        {
            get => filterValueCheckBox.IsChecked;
            set => filterValueCheckBox.IsChecked = value;
        }

        /// <summary>
        /// 
        /// </summary>
		protected override InteractiveWidget InputWidget => filterValueCheckBox;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new BooleanFilterSection<DataMinerObjectType>(this);
		}

		/// <summary>
		/// Sets value to unchecked and filter as non active.
		/// </summary>
		public override void Reset()
        {
            IsIncluded = false;
            Value = false;
        }

        /// <summary>
        /// Handles default update of checkbox filter.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterValueCheckBox.IsEnabled = !IsDefault;
        }
    }
}
