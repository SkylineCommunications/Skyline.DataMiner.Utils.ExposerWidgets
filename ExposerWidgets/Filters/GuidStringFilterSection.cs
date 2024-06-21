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
		/// 
		/// </summary>
		/// <param name="filterName"></param>
		/// <param name="filterFunctions"></param>
		/// <param name="firstValueExplanation"></param>
		/// <param name="secondValueExplanation"></param>
		public GuidStringFilterSection(string filterName, Dictionary<Comparers, Func<Guid, string, FilterElement<DataMinerObjectType>>> filterFunctions, string firstValueExplanation = null, string secondValueExplanation = null) : base(filterName, filterFunctions)
		{
			firstTextBox.PlaceHolder = firstValueExplanation ?? string.Empty;
			secondTextBox.PlaceHolder = secondValueExplanation ?? string.Empty;

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
		public override bool IsValid => Guid.TryParse(firstTextBox.Text, out _);

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
		/// 
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
			return new GuidStringFilterSection<DataMinerObjectType>(this);
		}

		/// <summary>
		/// Resets filter values to default.
		/// </summary>
		public override void Reset()
		{
			IsIncluded = false;
			FirstValue = Guid.Empty;
			SecondValue = string.Empty;
		}

		/// <summary>
		/// Generates filter section UI.
		/// </summary>
		protected override void GenerateUi()
		{
			base.GenerateUi();

			AddWidget(firstTextBox, 0, nextAvailableColumn++);
			AddWidget(secondTextBox, 0, nextAvailableColumn++);
		}

		/// <summary>
		/// Handles filter section default updates.
		/// </summary>
		protected override void HandleDefaultUpdate()
		{
			firstTextBox.IsEnabled = !IsDefault;
			secondTextBox.IsEnabled = !IsDefault;
		}
	}
}
