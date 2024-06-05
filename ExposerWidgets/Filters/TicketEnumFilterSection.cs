namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    ///  Represents filter section with custom name property and two inputs for custom property filtering. First input is string type, second is numeric type.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class TicketEnumFilterSection<DataMinerObjectType> : FilterSectionThreeInputs<DataMinerObjectType, string, string, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Custom property input widget.
        /// </summary>
        protected readonly TextBox propertyNameTextBox = new TextBox();

        /// <summary>
        /// String value of custom property input widget.
        /// </summary>
        protected readonly TextBox propertyValueTextBox = new TextBox();

        /// <summary>
        /// Numeric value of custom property input widget.
        /// </summary>
        protected readonly Numeric propertyValueNumeric = new Numeric { Decimals = 0, StepSize = 1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketEnumFilterSection{T}"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public TicketEnumFilterSection(string filterName, Func<string, string, int, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
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
        /// Gets or sets numeric filter value for custom property.
        /// </summary>
        public override int ThirdValue
        {
            get => (int)propertyValueNumeric.Value;
            set => propertyValueNumeric.Value = value;
        }

        /// <summary>
        /// Resets filter values to default.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            FirstValue = string.Empty;
            SecondValue = string.Empty;
            ThirdValue = 0;
        }

        /// <summary>
        /// Generates filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(propertyNameTextBox, 0, 1);
            AddWidget(propertyValueTextBox, 0, 2);
            AddWidget(propertyValueNumeric, 1, 2);
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
            propertyValueNumeric.IsEnabled = !IsDefault;
        }
    }
}