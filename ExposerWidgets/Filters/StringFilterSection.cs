﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one string input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class StringFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, string>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// String input widget.
        /// </summary>
        protected readonly TextBox filterContentTextBox = new TextBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="StringFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="emptyFilter">Filter that will be applied.</param>
		/// <param name="invertedEmptyFilter">Optional inverted filter.</param>
		public StringFilterSection(string filterName, Func<string, FilterElement<DataMinerObjectType>> emptyFilter, Func<string, FilterElement<DataMinerObjectType>> invertedEmptyFilter = null) : base(filterName, emptyFilter, invertedEmptyFilter)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided string value is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets provided filter value.
        /// </summary>
        public override string Value
        {
            get => filterContentTextBox.Text;
            set => filterContentTextBox.Text = value;
        }

        /// <summary>
        /// Resets filter values to default.
        /// </summary>
        public override void Reset()
        {
            IsIncluded = false;
            Value = string.Empty;
        }

        /// <summary>
        /// Generates filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterContentTextBox, 0, nextAvailableColumn++);
        }

        /// <summary>
        /// Handles filter section default updates.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterContentTextBox.IsEnabled = !IsDefault;
        }
    }
}