namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.YLE.UI.Filters;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Ticketing;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;

    public class FindTicketsWithFiltersSection : FindItemsWithFiltersSection<Ticket>
    {
        private readonly FilterSectionBase<Ticket> ticketIdFilterSection = new StringFilterSection<Ticket>("Ticket ID", x => TicketingExposers.FullID.Equal(x));

        private readonly FilterSectionBase<Ticket> ticketCreationDateFromFilterSection = new DateTimeFilterSection<Ticket>("Creation Date From", x => TicketingExposers.CreationDate.GreaterThanOrEqual(x));

        private readonly FilterSectionBase<Ticket> ticketCreationDateUntilFilterSection = new DateTimeFilterSection<Ticket>("Creation Date Until", x => TicketingExposers.CreationDate.LessThanOrEqual(x));

        private readonly FilterSectionBase<Ticket> ticketDomainFilterSection = new GuidFilterSection<Ticket>("Ticket Domain ID", x => TicketingExposers.ResolverID.Equal(x));

        private readonly List<FilterSectionBase<Ticket>> propertyFilterSections = new List<FilterSectionBase<Ticket>>();

        private readonly RadioButtonList typeOfPropertyValueFilterToAdd = new RadioButtonList(new[] { "String", "Integer", "Enum" });
        private readonly Button addPropertyValueFilterButton = new Button("Add Property Value Filter");

        private readonly Button addPropertyExistenceFilterButton = new Button("Add Property Existence Filter");

        private readonly TicketingGatewayHelper ticketingHelper;

        public FindTicketsWithFiltersSection() : base()
        {            
            ticketingHelper = new TicketingGatewayHelper { HandleEventsAsync = false };
            ticketingHelper.RequestResponseEvent += (sender, args) => args.responseMessage = Skyline.DataMiner.Automation.Engine.SLNet.SendSingleResponseMessage(args.requestMessage);

            addPropertyExistenceFilterButton.Pressed += AddPropertyExistenceFilterButton_Pressed;

            addPropertyValueFilterButton.Pressed += AddPropertyValueFilterButton_Pressed;
        }

        public void AddStringPropertyValueFilter(string propertyName, string propertyValue = null, bool setAsDefault = false)
        {
            var propertyFilterSection = new StringStringFilterSection<Ticket>("String Property", (pName, pValue) => TicketingExposers.CustomTicketFields.DictStringField(pName).Equal(pValue));

            if (setAsDefault)
            {
                propertyFilterSection.SetDefault(propertyName, propertyValue);
            }
            else
            {
                propertyFilterSection.FirstValue = propertyName;
                propertyFilterSection.SecondValue = propertyValue ?? string.Empty;
            }

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        public void AddEnumPropertyValueFilter(string propertyName, string firstPropertyValue = null, int? secondPropertyValue = null, bool setAsDefault = false)
        {
            var propertyFilterSection = new TicketEnumFilterSection<Ticket>("Enum Property", (pName, pValue1, pValue2) => TicketingExposers.CustomTicketFields.DictField(pName).Equal($"{pValue1}/{pValue2}"));

            if (setAsDefault)
            {
                propertyFilterSection.SetDefault(propertyName, firstPropertyValue, secondPropertyValue.Value);
            }
            else
            {
                propertyFilterSection.FirstValue = propertyName;
                propertyFilterSection.SecondValue = firstPropertyValue ?? string.Empty;
                propertyFilterSection.ThirdValue = secondPropertyValue.HasValue ? secondPropertyValue.Value : 0;
            }

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        public void AddIntegerPropertyValueFilter(string propertyName, int? propertyValue = null, bool setAsDefault = false)
        {
            var propertyFilterSection = new StringIntegerFilterSection<Ticket>("Integer Property", (pName, pValue) => TicketingExposers.CustomTicketFields.DictField(pName).Equal(pValue));

            if (setAsDefault)
            {
                propertyFilterSection.SetDefault(propertyName, propertyValue.Value);
            }
            else
            {
                propertyFilterSection.FirstValue = propertyName;
                propertyFilterSection.SecondValue = propertyValue.HasValue ? propertyValue.Value : 0;
            }

            propertyFilterSections.Add(propertyFilterSection);

            InvokeRegenerateUi();
        }

        public void AddPropertyExistenceFilter(string propertyName, bool setAsDefault = false)
        {
            var propertyExistenceFilterSection = new StringFilterSection<Ticket>("Property Exists", (pName) => TicketingExposers.CustomTicketFields.DictStringField(pName).NotEqual(string.Empty));

            if (setAsDefault)
            {
                propertyExistenceFilterSection.SetDefault(propertyName);
            }
            else
            {
                propertyExistenceFilterSection.Value = propertyName;
            }

            propertyFilterSections.Add(propertyExistenceFilterSection);

            InvokeRegenerateUi();
        }

        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }

            propertyFilterSections.Clear();
        }

        private void AddPropertyValueFilterButton_Pressed(object sender, EventArgs e)
        {
            switch (typeOfPropertyValueFilterToAdd.Selected)
            {
                case nameof(String):
                    AddStringPropertyValueFilter(string.Empty);
                    break;

                case "Integer":
                    AddIntegerPropertyValueFilter(string.Empty);
                    break;
                case "Enum":
                    AddEnumPropertyValueFilter(string.Empty);
                    break;

                default:
                    break;
            }
        }

        private void AddPropertyExistenceFilterButton_Pressed(object sender, EventArgs e)
        {
            AddPropertyExistenceFilter(string.Empty);
        }

        protected override void HandleVisibilityAndEnabledUpdate(bool isVisible, bool isEnabled)
        {
            ticketIdFilterSection.IsEnabled = isEnabled;

            ticketCreationDateFromFilterSection.IsEnabled = isEnabled;

            ticketCreationDateUntilFilterSection.IsEnabled = isEnabled;

            ticketDomainFilterSection.IsEnabled = isEnabled;

            propertyFilterSections.ForEach(f => f.IsEnabled = isEnabled);
        }

        protected override IEnumerable<Ticket> FindItemsWithFilters()
        {
            return new HashSet<Ticket>(ticketingHelper.GetTickets(null, GetCombinedFilterElement(), false).ToList());
        }

        protected override string GetItemIdentifier(Ticket item)
        {
            return item.ID.ToString();
        }

        protected override void AddFilterSections(ref int row)
        {
            AddSection(ticketIdFilterSection, new SectionLayout(++row, 0));

            AddSection(ticketCreationDateFromFilterSection, new SectionLayout(++row, 0));

            AddSection(ticketCreationDateUntilFilterSection, new SectionLayout(++row, 0));

            AddSection(ticketDomainFilterSection, new SectionLayout(++row, 0));

            foreach (var propertyFilterSection in propertyFilterSections)
            {
                AddSection(propertyFilterSection, new SectionLayout(++row, 0));
                row += propertyFilterSection.RowCount;
            }

            AddWidget(addPropertyExistenceFilterButton, ++row, 0);
            AddWidget(addPropertyValueFilterButton, ++row, 0, verticalAlignment: VerticalAlignment.Top);
            AddWidget(typeOfPropertyValueFilterToAdd, row, 1);
        }
    }
}
