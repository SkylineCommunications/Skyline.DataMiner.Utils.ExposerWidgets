namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class StringFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, string>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        protected readonly TextBox filterContentTextBox = new TextBox();

        public StringFilterSection(string filterName, Func<string, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => true;

        public override string Value
        {
            get => filterContentTextBox.Text;
            set => filterContentTextBox.Text = value;
        }

        public override void Reset()
        {
            IsActive = false;
            Value = string.Empty;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(filterContentTextBox, 0, 1);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            filterContentTextBox.IsEnabled = !IsDefault;
        }
    }
}