namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class DateTimeFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, DateTime>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private readonly DateTimePicker dateTimePicker = new DateTimePicker(DateTime.Now);

        public DateTimeFilterSection(string filterName, Func<DateTime, FilterElement<DataMinerObjectType>> filterFunction) : base(filterName, filterFunction)
        {
            GenerateUi();
        }

        public override bool IsValid => true;

        public override DateTime Value
        {
            get => dateTimePicker.DateTime;
            set => dateTimePicker.DateTime = value;
        }

        public override void Reset()
        {
            IsActive = false;
            Value = DateTime.Now;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(dateTimePicker, 0, 1);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            dateTimePicker.IsEnabled = !IsDefault;
        }
    }
}