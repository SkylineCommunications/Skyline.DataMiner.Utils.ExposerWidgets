namespace Skyline.DataMiner.Utils.YLE.UI.Filters
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Label = InteractiveAutomationScript.Label;

    public abstract class FindItemsWithFiltersSection<DataMinerObjectType> : Section, IDisableableUi
    {
        private readonly Label header = new Label($"Get {typeof(DataMinerObjectType).Name}s with filters") { Style = TextStyle.Heading };

        private readonly Button getItemsBasedOnFiltersButton = new Button($"Get {typeof(DataMinerObjectType).Name}s Based on Filters") { Style = ButtonStyle.CallToAction };
        private List<DataMinerObjectType> itemsBasedOnFilters = new List<DataMinerObjectType>();

        private readonly Label amountOfMatchingItemsLabel = new Label(string.Empty);
        private readonly Label amountOfSelectedItemsLabel = new Label(string.Empty);
        private readonly CheckBoxList selectItemsCheckBoxList = new CheckBoxList();
        private readonly Button selectAllButton = new Button("Select All");
        private readonly Button unselectAllButton = new Button("Unselect All");
        private readonly Numeric selectFirstXItemsNumeric = new Numeric { Decimals = 0, StepSize = 1, Minimum = 0 };
        private readonly Button selectFirstXItemsButton = new Button("Select First X");

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

            GenerateUi();
            HandleVisibilityAndEnabledUpdate();
        }

        public new bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                base.IsVisible = value;

                if (!IsVisible) return; // if everything should be hidden, there is no point in calling the method below

                HandleVisibilityAndEnabledUpdate();
            }
        }

        public new bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                base.IsEnabled = value;
                HandleVisibilityAndEnabledUpdate();
            }
        }

		public event EventHandler<ValueEventArgs<EnabledState>> UiEnabledStateChangeRequired;

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

        public IEnumerable<DataMinerObjectType> SelectedItems { get; private set; } = new List<DataMinerObjectType>();

        public void RegenerateUi()
        {
            GenerateUi();
            HandleVisibilityAndEnabledUpdate();
        }

        public void ResetFiltersAndSelectedItems()
        {
            ResetFilters();
            SelectedItems = GetItemsBasedOnFilters();
        }

        protected abstract void ResetFilters();

        protected abstract IEnumerable<DataMinerObjectType> FindItemsWithFilters();

        protected abstract string GetItemIdentifier(DataMinerObjectType item);

        private IEnumerable<DataMinerObjectType> GetItemsBasedOnFilters()
        {
            using (UiDisabler.StartNew(this))
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
        }

        private IEnumerable<DataMinerObjectType> GetIndividuallySelectedItems()
        {
            var selectedResourceNames = selectItemsCheckBoxList.Checked;

            var selectedResources = itemsBasedOnFilters.Where(r => selectedResourceNames.Contains(GetItemIdentifier(r))).ToList();

            amountOfSelectedItemsLabel.Text = $"Selected {selectedResources.Count} {typeof(DataMinerObjectType).Name}s";

            return selectedResources;
        }

        protected abstract void AddFilterSections(ref int row);

        protected void GenerateUi(ref int row)
        {
            AddWidget(header, ++row, 0, 1, 2);

            AddFilterSections(ref row);

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(getItemsBasedOnFiltersButton, ++row, 0);

            AddWidget(new WhiteSpace(), row + 1, 0);

            AddWidget(amountOfMatchingItemsLabel, 0, 3);
            AddWidget(selectAllButton, 0, 4, verticalAlignment: VerticalAlignment.Top);
            AddWidget(unselectAllButton, 0, 5, verticalAlignment: VerticalAlignment.Top);
            AddWidget(amountOfSelectedItemsLabel, 1, 3);
            AddWidget(selectFirstXItemsNumeric, 1, 4, verticalAlignment: VerticalAlignment.Top);
            AddWidget(selectFirstXItemsButton, 1, 5, verticalAlignment: VerticalAlignment.Top);
            AddWidget(selectItemsCheckBoxList, 2, 3, selectItemsCheckBoxList.Options.Any() ? selectItemsCheckBoxList.Options.Count() : 1, 1, verticalAlignment: VerticalAlignment.Top);
        }

        protected virtual void HandleVisibilityAndEnabledUpdate(bool isVisible, bool isEnabled)
        {
            selectAllButton.IsVisible = SelectedItems.Any();
            unselectAllButton.IsVisible = SelectedItems.Any();
        }

        protected IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>> GetIndividualFilters()
        {
            var fieldValues = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(field => field.GetValue(this)).ToList();

            var fieldsImplementingInterface = fieldValues.OfType<IDataMinerObjectFilter<DataMinerObjectType>>().ToList();

            var fieldsContainingCollectionOfInterface = fieldValues.OfType<IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>>>().SelectMany(collection => collection).ToList();

            var filters = fieldsImplementingInterface.Concat(fieldsContainingCollectionOfInterface).ToList();

            return filters;
        }

        protected bool ActiveFiltersAreValid()
        {
            var individualFilters = GetIndividualFilters();

            if (!individualFilters.Any()) return true;

            return individualFilters.Where(filter => filter.IsActive).All(filter => filter.IsValid);
        }

        protected bool OneOrMoreFiltersAreActive()
        {
            var individualFilters = GetIndividualFilters();

            return individualFilters.Any(filter => filter.IsActive);
        }

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

        protected ANDFilterElement<DataMinerObjectType> GetCombinedFilterElement(params FilterElement<DataMinerObjectType>[] defaultFilters)
        {
            var individualActiveFilterElements = new List<FilterElement<DataMinerObjectType>>();

            individualActiveFilterElements.AddRange(defaultFilters);
            individualActiveFilterElements.AddRange(GetIndividualFilters().Where(filter => filter.IsActive).Select(filter => filter.FilterElement));

            if (!individualActiveFilterElements.Any()) throw new InvalidOperationException("Unable to find any active filters");

            return new ANDFilterElement<DataMinerObjectType>(individualActiveFilterElements.ToArray());
        }

        protected void GenerateUi()
        {
            Clear();

            int row = 0;

            GenerateUi(ref row);
        }

        protected void HandleVisibilityAndEnabledUpdate()
        {
            HandleVisibilityAndEnabledUpdate(IsVisible, IsEnabled);
        }
        protected void InvokeRegenerateUi()
        {
            RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
        }

        public void DisableUi()
        {
            UiEnabledStateChangeRequired?.Invoke(this, new ValueEventArgs<EnabledState>(EnabledState.Disabled));
        }

        public void EnableUi()
        {
            UiEnabledStateChangeRequired?.Invoke(this, new ValueEventArgs<EnabledState>(EnabledState.Enabled));
        }
    }
}
