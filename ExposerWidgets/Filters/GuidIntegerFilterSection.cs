namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	///  Represents filter section with a Guid input and a integer input.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
	public class GuidIntegerFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, Guid, int>, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly TextBox textbox = new TextBox();

		private readonly Numeric numeric = new Numeric(0) { StepSize = 1, Decimals = 0 };

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidStringFilterSection{DataMinerObjectType}"/>"/> class.
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="firstValueExplanation"></param>
		/// <param name="tooltip"></param>
		public GuidIntegerFilterSection(string filterName, Dictionary<Comparers, Func<Guid, int, FilterElement<DataMinerObjectType>>> filterFunctions, string firstValueExplanation = null, string tooltip = null) : base(filterName, filterFunctions, tooltip)
		{
			textbox.PlaceHolder = firstValueExplanation ?? string.Empty;
			textbox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			numeric.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected GuidIntegerFilterSection(GuidIntegerFilterSection<DataMinerObjectType> other) : base(other)
		{
			textbox.PlaceHolder = other.textbox.PlaceHolder;

			GenerateUi();
		}

		/// <summary>
		/// Indicates if provided guid is valid or not.
		/// </summary>
		public override bool IsValid
		{
			get
			{
				bool valid = Guid.TryParse(textbox.Text, out _);

				textbox.ValidationState = valid ? Automation.UIValidationState.Valid : Automation.UIValidationState.Invalid;
				textbox.ValidationText = $"Provide a valid {nameof(Guid)}";

				return valid;
			}
		}

		/// <summary>
		/// Gets or sets custom property name value.
		/// </summary>
		public override Guid FirstValue
		{
			get => Guid.Parse(textbox.Text);
			set => textbox.Text = value.ToString();
		}

		/// <summary>
		/// Gets or sets string filter value for custom property.
		/// </summary>
		public override int SecondValue
		{
			get => (int)numeric.Value;
			set => numeric.Value = value;
		}

		/// <summary>
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => textbox;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => numeric;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new GuidIntegerFilterSection<DataMinerObjectType>(this);
		}
	}
}
