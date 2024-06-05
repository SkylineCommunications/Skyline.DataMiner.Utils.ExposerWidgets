namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class GuidFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, Guid>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        protected readonly TextBox filterContentTextBox = new TextBox();

        public GuidFilterSection(string filterName, Func<Guid, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => Guid.TryParse(filterContentTextBox.Text, out _);

        public override Guid Value
        {
            get => Guid.Parse(filterContentTextBox.Text);
            set => filterContentTextBox.Text = value.ToString();
        }

        public override void Reset()
        {
            IsActive = false;
            Value = Guid.Empty;
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