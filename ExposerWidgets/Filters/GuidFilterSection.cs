namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one Guid input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class GuidFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, Guid>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Textbox for Guid input.
        /// </summary>
        protected readonly TextBox filterContentTextBox = new TextBox();

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidFilterSection{T}"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        public GuidFilterSection(string filterName, Func<Guid, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided guid is valid or not.
        /// </summary>
        public override bool IsValid => Guid.TryParse(filterContentTextBox.Text, out _);

        /// <summary>
        /// Gets or sets guid filter value.
        /// </summary>
        public override Guid Value
        {
            get => Guid.Parse(filterContentTextBox.Text);
            set => filterContentTextBox.Text = value.ToString();
        }

        /// <summary>
        /// Resets filter to default value.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            Value = Guid.Empty;
        }

        /// <summary>
        /// Generates UI for Guid filter section.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterContentTextBox, 0, 1);
        }

        /// <summary>
        /// Handles default update of filter section.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterContentTextBox.IsEnabled = !IsDefault;
        }
    }
}