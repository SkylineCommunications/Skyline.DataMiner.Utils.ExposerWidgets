namespace Skyline.DataMiner.Utils.YLE.UI.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.ExposerWidgets.Sections;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Section for selecting base info about filtering.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
	public abstract class FindItemsWithFiltersSection<DataMinerObjectType> : Section
    {
		private readonly CollapseButton collapseButton = new CollapseButton() { CollapseText = "-", ExpandText = "+", Width = 44 };
		private readonly Label header = new Label($"Find {typeof(DataMinerObjectType).Name}s with filters") { Style = TextStyle.Title };

        private readonly Button findItemsBasedOnFiltersButton = new Button($"Find {typeof(DataMinerObjectType).Name}s Based on Filters") { Style = ButtonStyle.CallToAction, Width = 300 };

        private readonly ResultsSection<DataMinerObjectType> resultsSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindItemsWithFiltersSection{T}"/>"/> class.
        /// </summary>
        protected FindItemsWithFiltersSection()
        {
            resultsSection = new ResultsSection<DataMinerObjectType>((DataMinerObjectType obj) => IdentifyItem(obj));
			resultsSection.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();

            collapseButton.Pressed += (s, e) => UpdateWidgetsVisibility();

            findItemsBasedOnFiltersButton.Pressed += (s, e) =>
            {
                collapseButton.IsCollapsed = true;
                UpdateWidgetsVisibility();
				resultsSection.LoadNewItems(GetItemsBasedOnFilters());
                DataMinerObjectsRetrievedBasedOnFilters?.Invoke(this, EventArgs.Empty);
            };      
        }

        /// <summary>
        /// Event triggered when we need to regenerate UI.
        /// </summary>
        public event EventHandler RegenerateUiRequired;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler DataMinerObjectsRetrievedBasedOnFilters;

        /// <summary>
        /// Gets list of selected DataMiner objects.
        /// </summary>
        public IEnumerable<DataMinerObjectType> SelectedItems => resultsSection.SelectedItems;

        /// <summary>
        /// Regenerates section UI.
        /// </summary>
        public void RegenerateUi()
        {
            RegenerateFilterSections();
            GenerateUi();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RegenerateFilterSections()
        {
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUi();
			}

			resultsSection.RegenerateUi();
		}

        /// <summary>
        /// Resets all filters to default value.
        /// </summary>
        public void ResetFiltersAndSelectedItems()
        {
            ResetFilters();
			resultsSection.LoadNewItems(GetItemsBasedOnFilters());
			DataMinerObjectsRetrievedBasedOnFilters?.Invoke(this, EventArgs.Empty);
		}

        /// <summary>
        /// Resets all filters in section.
        /// </summary>
        protected virtual void ResetFilters()
        {
			foreach (var section in GetMultipleFiltersSections())
			{
				section.Reset();
			}
		}

        /// <summary>
        /// Finding all objects based on provided filters.
        /// </summary>
        /// <returns>Collection of filtered objects.</returns>
        protected abstract IEnumerable<DataMinerObjectType> FindItemsWithFilters();

        /// <summary>
        /// Gets ID of object.
        /// </summary>
        /// <param name="item">Object for which ID is being retrieved.</param>
        /// <returns>ID of an object.</returns>
        protected abstract string IdentifyItem(DataMinerObjectType item);

        private IEnumerable<DataMinerObjectType> GetItemsBasedOnFilters()
        {           
            if (!OneOrMoreFiltersAreActive() || !ActiveFiltersAreValid())
            {
                return new List<DataMinerObjectType>();
            }
            else
            {
                return FindItemsWithFilters().ToList();
            }
        }

		/// <summary>
		/// Adding filter section to UI.
		/// </summary>
		/// <param name="row">Row on which we want to add section.</param>
		/// <param name="firstAvailableColumn"></param>
		protected abstract void AddFilterSections(ref int row, out int firstAvailableColumn);

        /// <summary>
        /// Gets collection of individual filters in section.
        /// </summary>
        /// <returns>Collection of individual filters.</returns>
        protected IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>> GetIndividualFilters()
        {
            var fieldValues = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(field => field.GetValue(this)).ToList();

            var fieldsImplementingInterface = fieldValues.OfType<IDataMinerObjectFilter<DataMinerObjectType>>().ToList();

            var fieldsContainingCollectionOfInterface = fieldValues.OfType<IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>>>().SelectMany(collection => collection).ToList();

            var filters = fieldsImplementingInterface.Concat(fieldsContainingCollectionOfInterface).ToList();

            return filters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected IEnumerable<MultipleFiltersSection<DataMinerObjectType>> GetMultipleFiltersSections()
		{
			var fieldValues = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(field => field.GetValue(this)).ToList();

			return fieldValues.OfType<MultipleFiltersSection<DataMinerObjectType>>().ToList();
		}

		/// <summary>
		/// Checks if all active filters are valid.
		/// </summary>
		/// <returns>True if all active filters are valid, false if one or more of them isn't.</returns>
		protected bool ActiveFiltersAreValid()
        {
            var individualFilters = GetIndividualFilters();

            if (!individualFilters.Any()) return true;

            return individualFilters.Where(filter => filter.IsIncluded).All(filter => filter.IsValid);
        }

        /// <summary>
        /// Check if one or more filters are active.
        /// </summary>
        /// <returns>True if one or more filters are active, false if there isn't any active filter.</returns>
        protected bool OneOrMoreFiltersAreActive()
        {
            var individualFilters = GetIndividualFilters();

            return individualFilters.Any(filter => filter.IsIncluded);
        }

        /// <summary>
        /// Method that tries to get combined filter based on input values of active filters.
        /// </summary>
        /// <param name="filter">Combined filter.</param>
        /// <returns>True if successful.</returns>
        protected bool TryGetCombinedFilterElement(out ANDFilterElement<DataMinerObjectType> filter)
        {
            try
            {
                filter = GetCombinedFilterElement();
                return true;
            }
            catch (Exception)
            {
                filter = null;
                return false;
            }
        }

        /// <summary>
        /// Method that gets combined filter based on input values of active filters.
        /// </summary>
        /// <returns>Combined filter.</returns>
        /// <exception cref="InvalidOperationException">If there isn't any active filter.</exception>
        protected ANDFilterElement<DataMinerObjectType> GetCombinedFilterElement()
        {
            var individualActiveFilterElements = GetIndividualFilters().Where(filter => filter.IsIncluded).Select(filter => filter.FilterElement);

            if (!individualActiveFilterElements.Any()) throw new InvalidOperationException("Unable to find any active filters");

            return new ANDFilterElement<DataMinerObjectType>(individualActiveFilterElements.ToArray());
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

        /// <summary>
        /// Generates section UI.
        /// </summary>
        /// <param name="row"></param>
        protected void GenerateUi(ref int row)
        {
            AddWidget(collapseButton, ++row, 0);
            AddWidget(header, row, 1, 1, 4);

            AddFilterSections(ref row, out int firstAvailablecolumn);

			AddWidget(new WhiteSpace(), row + 1, 0);

			AddWidget(findItemsBasedOnFiltersButton, ++row, 1, 1, 3, horizontalAlignment:HorizontalAlignment.Left);

			AddSection(ResultsSection, new SectionLayout(++row, 0));
		}

        /// <summary>
        /// Method that triggers UI regeneration evenet.
        /// </summary>
        protected void InvokeRegenerateUi()
        {
            RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
        }

		private void UpdateWidgetsVisibility()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.IsVisible = !collapseButton.IsCollapsed;
			}

            findItemsBasedOnFiltersButton.IsVisible = !collapseButton.IsCollapsed;
		}
	}
}
