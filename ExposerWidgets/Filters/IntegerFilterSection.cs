namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class IntegerFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        protected readonly Numeric filterContentNumeric = new Numeric();

        public IntegerFilterSection(string filterName, Func<int, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => true;

        public override int Value
        {
            get => (int)filterContentNumeric.Value;
            set => filterContentNumeric.Value = value;
        }

        public override void Reset()
        {
            IsActive = false;
            Value = 0;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterContentNumeric, 0, 1);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterContentNumeric.IsEnabled = !IsDefault;
        }
    }
}