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
		public StringStringFilterSection(string filterName, Dictionary<Comparers, Func<string, string, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
        {
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
        /// Resets filter values to default.
        /// </summary>
        public override void Reset()
        {
            IsIncluded = false;
            FirstValue = string.Empty;
            SecondValue = string.Empty;
        }

        /// <summary>
        /// Generates filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            if (!string.IsNullOrWhiteSpace(firstValueExplanationLabel.Text)) AddWidget(firstValueExplanationLabel, 0, nextAvailableColumn++);
            AddWidget(firstTextBox, 0, nextAvailableColumn++);
			if (!string.IsNullOrWhiteSpace(secondValueExplanationLabel.Text)) AddWidget(secondValueExplanationLabel, 0, nextAvailableColumn++);
			AddWidget(secondTextBox, 0, nextAvailableColumn++);
        }

        /// <summary>
        /// Handles filter section default updates.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            firstTextBox.IsEnabled = !IsDefault;
            secondTextBox.IsEnabled = !IsDefault;
        }
    }
}
