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
            dateTimePicker.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected DateTimeFilterSection(DateTimeFilterSection<DataMinerObjectType> other) : base(other)
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
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget InputWidget => dateTimePicker;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns>A clone of the current instance. Values that we're set by user interaction are cleared.</returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new DateTimeFilterSection<DataMinerObjectType>(this);
		}
    }
}