namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	public class GuidStringFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, Guid, string>, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly TextBox firstTextBox = new TextBox();

		private readonly TextBox secondTextBox = new TextBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidStringFilterSection{DataMinerObjectType}"/>"/> class.
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="firstValueExplanation"></param>
		/// <param name="secondValueExplanation"></param>
		/// <param name="tooltip"></param>
		public GuidStringFilterSection(string filterName, Dictionary<Comparers, Func<Guid, string, FilterElement<DataMinerObjectType>>> filterFunctions, string firstValueExplanation = null, string secondValueExplanation = null, string tooltip = null) : base(filterName, filterFunctions, tooltip)
		{
			firstTextBox.PlaceHolder = firstValueExplanation ?? string.Empty;
			firstTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			secondTextBox.PlaceHolder = secondValueExplanation ?? string.Empty;
			secondTextBox.FocusLost += (s, e) => isIncludedCheckBox.IsChecked = true;

			GenerateUi();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		protected GuidStringFilterSection(GuidStringFilterSection<DataMinerObjectType> other) : base(other)
		{
			firstTextBox.PlaceHolder = other.firstTextBox.PlaceHolder;
			secondTextBox.PlaceHolder = other.secondTextBox.PlaceHolder;

			GenerateUi();
		}

		/// <summary>
		/// Indicates if provided guid is valid or not.
		/// </summary>
		public override bool IsValid
		{
			get
			{
				bool valid = Guid.TryParse(firstTextBox.Text, out _);

				firstTextBox.ValidationState = valid ? Automation.UIValidationState.Valid : Automation.UIValidationState.Invalid;
				firstTextBox.ValidationText = $"Provide a valid {nameof(Guid)}";

				return valid;
			}
		}

		/// <summary>
		/// Gets or sets custom property name value.
		/// </summary>
		public override Guid FirstValue
		{
			get => Guid.Parse(firstTextBox.Text);
			set => firstTextBox.Text = value.ToString();
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
		/// The first widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget FirstInputWidget => firstTextBox;

		/// <summary>
		/// The second widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget SecondInputWidget => secondTextBox;

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new GuidStringFilterSection<DataMinerObjectType>(this);
		}
	}
}
