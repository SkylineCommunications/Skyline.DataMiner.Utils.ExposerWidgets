namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering dom instances.
	/// </summary>
	public class FindDomObjectsWithFiltersSection : FindItemsWithFiltersSection<DomInstance>
    {
        private readonly Label moduleId = new Label("Module ID:");
		private readonly TextBox moduleIdTextBox = new TextBox(string.Empty);

		private readonly FilterSectionBase<DomInstance> idFilterSection = new GuidFilterSection<DomInstance>("Dom Instance ID", x => DomInstanceExposers.DomDefinitionId.Equal(x));
        private readonly FilterSectionBase<DomInstance> nameFilterSection = new StringFilterSection<DomInstance>("Dom Instance Name Equals", x => DomInstanceExposers.Name.Equal(x));
        private readonly FilterSectionBase<DomInstance> nameContainsFilterSection = new StringFilterSection<DomInstance>("Dom Instance Name Contains", x => DomInstanceExposers.Name.Contains(x));
        private readonly FilterSectionBase<DomInstance> nameDoesntContainFilterSection = new StringFilterSection<DomInstance>("Dom Instance Name Doesn't Contain", x => DomInstanceExposers.Name.NotContains(x));
        
        private DomHelper domHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDomObjectsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindDomObjectsWithFiltersSection() : base()
        {
            moduleIdTextBox.FocusLost += ModuleIdTextBox_FocusLost;
        }

        private void ModuleIdTextBox_FocusLost(object sender, TextBox.TextBoxFocusLostEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Value)) domHelper = new DomHelper(Engine.SLNet.SendMessages, e.Value);
        }

        /// <summary>
        /// Adding filter sections on a row specified.
        /// </summary>
        /// <param name="row">Row position where new section should appear.</param>
        protected override void AddFilterSections(ref int row)
        {
            AddWidget(moduleId, ++row, 0);
            AddWidget(moduleIdTextBox, row, 1);

            AddSection(idFilterSection, new SectionLayout(++row, 0));

            AddSection(nameFilterSection, new SectionLayout(++row, 0));

            AddSection(nameContainsFilterSection, new SectionLayout(++row, 0));

            AddSection(nameDoesntContainFilterSection, new SectionLayout(++row, 0));
        }

        /// <summary>
        /// Retrieving all items in the system based on input values.
        /// </summary>
        /// <returns>Collection of dom instances.</returns>
        protected override IEnumerable<DomInstance> FindItemsWithFilters()
        {
            if (domHelper == null) 
            { 
                return new List<DomInstance>();
            } 

            return domHelper.DomInstances.Read(GetCombinedFilterElement());
        }

        /// <summary>
        /// Retrieves name of dom instance.
        /// </summary>
        /// <returns>Name of dom instance.</returns>
        protected override string GetItemIdentifier(DomInstance item)
        {
            return item.Name;
        }

        /// <summary>
        /// Resets filters in section to default values.
        /// </summary>
        protected override void ResetFilters()
        {
            foreach (var filterSection in GetIndividualFilters())
            {
                filterSection.Reset();
            }
        }
    }
}
