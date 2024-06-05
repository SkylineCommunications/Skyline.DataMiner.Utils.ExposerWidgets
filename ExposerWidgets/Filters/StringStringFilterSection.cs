namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with custom name property and string input value of custom property.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class StringStringFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, string, string>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Custom property name value input widget.
        /// </summary>
        protected readonly TextBox propertyNameTextBox = new TextBox();

        /// <summary>
        /// string value input widget for specified custom property.
        /// </summary>
        protected readonly TextBox propertyValueTextBox = new TextBox();

        /// <summary>
        /// Initializes a new instance of the <see cref="StringStringFilterSection{T}"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public StringStringFilterSection(string filterName, Func<string, string, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided custom property value is not null or empty.
        /// </summary>
        public override bool IsValid => !string.IsNullOrEmpty(propertyNameTextBox.Text);

        /// <summary>
        /// Gets or sets custom property name value.
        /// </summary>
        public override string FirstValue
        {
            get => propertyNameTextBox.Text;
            set => propertyNameTextBox.Text = value;
        }

        /// <summary>
        /// Gets or sets string filter value for custom property.
        /// </summary>
        public override string SecondValue
        {
            get => propertyValueTextBox.Text;
            set => propertyValueTextBox.Text = value;
        }

        /// <summary>
        /// Resets filter values to default.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            FirstValue = string.Empty;
            SecondValue = string.Empty;
        }

        /// <summary>
        /// Generates filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(propertyNameTextBox, 0, 1);
            AddWidget(propertyValueTextBox, 0, 2);
        }

        /// <summary>
        /// Handles filter section default updates.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            propertyNameTextBox.IsEnabled = !IsDefault;
            propertyValueTextBox.IsEnabled = !IsDefault;
        }
    }
}
