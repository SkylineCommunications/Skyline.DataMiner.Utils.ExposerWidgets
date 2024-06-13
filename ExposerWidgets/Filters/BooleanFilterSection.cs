namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one checkbox input.
    /// </summary>
    /// <typeparam name="T">Type of filtered object.</typeparam>
    public class BooleanFilterSection<T> : FilterSectionOneInput<T, bool>, IDataMinerObjectFilter<T>
    {
        private readonly CheckBox filterChecbox = new CheckBox();

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanFilterSection{T}"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public BooleanFilterSection(string filterName, Func<bool, FilterElement<T>> emptyFilter) : base(filterName, emptyFilter)
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
            get => filterChecbox.IsChecked;
            set => filterChecbox.IsChecked = value;
        }

        /// <summary>
        /// Sets value to unchecked and filter as non active.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            Value = false;
        }

        /// <summary>
        /// Handles default update of checkbox filter.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterChecbox.IsEnabled = !IsDefault;
        }

        /// <summary>
        /// Generates UI of checkbox filter section.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterChecbox, 0, 1);
        }
    }
}
