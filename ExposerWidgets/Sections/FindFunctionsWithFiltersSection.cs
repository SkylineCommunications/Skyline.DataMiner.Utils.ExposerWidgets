namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering protocol functions.
	/// </summary>
	public class FindFunctionsWithFiltersSection : FindItemsWithFiltersSection<FunctionDefinition>
    {
        private readonly Label allActiveLabel = new Label("All active functions:");
        private readonly Label protocolNameLabel = new Label("Protocol name:");

        private readonly CheckBox allActiveCheckbox = new CheckBox("Only active ones");
        private readonly TextBox protocolNameTextBox = new TextBox(string.Empty);

        private readonly ProtocolFunctionHelper protocolFunctionHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindFunctionsWithFiltersSection"/>"/> class.
        /// </summary>
        public FindFunctionsWithFiltersSection() : base()
        {
            protocolFunctionHelper = new ProtocolFunctionHelper(Engine.SLNet.SendMessages);
			GenerateUi();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		/// <param name="row">Row on which section should appear.</param>
		/// <param name="firstAvailableColumn"></param>
		protected override void AddFilterSections(ref int row, out int firstAvailableColumn)
        {
            AddWidget(allActiveLabel, ++row, 0);
            AddWidget(allActiveCheckbox, row, 1);

            AddWidget(protocolNameLabel, ++row, 0);
            AddWidget(protocolNameTextBox, row, 1);

			firstAvailableColumn = ColumnCount + 1;
		}

        /// <summary>
        /// Filtering all protocol functions in system based on provided input.
        /// </summary>
        /// <returns>Collection of filtered protocol functions.</returns>
        protected override IEnumerable<FunctionDefinition> FindItemsWithFilters()
        {
            List<FunctionDefinition> functionDefinitions = new List<FunctionDefinition>();

            if (allActiveCheckbox.IsChecked)
            {
                var activeProtocolFunctionVersions = protocolFunctionHelper.GetAllProtocolFunctions(true).Select(p => p.ProtocolFunctionVersions.FirstOrDefault());

                foreach (var version in activeProtocolFunctionVersions)
                {
                    foreach (var functionDefinition in version.FunctionDefinitions)
                    {
                        functionDefinitions.Add(functionDefinition);
                    }
                }

                return functionDefinitions;
            }

            if(!protocolNameLabel.Text.IsNullOrEmpty())
            {
                var activeProtocolFunctionVersions = protocolFunctionHelper.GetProtocolFunctions(protocolNameLabel.Text).Select(p => p.ProtocolFunctionVersions.FirstOrDefault());

                foreach (var version in activeProtocolFunctionVersions)
                {
                    foreach (var functionDefinition in version.FunctionDefinitions)
                    {
                        functionDefinitions.Add(functionDefinition);
                    }
                }

                return functionDefinitions;
            }

            return functionDefinitions;
        }

        /// <summary>
        /// Gets name of protocol function.
        /// </summary>
        /// <param name="item">Protocol function for which we want to retrieve name.</param>
        /// <returns>Name of protocol function.</returns>
        protected override string GetItemIdentifier(FunctionDefinition item)
        {
            return item.Name;
        }

        /// <summary>
        /// Resets filters in section to default values.
        /// </summary>
        protected override void ResetFilters()
        {
            allActiveCheckbox.IsChecked = false;
            protocolNameTextBox.Text = string.Empty;
        }
    }
}
