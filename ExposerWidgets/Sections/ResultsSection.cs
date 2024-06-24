namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	public class ResultsSection<DataMinerObjectType> : Section
	{
		private readonly CollapseButton collapseButton = new CollapseButton() { CollapseText = "-", ExpandText = "+", Width = 44 };
		private readonly Label selectedItemsHeader = new Label("Results") { Style = TextStyle.Title, IsVisible = false };
		private readonly Label amountOfMatchingItemsLabel = new Label(string.Empty);
		private readonly Label amountOfSelectedItemsLabel = new Label(string.Empty);
		private List<CheckBox> selectItemsCheckBoxList = new List<CheckBox>();
		private readonly CheckBoxList selectItemsCheckBoxList2 = new CheckBoxList() { Height = 500 };
		private readonly Button selectAllButton = new Button("Select All") { Width = 100, IsVisible = false };
		private readonly Button unselectAllButton = new Button("Unselect All") { Width = 100, IsVisible = false };
		
		private readonly Func<DataMinerObjectType, string> identifyItemFunction;
		
		private List<DataMinerObjectType> allItems = new List<DataMinerObjectType>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="identifyItemFunction"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ResultsSection(Func<DataMinerObjectType, string> identifyItemFunction)
		{
			this.identifyItemFunction = identifyItemFunction ?? throw new ArgumentNullException(nameof(identifyItemFunction));

			collapseButton.Pressed += (s,e) => UpdateWidgetVisibility();

			selectAllButton.Pressed += (o, e) =>
			{
				selectItemsCheckBoxList.ForEach(x => x.IsChecked = true);
				selectItemsCheckBoxList2.CheckAll();
				SetAmountOfSelectedItemsMessage();
			};

			unselectAllButton.Pressed += (o, e) =>
			{
				selectItemsCheckBoxList.ForEach(x => x.IsChecked = false);
				selectItemsCheckBoxList2.UncheckAll();
				SetAmountOfSelectedItemsMessage();
			};
		}

		private void UpdateWidgetVisibility()
		{
			amountOfMatchingItemsLabel.IsVisible = !collapseButton.IsCollapsed;

			selectedItemsHeader.IsVisible = !collapseButton.IsCollapsed;
			amountOfSelectedItemsLabel.IsVisible = !collapseButton.IsCollapsed && allItems.Any();
			selectAllButton.IsVisible = !collapseButton.IsCollapsed && allItems.Any();
			unselectAllButton.IsVisible = !collapseButton.IsCollapsed && allItems.Any();

			selectItemsCheckBoxList.ForEach(x => x.IsVisible = !collapseButton.IsCollapsed);
			selectItemsCheckBoxList2.IsVisible = !collapseButton.IsCollapsed;
		}

		/// <summary>
		/// Gets list of selected DataMiner objects.
		/// </summary>
		public IEnumerable<DataMinerObjectType> SelectedItems => GetIndividuallySelectedItems();

		/// <summary>
		/// Event triggered when we need to regenerate UI.
		/// </summary>
		public event EventHandler RegenerateUiRequired;

		/// <summary>
		/// 
		/// </summary>
		public void RegenerateUi()
		{
			GenerateUi();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newItems"></param>
		public void LoadNewItems(IEnumerable<DataMinerObjectType> newItems)
		{
			allItems = newItems.ToList();

			amountOfMatchingItemsLabel.Text = $"Found {allItems.Count} {typeof(DataMinerObjectType).Name}s matching the filters";

			selectItemsCheckBoxList = allItems.Select(r => identifyItemFunction(r)).OrderBy(name => name).Select(name => new CheckBox(name) { IsChecked = true }).ToList();
			selectItemsCheckBoxList.ForEach(x => x.Changed += (o, e) => SetAmountOfSelectedItemsMessage());

			selectItemsCheckBoxList2.SetOptions(allItems.Select(r => identifyItemFunction(r)).OrderBy(name => name));
			selectItemsCheckBoxList2.CheckAll();
			selectItemsCheckBoxList2.Changed += (s, e) => SetAmountOfSelectedItemsMessage();

			collapseButton.IsCollapsed = false;
			UpdateWidgetVisibility();
			RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
		}

		private void SetAmountOfSelectedItemsMessage()
		{
			amountOfSelectedItemsLabel.Text = $"Selected {selectItemsCheckBoxList2.Checked.Count()} {typeof(DataMinerObjectType).Name}s";
		}

		private IEnumerable<DataMinerObjectType> GetIndividuallySelectedItems()
		{
			var selectedItemNames = selectItemsCheckBoxList.Where(x => x.IsChecked).Select(x => x.Text);

			var selectedItems = allItems.Where(r => selectedItemNames.Contains(identifyItemFunction(r))).ToList();

			SetAmountOfSelectedItemsMessage();

			return selectedItems;
		}

		private void GenerateUi()
		{
			Clear();

			int row = -1;
			int column = -1;

			AddWidget(collapseButton, ++row, ++column);
			AddWidget(selectedItemsHeader, row, ++column, 1, 5);
			AddWidget(amountOfMatchingItemsLabel, ++row, column, 1, 2);
			AddWidget(selectAllButton, ++row, column);
			AddWidget(unselectAllButton, row, column + 1);
			AddWidget(amountOfSelectedItemsLabel, ++row, column, 1, 2);

			foreach (var selectedItemCheckBox in selectItemsCheckBoxList)
			{
				AddWidget(selectedItemCheckBox, ++row, column, 1, 2);
			}

			AddWidget(selectItemsCheckBoxList2, ++row, 5);
		}
	}
}
