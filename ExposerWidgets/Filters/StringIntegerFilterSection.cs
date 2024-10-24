﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with custom name property and numeric input value of custom property.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class StringIntegerFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, string, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Custom property input widget.
        /// </summary>
        protected readonly TextBox firstValueTextBox = new TextBox();

        /// <summary>
        /// Numeric value of custom property input widget.
        /// </summary>
        protected readonly Numeric secondValueNumeric = new Numeric { Decimals = 0, StepSize = 1 };

		/// <summary>
		/// Initializes a new instance of the <see cref="StringIntegerFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions"></param>
		/// <param name="firstValueExplanation"></param>
		public StringIntegerFilterSection(string filterName, Dictionary<Comparers, Func<string, int, FilterElement<DataMinerObjectType>>> filterFunctions, string firstValueExplanation = null) : base(filterName, filterFunctions)
        {
            firstValueTextBox.PlaceHolder = firstValueExplanation ?? string.Empty;
            firstValueTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

            secondValueNumeric.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected StringIntegerFilterSection(StringIntegerFilterSection<DataMinerObjectType> other) : base(other)
        {
			firstValueTextBox.PlaceHolder = other.firstValueTextBox.PlaceHolder;

			GenerateUi();
        }

        /// <summary>
        /// Indicates if provided custom property value is not null or empty.
        /// </summary>
        public override bool IsValid => !string.IsNullOrEmpty(firstValueTextBox.Text);

        /// <summary>
        /// Gets or sets custom property name value.
        /// </summary>
        public override string FirstValue
        {
            get => firstValueTextBox.Text;
            set => firstValueTextBox.Text = value;
        }

        /// <summary>
        /// Gets or sets numeric filter value.
        /// </summary>
        public override int SecondValue
        {
            get => (int)secondValueNumeric.Value;
            set => secondValueNumeric.Value = value;
        }

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => firstValueTextBox;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => secondValueNumeric;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new StringIntegerFilterSection<DataMinerObjectType>(this);
		}
    }
}
