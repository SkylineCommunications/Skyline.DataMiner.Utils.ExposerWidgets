namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Skyline.DataMiner.Net.Dialogs;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	public class MultipleFiltersSection<DataMinerObjectType> : Section, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly List<FilterSectionBase<DataMinerObjectType>> filterSections = new List<FilterSectionBase<DataMinerObjectType>>();

		private readonly Button addMoreFiltersButton = new Button("+ Add More");

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterSectionBase"></param>
		public MultipleFiltersSection(FilterSectionBase<DataMinerObjectType> filterSectionBase)
		{
			filterSections.Add(filterSectionBase);

			addMoreFiltersButton.Pressed += AddMoreFiltersButton_Pressed;

			GenerateUi();
		}

		/// <summary>
		/// 
		/// </summary>
		public FilterElement<DataMinerObjectType> FilterElement => new ANDFilterElement<DataMinerObjectType>(filterSections.Where(filter => filter.IsIncluded).Select(filter => filter.FilterElement).ToArray());

		/// <summary>
		/// 
		/// </summary>
		public bool IsIncluded => filterSections.Any(f => f.IsIncluded);

		/// <summary>
		/// 
		/// </summary>
		public bool IsValid => filterSections.Any(f => f.IsValid);

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler RegenerateUiRequired;

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public void Reset()
		{
			var newFilterSection = filterSections.First().Clone();

			filterSections.Clear();

			filterSections.Add(newFilterSection);

			RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
		}

		public void RegenerateUi()
		{
			GenerateUi();
		}

		private void AddMoreFiltersButton_Pressed(object sender, EventArgs e)
		{
			filterSections.Add(filterSections.First().Clone());

			RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
		}

		private void GenerateUi()
		{
			Clear();
			int row = -1;

			AddSection(filterSections.First(), new SectionLayout(++row, 0));
			AddWidget(addMoreFiltersButton, row, filterSections.First().ColumnCount);
			row += filterSections.First().RowCount;

			foreach (var filterSection in filterSections.Skip(1))
			{
				AddSection(filterSection, new SectionLayout(++row, 0));
				row += filterSection.RowCount;
			}
		}
	}
}
