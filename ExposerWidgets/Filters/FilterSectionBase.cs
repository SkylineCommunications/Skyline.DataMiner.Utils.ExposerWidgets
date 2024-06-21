namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents base filter section with one checkbox input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of object that is being filtered.</typeparam>
    public abstract class FilterSectionBase<DataMinerObjectType> : Section, IDataMinerObjectFilter<DataMinerObjectType>
    {
        private bool isDefault;

        /// <summary>
        /// Indicates how many columns this section already used.
        /// </summary>
        protected int nextAvailableColumn = 0;

        /// <summary>
        /// 
        /// </summary>
        protected readonly CheckBox filterNameCheckBox;

        /// <summary>
        /// 
        /// </summary>
        protected readonly DropDown filterDropDown = new DropDown();

        /// <summary>
        /// 
        /// </summary>
        protected readonly Label tooltipLabel = new Label("ⓘ") { Style = TextStyle.Title };

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterSectionBase{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="tooltip"></param>
		protected FilterSectionBase(string filterName, string tooltip = null)
        {
            this.filterNameCheckBox = new CheckBox(filterName);
            tooltipLabel.Tooltip = tooltip ?? string.Empty;
            tooltipLabel.IsVisible = !string.IsNullOrWhiteSpace(tooltipLabel.Tooltip);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected FilterSectionBase(FilterSectionBase<DataMinerObjectType> other)
        {
            this.filterNameCheckBox = new CheckBox(other.filterNameCheckBox.Text);
        } 

        /// <summary>
        /// Gets or sets value of active filter.
        /// </summary>
        public bool IsIncluded
        {
            get => filterNameCheckBox.IsChecked;
            protected set => filterNameCheckBox.IsChecked = value;
        }

        /// <summary>
        /// Gets and sets enable status of filter.
        /// </summary>
        public new bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                base.IsEnabled = value;
                HandleEnabledUpdate();
            }
        }

        /// <summary>
        /// Gets or sets of default filter value.
        /// </summary>
        public bool IsDefault
        {
            get => isDefault;
            protected set
            {
                isDefault = value;
                filterNameCheckBox.IsChecked = value;
                filterNameCheckBox.IsEnabled = !value;
                HandleDefaultUpdate();
            }
        }

        /// <summary>
        /// Indicates if the filter is valid.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Filter that is created based on input values. Used in getting DataMiner objects in the system.
        /// </summary>
        public abstract FilterElement<DataMinerObjectType> FilterElement { get; }

        /// <summary>
        /// Resets filter values to default.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract FilterSectionBase<DataMinerObjectType> Clone();

        /// <summary>
        /// Generates UI for filter section.
        /// </summary>
        protected virtual void GenerateUi()
        {
            Clear();
            nextAvailableColumn = 0;

            AddWidget(filterNameCheckBox, 0, nextAvailableColumn++);
			
            AddWidget(filterDropDown, 0, nextAvailableColumn++);
		}

		/// <summary>
		/// Handles default update of filter.
		/// </summary>
		protected abstract void HandleDefaultUpdate();

        private void HandleEnabledUpdate()
        {
            bool filterIsChecked = IsIncluded;

            HandleDefaultUpdate();

            filterNameCheckBox.IsChecked = filterIsChecked || IsDefault;
        }
    }
}
