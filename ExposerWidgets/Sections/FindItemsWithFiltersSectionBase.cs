namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Base class for filter sections.
	/// </summary>
	public abstract class FindItemsWithFiltersSectionBase : Section
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="FindItemsWithFiltersSectionBase"/>"/> class.
		/// </summary>
		protected FindItemsWithFiltersSectionBase()
		{

		}

		/// <summary>
		/// Event triggered when we need to regenerate UI.
		/// </summary>
		public event EventHandler RegenerateUiRequired;

		/// <summary>
		/// An event raised when DataMiner objects have been retrieved based on the filters.
		/// </summary>
		public event EventHandler DataMinerObjectsRetrievedBasedOnFilters;

		/// <summary>
		/// Gets a boolean indicating if this item type is supported on the current system.
		/// </summary>
		public bool ItemTypeIsSupportedOnThisSystem { get; protected set; } = true;

		/// <summary>
		/// Regenerates the UI for this instance.
		/// </summary>
		public abstract void RegenerateUi();

		/// <summary>
		/// Method that triggers UI regeneration evenet.
		/// </summary>
		protected void InvokeRegenerateUi()
		{
			RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Method that triggers UI regeneration evenet.
		/// </summary>
		protected void InvokeDataMinerObjectsRetrievedBasedOnFilters()
		{
			DataMinerObjectsRetrievedBasedOnFilters?.Invoke(this, EventArgs.Empty);
		}
	}
}
