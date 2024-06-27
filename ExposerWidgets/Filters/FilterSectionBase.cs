namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// The base class for each individual filter.
	/// </summary>
	/// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
	public abstract class FilterSectionBase<DataMinerObjectType> : Section, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// Checkbox showing the name of the property to which the filter applies and indicating whether the filter should be included or not.
        /// </summary>
        protected readonly CheckBox isIncludedCheckBox;

        /// <summary>
        /// DropDown containing the different comparer options.
        /// </summary>
        protected readonly DropDown comparerDropDown = new DropDown();

        /// <summary>
        /// Optional tooltip icon. Will be hidden when no tooltip is defined.
        /// </summary>
        protected readonly Label tooltipLabel = new Label("ⓘ") { Style = TextStyle.Title };

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionBase{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="tooltip"></param>
		protected FilterSectionBase(string filterName, string tooltip = null)
        {
            this.isIncludedCheckBox = new CheckBox(filterName);
            tooltipLabel.Tooltip = tooltip ?? string.Empty;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected FilterSectionBase(FilterSectionBase<DataMinerObjectType> other)
        {
            this.isIncludedCheckBox = new CheckBox(other.isIncludedCheckBox.Text);
        } 

        /// <summary>
        /// Gets a value indicating if the filter should be included in the query or not.
        /// </summary>
        public bool IsIncluded => isIncludedCheckBox.IsChecked;

		/// <summary>
		/// Gets a value indicating if the filter is valid or not.
		/// </summary>
		public abstract bool IsValid { get; }

        /// <summary>
        /// Gets a the actual filter based on input values. Will be used to query DataMiner objects in the system.
        /// </summary>
        public abstract FilterElement<DataMinerObjectType> FilterElement { get; }

        /// <summary>
        /// Creates a clone of this FilterSection.
        /// </summary>
        /// <returns></returns>
        public abstract FilterSectionBase<DataMinerObjectType> Clone();

        /// <summary>
        /// Generates UI for filter section.
        /// </summary>
        protected virtual void GenerateUi()
        {
            Clear();

            AddWidget(isIncludedCheckBox, 0, 0);
			
            AddWidget(comparerDropDown, 0, 1);
		}
    }
}
