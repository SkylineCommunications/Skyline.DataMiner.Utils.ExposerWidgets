namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class BooleanFilterSection<T> : FilterSectionOneInput<T, bool>, IDataMinerObjectFilter<T>
    {
        private readonly CheckBox filterChecbox = new CheckBox();

        public BooleanFilterSection(string filterName, Func<bool, FilterElement<T>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => true;

        public override bool Value
        {
            get => filterChecbox.IsChecked;
            set => filterChecbox.IsChecked = value;
        }

        public override void Reset()
        {
            IsActive = false;
            Value = false;
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterChecbox.IsEnabled = !IsDefault;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterChecbox, 0, 1);
        }
    }
}
