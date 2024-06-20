namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
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
		/// <param name="emptyFilter">Filter that will be applied.</param>
		/// <param name="invertedEmptyFilter">Optional inverted filter.</param>
		/// <param name="firstValueExplanation"></param>
		/// <param name="secondValueExplanation"></param>
		public StringIntegerFilterSection(string filterName, Func<string, int, FilterElement<DataMinerObjectType>> emptyFilter, Func<string, int, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null, string firstValueExplanation = null, string secondValueExplanation = null) : base(filterName, emptyFilter, invertedEmptyFilter, firstValueExplanation, secondValueExplanation)
        {
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
        /// Resets filter values to default.
        /// </summary>
        public override void Reset()
        {
            IsIncluded = false;
            FirstValue = string.Empty;
            SecondValue = 0;
        }

        /// <summary>
        /// Generates filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

			if (!string.IsNullOrWhiteSpace(firstValueExplanationLabel.Text)) AddWidget(firstValueExplanationLabel, 0, nextAvailableColumn++);
			AddWidget(firstValueTextBox, 0, nextAvailableColumn++);
			if (!string.IsNullOrWhiteSpace(secondValueExplanationLabel.Text)) AddWidget(secondValueExplanationLabel, 0, nextAvailableColumn++);
			AddWidget(secondValueNumeric, 0, nextAvailableColumn++);
		}

        /// <summary>
        /// Handles filter section default updates.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            firstValueTextBox.IsEnabled = !IsDefault;
            secondValueNumeric.IsEnabled = !IsDefault;
        }
    }
}
