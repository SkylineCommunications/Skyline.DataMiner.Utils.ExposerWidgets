namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.DomInstances
{
	using System;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering DOM instances.
	/// </summary>
	public class FindDomInstancesWithFiltersSection : FindItemsWithFiltersSection<DomInstance>
	{
		private readonly Label moduleIdLabel = new Label("DOM Module ID:");
		private readonly DropDown moduleIdDropDown;

		/// <summary>
		/// Initializes a new instance of the <see cref="FindDomInstancesWithFiltersSection"/>"/> class.
		/// </summary>
		public FindDomInstancesWithFiltersSection() : base()
		{
			var moduleSettingsHelper = new ModuleSettingsHelper(Engine.SLNet.SendMessages);
			var allModuleIds = moduleSettingsHelper.ModuleSettings.ReadAll().Select(x => x.ModuleId).OrderBy(id => id).ToList();

			if (!allModuleIds.Any())
			{
				ItemTypeIsSupportedOnThisSystem = false;
				return;
			}

			moduleIdDropDown = new DropDown(allModuleIds, allModuleIds.FirstOrDefault() ?? throw new InvalidOperationException("No DOM modules defined on this system")) { IsDisplayFilterShown = true };
			moduleIdDropDown.Changed += ModuleIdDropDown_Changed;

			SetDomHelper(moduleIdDropDown.Selected);

			GenerateUi();
		}

		/// <summary>
		/// Gets the DomHelper for the current DOM Module ID.
		/// </summary>
		public DomHelper DomHelper { get; private set; }

		private void SetDomHelper(string domModuleId)
		{
			if (!string.IsNullOrWhiteSpace(domModuleId))
			{
				DomHelper = new DomHelper(Engine.SLNet.SendMessages, domModuleId);

				base.RemoveAllSectionsContainingFilters();
				base.AddNewSectionContainingFilters(new DomInstanceFiltersSection(DomHelper));
				base.SetDataMinerObjectFinder(new DomInstanceFinder(DomHelper));
			}
		}

		private void ModuleIdDropDown_Changed(object sender, DropDown.DropDownChangedEventArgs e)
		{
			SetDomHelper(e.Selected);

			InvokeRegenerateUi();
		}

		/// <summary>
		/// Adding filter sections on a row specified.
		/// </summary>
		/// <param name="row">Row position where new section should appear.</param>
		protected override void AddWidgetsBeforeFilters(ref int row)
		{
			AddWidget(moduleIdLabel, ++row, 1);
			AddWidget(moduleIdDropDown, row, 4, 1, 2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isVisible"></param>
		protected override void SetWidgetsVisibility(bool isVisible)
		{
			base.SetWidgetsVisibility(isVisible);

			moduleIdLabel.IsVisible = isVisible;
			moduleIdDropDown.IsVisible = isVisible;
		}
	}
}
