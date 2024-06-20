namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one date-time picker input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class DateTimeFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, DateTime>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private readonly DateTimePicker dateTimePicker = new DateTimePicker(DateTime.Now);

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter exposers that will be used.</param>
		public DateTimeFilterSection(string filterName, Dictionary<Comparers, Func<DateTime, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if filter is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets value of date-time picker.
        /// </summary>
        public override DateTime Value
        {
            get => dateTimePicker.DateTime;
            set => dateTimePicker.DateTime = value;
        }

        /// <summary>
        /// Sets value to current date-time and filter as non active.
        /// </summary>
        public override void Reset()
        {
            IsIncluded = false;
            Value = DateTime.Now;
        }

        /// <summary>
        /// Generates UI of date-time filter section.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(dateTimePicker, 0, nextAvailableColumn++);
        }

        /// <summary>
        /// Handles default update of date-time filter.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            dateTimePicker.IsEnabled = !IsDefault;
        }
    }
}