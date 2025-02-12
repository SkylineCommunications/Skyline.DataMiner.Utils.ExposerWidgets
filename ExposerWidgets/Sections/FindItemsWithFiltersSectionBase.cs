namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;

	/// <summary>
	/// Base class for filter sections.
	/// </summary>
	public abstract class FindItemsWithFiltersSectionBase : SectionBase
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="FindItemsWithFiltersSectionBase"/>"/> class.
		/// </summary>
		protected FindItemsWithFiltersSectionBase()
		{

		}

		/// <summary>
		/// An event raised when DataMiner objects have been retrieved based on the filters.
		/// </summary>
		public event EventHandler DataMinerObjectsRetrievedBasedOnFilters;

		/// <summary>
		/// Gets a boolean indicating if this item type is supported on the current system.
		/// </summary>
		public bool ItemTypeIsSupportedOnThisSystem { get; set; } = true;

		/// <summary>
		/// Method that triggers UI regeneration evenet.
		/// </summary>
		protected void InvokeDataMinerObjectsRetrievedBasedOnFilters()
		{
			DataMinerObjectsRetrievedBasedOnFilters?.Invoke(this, EventArgs.Empty);
		}
	}
}
