namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class TicketEnumFilterSection<DataMinerObjectType> : FilterSectionThreeInputs<DataMinerObjectType, string, string, int>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        protected readonly TextBox propertyNameTextBox = new TextBox();
        protected readonly TextBox propertyValueTextBox = new TextBox();
        protected readonly Numeric propertyValueNumeric = new Numeric { Decimals = 0, StepSize = 1 };

        public TicketEnumFilterSection(string filterName, Func<string, string, int, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName, emptyFilter)
        {
            GenerateUi();
        }

        public override bool IsValid => !string.IsNullOrEmpty(propertyNameTextBox.Text);

        public override string FirstValue
        {
            get => propertyNameTextBox.Text;
            set => propertyNameTextBox.Text = value;
        }

        public override string SecondValue
        {
            get => propertyValueTextBox.Text;
            set => propertyValueTextBox.Text = value;
        }

        public override int ThirdValue
        {
            get => (int)propertyValueNumeric.Value;
            set => propertyValueNumeric.Value = value;
        }

        public override void Reset()
        {
            IsActive = false;
            FirstValue = string.Empty;
            SecondValue = string.Empty;
            ThirdValue = 0;
        }

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(propertyNameTextBox, 0, 1);
            AddWidget(propertyValueTextBox, 0, 2);
            AddWidget(propertyValueNumeric, 1, 2);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            propertyNameTextBox.IsEnabled = !IsDefault;
            propertyValueTextBox.IsEnabled = !IsDefault;
            propertyValueNumeric.IsEnabled = !IsDefault;
        }
    }
}
