namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
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
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public BooleanFilterSection(string filterName, Func<bool, FilterElement<DataMinerObjectType>> emptyFilter, Func<bool, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null) : base(filterName, emptyFilter, invertedEmptyFilter)
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

        /// <summary>
        /// Generates UI of checkbox filter section.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterValueCheckBox, 0, nextAvailableColumn++);
        }
    }
}
