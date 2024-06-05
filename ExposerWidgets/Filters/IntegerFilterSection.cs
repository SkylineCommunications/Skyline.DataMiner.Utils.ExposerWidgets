namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
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
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public IntegerFilterSection(string filterName, Func<int, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
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
        /// Resets filter to default values.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            Value = 0;
        }

        /// <summary>
        /// Generates UI section of numeric filter.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterContentNumeric, 0, 1);
        }

        /// <summary>
        /// Handles default update of filter.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterContentNumeric.IsEnabled = !IsDefault;
        }
    }
}