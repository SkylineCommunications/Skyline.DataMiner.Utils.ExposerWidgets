namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class StringIntegerFilterSection<DataMinerObjectType> : FilterSectionTwoInputs<DataMinerObjectType, string, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        protected readonly TextBox propertyNameTextBox = new TextBox();
        protected readonly Numeric propertyValueNumeric = new Numeric { Decimals = 0, StepSize = 1 };

        public StringIntegerFilterSection(string filterName, Func<string, int, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => !string.IsNullOrEmpty(propertyNameTextBox.Text);

        public override string FirstValue
        {
            get => propertyNameTextBox.Text;
            set => propertyNameTextBox.Text = value;
        }

        public override int SecondValue
        {
            get => (int)propertyValueNumeric.Value;
            set => propertyValueNumeric.Value = value;
        }

        public override void Reset()
        {
            IsActive = false;
            FirstValue = string.Empty;
            SecondValue = 0;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(propertyNameTextBox, 0, 1);
            AddWidget(propertyValueNumeric, 0, 2);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            propertyNameTextBox.IsEnabled = !IsDefault;
            propertyValueNumeric.IsEnabled = !IsDefault;
        }
    }
}
