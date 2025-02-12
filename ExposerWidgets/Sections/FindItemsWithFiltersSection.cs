namespace Skyline.DataMiner.Utils.YLE.UI.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.ExposerWidgets.Sections;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Section for selecting base info about filtering.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
	public class FindItemsWithFiltersSection<DataMinerObjectType> : FindItemsWithFiltersSectionBase
    {
		private readonly CollapseButton collapseButton = new CollapseButton() { CollapseText = "-", ExpandText = "+", Width = 44 };
		private readonly Label header = new Label($"Find {typeof(DataMinerObjectType).Name}s with filters") { Style = TextStyle.Title };

		private readonly List<SectionContainingDataMinerObjectFilters<DataMinerObjectType>> sectionsContainingFilters = new List<SectionContainingDataMinerObjectFilters<DataMinerObjectType>>();

		private readonly Button addOrFilterButton = new Button("Add OR Filter");

		private readonly Button countItemsBasedOnFiltersButton = new Button($"Count {typeof(DataMinerObjectType).Name}s Based on Filters") { Width = 300 };
        private readonly Button findItemsBasedOnFiltersButton = new Button($"Find {typeof(DataMinerObjectType).Name}s Based on Filters") { Style = ButtonStyle.CallToAction, Width = 300 };

        private readonly ResultsSection<DataMinerObjectType> resultsSection;
		private IDataMinerObjectFinder<DataMinerObjectType> dataMinerObjectFinder;

		/// <summary>
		/// 
		/// </summary>
		public FindItemsWithFiltersSection()
		{
			collapseButton.Pressed += (s, e) => SetWidgetsVisibility(!collapseButton.IsCollapsed);

			addOrFilterButton.Pressed += AddOrFilterButton_Pressed;

			countItemsBasedOnFiltersButton.Pressed += (s, e) =>
			{
				collapseButton.IsCollapsed = true;
				SetWidgetsVisibility(!collapseButton.IsCollapsed);
				resultsSection.LoadNewCount(dataMinerObjectFinder.CountObjects(GetCombinedFilterElement(allowNoActiveFilter: true)));
				InvokeDataMinerObjectsRetrievedBasedOnFilters();
			};

			findItemsBasedOnFiltersButton.Pressed += (s, e) =>
			{
				collapseButton.IsCollapsed = true;
				SetWidgetsVisibility(!collapseButton.IsCollapsed);
				resultsSection.LoadNewItems(GetItemsBasedOnFilters());
				InvokeDataMinerObjectsRetrievedBasedOnFilters();
			};

			resultsSection = new ResultsSection<DataMinerObjectType>((DataMinerObjectType obj) => dataMinerObjectFinder.IdentifyObject(obj));
			resultsSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
		}

		/// <summary>
		/// Gets list of selected DataMiner objects.
		/// </summary>
		public IEnumerable<DataMinerObjectType> SelectedItems => resultsSection.SelectedItems;

        /// <summary>
        /// Regenerates section UI.
        /// </summary>
        public override void RegenerateUi()
        {
			foreach (var section in sectionsContainingFilters)
			{
				section.RegenerateUi();
			}

			resultsSection.RegenerateUi();
            
            GenerateUi();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataMinerObjectFinder"></param>
		public void SetDataMinerObjectFinder(IDataMinerObjectFinder<DataMinerObjectType> dataMinerObjectFinder)
		{
			this.dataMinerObjectFinder = dataMinerObjectFinder;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newSectionContainingFilters"></param>
		public void AddNewSectionContainingFilters(SectionContainingDataMinerObjectFilters<DataMinerObjectType> newSectionContainingFilters)
		{
			newSectionContainingFilters.RegenerateUiRequired += (o, e2) => InvokeRegenerateUi();

			newSectionContainingFilters.Deleted += (o, e2) =>
			{
				sectionsContainingFilters.Remove(newSectionContainingFilters);
				InvokeRegenerateUi();
			};

			sectionsContainingFilters.Add(newSectionContainingFilters);
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveAllSectionsContainingFilters()
		{
			sectionsContainingFilters.Clear();
		}

		private IEnumerable<DataMinerObjectType> GetItemsBasedOnFilters()
        {           
            bool oneOrMoreFiltersAreIncluded = sectionsContainingFilters.Any(s => s.IsIncluded);
            bool includedFiltersAreValid = sectionsContainingFilters.Where(s => s.IsIncluded).All(s => s.IsValid);

            if (!oneOrMoreFiltersAreIncluded || !includedFiltersAreValid)
            {
                return new List<DataMinerObjectType>();
            }
            else if(TryGetCombinedFilterElement(out var filterElement))
            {
				return dataMinerObjectFinder.FindObjects(filterElement).ToList();
            }
            else
            {
				return new List<DataMinerObjectType>();
			}
        }

        private bool TryGetCombinedFilterElement(out FilterElement<DataMinerObjectType> filterElement, bool allowNoActiveFilter = false)
        {
            try
            {
                filterElement = GetCombinedFilterElement(allowNoActiveFilter);

                return filterElement != null;
            }
            catch (Exception)
            {
                filterElement = null;
                return false;
            }
        }

		/// <summary>
		/// Method that gets combined filter based on input values of active filters.
		/// </summary>
		/// <returns>Combined filter.</returns>
		/// <exception cref="InvalidOperationException">If there isn't any active filter.</exception>
		private FilterElement<DataMinerObjectType> GetCombinedFilterElement(bool allowNoActiveFilter = false)
        {
            var includedAndFilters = sectionsContainingFilters.Where(filter => filter.IsIncluded).Select(filter => filter.FilterElement);

            if (!includedAndFilters.Any())
            {
                if (allowNoActiveFilter)
                {
                    return new ANDFilterElement<DataMinerObjectType>();
                }
                else
                {
					throw new InvalidOperationException("Unable to find any active filters");
				}
			}

            return new ORFilterElement<DataMinerObjectType>(includedAndFilters.ToArray());
        }

        /// <summary>
        /// Generates section UI.
        /// </summary>
        protected void GenerateUi()
        {
            Clear();

            int row = 0;

            GenerateUi(ref row);
        }

		protected virtual void AddWidgetsBeforeFilters(ref int row)
		{
			// Override to implement
		}

        /// <summary>
        /// Generates section UI.
        /// </summary>
        /// <param name="row"></param>
        protected void GenerateUi(ref int row)
        {
            AddWidget(collapseButton, ++row, 0);
            AddWidget(header, row, 1, 1, 4);

			AddWidgetsBeforeFilters(ref row);

			foreach (var filtersSection in sectionsContainingFilters)
			{
				AddSection(filtersSection, new SectionLayout(++row, 0));
				row += filtersSection.RowCount;
			}

			AddWidget(addOrFilterButton, ++row, 0, 1, 5);

			AddWidget(new WhiteSpace(), ++row, 0);

            if (dataMinerObjectFinder.SupportsCountingObjects)
            {
				AddWidget(countItemsBasedOnFiltersButton, ++row, 0, 1, 5);
			}

			AddWidget(findItemsBasedOnFiltersButton, ++row, 0, 1, 5);

			AddSection(resultsSection, new SectionLayout(++row, 0));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isVisible"></param>
		protected virtual void SetWidgetsVisibility(bool isVisible)
		{
			foreach (var section in sectionsContainingFilters)
			{
				section.IsVisible = isVisible;
			}

            countItemsBasedOnFiltersButton.IsVisible = isVisible;
            findItemsBasedOnFiltersButton.IsVisible = isVisible;
		}

		private void AddOrFilterButton_Pressed(object sender, EventArgs e)
		{
			var newSectionContainingFilters = sectionsContainingFilters.First().Clone();

			AddNewSectionContainingFilters(newSectionContainingFilters);
		}
	}
}
