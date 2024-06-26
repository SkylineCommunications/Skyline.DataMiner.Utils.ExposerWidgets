﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Ticketing;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    ///  Represents filter section with custom name property and two inputs for custom property filtering. First input is string type, second is numeric type.
    /// </summary>
    public class TicketEnumFilterSection : FilterSectionThreeInputs<Ticket, string, string, int>, IDataMinerObjectFilter<Ticket>
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
		/// Initializes a new instance of the <see cref="TicketEnumFilterSection"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		/// <param name="firstExplanationLabel"></param>
		/// <param name="secondExplanationLabel"></param>
		/// <param name="tooltip"></param>
		public TicketEnumFilterSection(string filterName, Dictionary<Comparers, Func<string, string, int, FilterElement<Ticket>>> filterFunctions, string firstExplanationLabel = null, string secondExplanationLabel = null, string tooltip = null) : base(filterName, filterFunctions, tooltip)
        {
            propertyNameTextBox.PlaceHolder = firstExplanationLabel ?? string.Empty;
			propertyValueTextBox.PlaceHolder = secondExplanationLabel ?? string.Empty;

            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected TicketEnumFilterSection(TicketEnumFilterSection other) : base(other)
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
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => propertyNameTextBox;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => propertyValueTextBox;

		/// <summary>
		/// The third widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget ThirdInputWidget => propertyValueNumeric;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<Ticket> Clone()
		{
            return new TicketEnumFilterSection(this);
		}
    }
}