namespace Skyline.DataMiner.Utils.YLE.UI.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly Label amountOfMatchingItemsLabel = new Label(string.Empty);
        private readonly Label amountOfSelectedItemsLabel = new Label(string.Empty);
        private readonly CheckBoxList selectItemsCheckBoxList = new CheckBoxList();
        private readonly Button selectAllButton = new Button("Select All");
        private readonly Button unselectAllButton = new Button("Unselect All");
        private readonly Numeric selectFirstXItemsNumeric = new Numeric { Decimals = 0, StepSize = 1, Minimum = 0 };
        private readonly Button selectFirstXItemsButton = new Button("Select First X");

        /// <summary>
        /// Initializes a new instance of the <see cref="FindItemsWithFiltersSection{T}"/>"/> class.
        /// </summary>
        protected FindItemsWithFiltersSection()
        {
            getItemsBasedOnFiltersButton.Pressed += (s, e) => SelectedItems = GetItemsBasedOnFilters();

            selectAllButton.Pressed += (o, e) =>
            {
                selectItemsCheckBoxList.CheckAll();
                SelectedItems = GetIndividuallySelectedItems();
            };

            unselectAllButton.Pressed += (o, e) =>
            {
                selectItemsCheckBoxList.UncheckAll();
                SelectedItems = GetIndividuallySelectedItems();
            };

            selectFirstXItemsButton.Pressed += SelectFirstXItemsButton_Pressed;

            selectItemsCheckBoxList.Changed += (o, e) => SelectedItems = GetIndividuallySelectedItems();
        }

        /// <summary>
        /// Event triggered when we need to regenerate UI.
        /// </summary>
        public event EventHandler RegenerateUiRequired;

        private void SelectFirstXItemsButton_Pressed(object sender, EventArgs e)
        {
            int counter = 0;
            foreach (var option in selectItemsCheckBoxList.Options.ToList())
            {
                if (counter < (int)selectFirstXItemsNumeric.Value)
                {
                    selectItemsCheckBoxList.Check(option);
                }
                else
                {
                    selectItemsCheckBoxList.Uncheck(option);
                }

                counter++;
            }

            SelectedItems = GetIndividuallySelectedItems();
        }

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

            int previousAmountOfOptions = selectItemsCheckBoxList.Options.Count();

            amountOfMatchingItemsLabel.Text = $"Found {itemsBasedOnFilters.Count} matching {typeof(DataMinerObjectType).Name}s";

            selectItemsCheckBoxList.SetOptions(itemsBasedOnFilters.Select(r => GetItemIdentifier(r)).OrderBy(name => name).ToList());
            selectItemsCheckBoxList.CheckAll();

            var selectedResources = GetIndividuallySelectedItems();

            selectAllButton.IsVisible = selectedResources.Any();
            unselectAllButton.IsVisible = selectedResources.Any();

            if (selectItemsCheckBoxList.Options.Count() != previousAmountOfOptions) InvokeRegenerateUi();

            return selectedResources;       
        }

        private IEnumerable<DataMinerObjectType> GetIndividuallySelectedItems()
        {
            var selectedResourceNames = selectItemsCheckBoxList.Checked;

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
            AddWidget(header, ++row, 0, 1, 5);

            AddFilterSections(ref row, out int firstAvailablecolumn);

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(getItemsBasedOnFiltersButton, ++row, 0, 1, 5);

            AddWidget(new WhiteSpace(), row + 1, 0);

            AddWidget(amountOfMatchingItemsLabel, 0, firstAvailablecolumn);
            AddWidget(selectAllButton, 0, firstAvailablecolumn + 1, verticalAlignment: VerticalAlignment.Top);
            AddWidget(unselectAllButton, 0, firstAvailablecolumn + 2, verticalAlignment: VerticalAlignment.Top);
            AddWidget(amountOfSelectedItemsLabel, 1, firstAvailablecolumn);
            AddWidget(selectFirstXItemsNumeric, 1, firstAvailablecolumn + 1, verticalAlignment: VerticalAlignment.Top);
            AddWidget(selectFirstXItemsButton, 1, firstAvailablecolumn + 2, verticalAlignment: VerticalAlignment.Top);
            AddWidget(selectItemsCheckBoxList, 2, firstAvailablecolumn, selectItemsCheckBoxList.Options.Any() ? selectItemsCheckBoxList.Options.Count() : 1, 1, verticalAlignment: VerticalAlignment.Top);
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
