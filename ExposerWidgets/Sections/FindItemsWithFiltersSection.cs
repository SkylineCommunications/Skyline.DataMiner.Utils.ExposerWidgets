namespace Skyline.DataMiner.Utils.YLE.UI.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Label = InteractiveAutomationScript.Label;

    /// <summary>
    /// Section for selecting base info about filtering.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public abstract class FindItemsWithFiltersSection<DataMinerObjectType> : Section
    {
        private readonly Label header = new Label($"Find {typeof(DataMinerObjectType).Name}s with filters") { Style = TextStyle.Heading };

        private readonly Button getItemsBasedOnFiltersButton = new Button($"Find {typeof(DataMinerObjectType).Name}s Based on Filters") { Style = ButtonStyle.CallToAction, Width = 300 };
        private List<DataMinerObjectType> itemsBasedOnFilters = new List<DataMinerObjectType>();

        private readonly Label selectedItemsHeader = new Label("Filter Results") { Style = TextStyle.Heading };
        private readonly Label amountOfMatchingItemsLabel = new Label(string.Empty);
        private readonly Label amountOfSelectedItemsLabel = new Label(string.Empty);
        private List<CheckBox> selectItemsCheckBoxList = new List<CheckBox>();
        private readonly Button selectAllButton = new Button("Select All") { Width = 100, IsVisible = false };
        private readonly Button unselectAllButton = new Button("Unselect All") { Width = 100, IsVisible = false };

        /// <summary>
        /// Initializes a new instance of the <see cref="FindItemsWithFiltersSection{T}"/>"/> class.
        /// </summary>
        protected FindItemsWithFiltersSection()
        {
            getItemsBasedOnFiltersButton.Pressed += (s, e) => SelectedItems = GetItemsBasedOnFilters();

            selectAllButton.Pressed += (o, e) =>
            {
                selectItemsCheckBoxList.ForEach(x => x.IsChecked = true);
                SelectedItems = GetIndividuallySelectedItems();
            };

            unselectAllButton.Pressed += (o, e) =>
            {
                selectItemsCheckBoxList.ForEach(x => x.IsChecked = false);
                SelectedItems = GetIndividuallySelectedItems();
            };
        }

        /// <summary>
        /// Event triggered when we need to regenerate UI.
        /// </summary>
        public event EventHandler RegenerateUiRequired;

        /// <summary>
        /// Gets list of selected DataMiner objects.
        /// </summary>
        public IEnumerable<DataMinerObjectType> SelectedItems { get; private set; } = new List<DataMinerObjectType>();

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
        protected abstract void RegenerateFilterSections();

        /// <summary>
        /// Resets all filters to default value.
        /// </summary>
        public void ResetFiltersAndSelectedItems()
        {
            ResetFilters();
            SelectedItems = GetItemsBasedOnFilters();
        }

        /// <summary>
        /// Resets all filters in section.
        /// </summary>
        protected abstract void ResetFilters();

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
        protected abstract string GetItemIdentifier(DataMinerObjectType item);

        private IEnumerable<DataMinerObjectType> GetItemsBasedOnFilters()
        {           
            if (!OneOrMoreFiltersAreActive() || !ActiveFiltersAreValid())
            {
                itemsBasedOnFilters = new List<DataMinerObjectType>();
            }
            else
            {
                itemsBasedOnFilters = FindItemsWithFilters().ToList();
            }

            amountOfMatchingItemsLabel.Text = $"Found {itemsBasedOnFilters.Count} {typeof(DataMinerObjectType).Name}s matching the filters";

			selectItemsCheckBoxList = itemsBasedOnFilters.Select(r => GetItemIdentifier(r)).OrderBy(name => name).Select(name => new CheckBox(name) { IsChecked = true }).ToList();
			selectItemsCheckBoxList.ForEach(x => x.Changed += (o, e) => SelectedItems = GetIndividuallySelectedItems());

            var selectedItems = GetIndividuallySelectedItems();

            selectAllButton.IsVisible = selectedItems.Any();
            unselectAllButton.IsVisible = selectedItems.Any();

            InvokeRegenerateUi();

            return selectedItems;       
        }

        private IEnumerable<DataMinerObjectType> GetIndividuallySelectedItems()
        {
            var selectedResourceNames = selectItemsCheckBoxList.Where(x => x.IsChecked).Select(x => x.Text);

            var selectedResources = itemsBasedOnFilters.Where(r => selectedResourceNames.Contains(GetItemIdentifier(r))).ToList();

            amountOfSelectedItemsLabel.Text = $"Selected {selectedResources.Count} {typeof(DataMinerObjectType).Name}s";

            return selectedResources;
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
            int topRow = row;

            AddWidget(header, ++row, 0, 1, 5);

            AddFilterSections(ref row, out int firstAvailablecolumn);

            AddWidget(new Label("          "), row, firstAvailablecolumn++);

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(getItemsBasedOnFiltersButton, ++row, 0, 1, 5);

            AddWidget(new WhiteSpace(), row + 1, 0);

            AddResultWidgets(topRow, firstAvailablecolumn);
        }

        private void AddResultWidgets(int row, int column)
        {
			AddWidget(selectedItemsHeader, ++row, ++column, 1, 5);
			AddWidget(amountOfMatchingItemsLabel, ++row, column, 1, 2);
			AddWidget(selectAllButton, ++row, column);
			AddWidget(unselectAllButton, row, column + 1);
			AddWidget(amountOfSelectedItemsLabel, ++row, column, 1, 2);

			foreach (var selectedItemCheckBox in selectItemsCheckBoxList)
			{
				AddWidget(selectedItemCheckBox, ++row, column);
			}
		}

        /// <summary>
        /// Method that triggers UI regeneration evenet.
        /// </summary>
        protected void InvokeRegenerateUi()
        {
            RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
        }
    }
}
