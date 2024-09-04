namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
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
        protected readonly TextBox filterContentTextBox = new TextBox() { PlaceHolder = nameof(Guid) };

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidFilterSection{DataMinerObjectType}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		public GuidFilterSection(string filterName, Dictionary<Comparers, Func<Guid, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
        {
			filterContentTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected GuidFilterSection(GuidFilterSection<DataMinerObjectType> other) : base(other)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided guid is valid or not.
        /// </summary>
        public override bool IsValid 
        {
            get
            {
				bool valid = Guid.TryParse(filterContentTextBox.Text, out _);

                filterContentTextBox.ValidationState = valid ? Automation.UIValidationState.Valid : Automation.UIValidationState.Invalid;
                filterContentTextBox.ValidationText = $"Provide a valid {nameof(Guid)}";

                return valid;
			}
		}

        /// <summary>
        /// Gets or sets guid filter value.
        /// </summary>
        public override Guid Value
        {
            get => Guid.Parse(filterContentTextBox.Text);
            set => filterContentTextBox.Text = value.ToString();
        }

		/// <summary>
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget InputWidget => filterContentTextBox;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns>A clone of the current instance.</returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new GuidFilterSection<DataMinerObjectType>(this);
		}
    }
}