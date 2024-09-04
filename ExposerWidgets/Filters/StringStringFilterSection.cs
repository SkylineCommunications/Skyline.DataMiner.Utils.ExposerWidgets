namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with custom name property and string input value of custom property.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class StringStringFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, string, string>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private readonly TextBox firstTextBox = new TextBox();

		private readonly TextBox secondTextBox = new TextBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="StringStringFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		/// <param name="firstValueExplanation"></param>
		/// <param name="secondValueExplanation"></param>
		public StringStringFilterSection(string filterName, Dictionary<Comparers, Func<string, string, FilterElement<DataMinerObjectType>>> filterFunctions, string firstValueExplanation = null, string secondValueExplanation = null) : base(filterName, filterFunctions)
        {
            firstTextBox.PlaceHolder = firstValueExplanation ?? string.Empty;
            firstTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;
            
            secondTextBox.PlaceHolder = secondValueExplanation ?? string.Empty;
			secondTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected StringStringFilterSection(StringStringFilterSection<DataMinerObjectType> other) : base(other)
        {
            firstTextBox.PlaceHolder = other.firstTextBox.PlaceHolder;
            secondTextBox.PlaceHolder = other.secondTextBox.PlaceHolder;

            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided custom property value is not null or empty.
        /// </summary>
        public override bool IsValid => !string.IsNullOrEmpty(firstTextBox.Text);

        /// <summary>
        /// Gets or sets custom property name value.
        /// </summary>
        public override string FirstValue
        {
            get => firstTextBox.Text;
            set => firstTextBox.Text = value;
        }

        /// <summary>
        /// Gets or sets string filter value for custom property.
        /// </summary>
        public override string SecondValue
        {
            get => secondTextBox.Text;
            set => secondTextBox.Text = value;
        }

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => firstTextBox;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => secondTextBox;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new StringStringFilterSection<DataMinerObjectType>(this);
		}
    }
}
