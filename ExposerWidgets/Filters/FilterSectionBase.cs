namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public abstract class FilterSectionBase<DataMinerObjectType> : Section, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private bool isDefault;

        protected CheckBox filterNameCheckBox;

        protected FilterSectionBase(string filterName)
        {
            this.filterNameCheckBox = new CheckBox(filterName);
        }

        public bool IsActive
        {
            get => filterNameCheckBox.IsChecked;
            protected set => filterNameCheckBox.IsChecked = value;
        }

        public new bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                base.IsEnabled = value;
                HandleEnabledUpdate();
            }
        }

        public bool IsDefault
        {
            get => isDefault;
            protected set
            {
                isDefault = value;
                HandleDefaultUpdate();
            }
        }

        public abstract bool IsValid { get; }

        public abstract FilterElement<DataMinerObjectType> FilterElement { get; }

        public abstract void Reset();

        protected virtual void GenerateUi()
        {
            Clear();

            AddWidget(filterNameCheckBox, 0, 0);
        }

        protected abstract void HandleDefaultUpdate();

        private void HandleEnabledUpdate()
        {
            bool filterIsChecked = IsActive;

            HandleDefaultUpdate();

            filterNameCheckBox.IsChecked = filterIsChecked || IsDefault;
        }
    }
}
