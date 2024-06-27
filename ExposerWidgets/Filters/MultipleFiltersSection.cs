namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// A class to manage multiple of the same filter sections.
    /// </summary>
    /// <typeparam name="DataMinerObjectType"></typeparam>
    public class MultipleFiltersSection<DataMinerObjectType> : Section, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private readonly List<FilterSectionBase<DataMinerObjectType>> filterSections = new List<FilterSectionBase<DataMinerObjectType>>();

        private readonly Button duplicateFilterButton = new Button("Duplicate") { Width = 100 };

		/// <summary>
		/// Initializes an instance of the <see cref="MultipleFiltersSection{DataMinerObjectType}"/> class.
		/// </summary>
		/// <param name="filterSectionBase"></param>
		public MultipleFiltersSection(FilterSectionBase<DataMinerObjectType> filterSectionBase)
        {
            filterSections.Add(filterSectionBase);

            duplicateFilterButton.Pressed += AddMoreFiltersButton_Pressed;

            GenerateUi();
        }

        /// <summary>
        /// The FilterElement consisting of all individual filters combined with an AND operation.
        /// </summary>
        public FilterElement<DataMinerObjectType> FilterElement => new ANDFilterElement<DataMinerObjectType>(filterSections.Where(filter => filter.IsIncluded).Select(filter => filter.FilterElement).ToArray());

        /// <summary>
        /// A boolean indicating if the filter is included by the user.
        /// </summary>
        public bool IsIncluded => filterSections.Any(f => f.IsIncluded);

        /// <summary>
        /// A boolean indicating if the filter is valid.
        /// </summary>
        public bool IsValid => filterSections.Where(f => f.IsIncluded).Any(f => f.IsValid);

        /// <summary>
        /// An event raised when widgets should be added to or removed from the dialog.
        /// </summary>
        public event EventHandler RegenerateUiRequired;

        /// <summary>
        /// Re-adds the widgets to the section.
        /// </summary>
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
            AddWidget(duplicateFilterButton, row, filterSections.First().ColumnCount);
            row += filterSections.First().RowCount;

            foreach (var filterSection in filterSections.Skip(1))
            {
                AddSection(filterSection, new SectionLayout(row, 0));
                row += filterSection.RowCount;
            }
        }
    }
}
