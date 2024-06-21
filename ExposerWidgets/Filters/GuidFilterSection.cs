namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
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
		/// Initializes a new instance of the <see cref="GuidFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		public GuidFilterSection(string filterName, Dictionary<Comparers, Func<Guid, FilterElement<DataMinerObjectType>>> filterFunctions) : base(filterName, filterFunctions)
        {
			GenerateUi();
		}

        /// <summary>
        /// 
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
        /// 
        /// </summary>
		protected override InteractiveWidget InputWidget => filterContentTextBox;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new GuidFilterSection<DataMinerObjectType>(this);
		}

		/// <summary>
		/// Resets filter to default value.
		/// </summary>
		public override void Reset()
        {
            IsIncluded = false;
            Value = Guid.Empty;
        }

        /// <summary>
        /// Handles default update of filter section.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterContentTextBox.IsEnabled = !IsDefault;
        }
    }
}