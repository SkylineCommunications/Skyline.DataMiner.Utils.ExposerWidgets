namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.ResourceManager.Objects;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section for resource pools.
    /// </summary>
    public class ResourcePoolFilterSection : FilterSectionOneInput<FunctionResource, Guid>, IDataMinerObjectFilter<FunctionResource>
    {
        private readonly IEnumerable<ResourcePool> resourcePools;
        private readonly DropDown resourcePoolDropDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePoolFilterSection"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="emptyFilter">Filter that will be applied.</param>
        /// <param name="resourcePools">Available resource pools.</param>
        public ResourcePoolFilterSection(string filterName, Func<Guid, FilterElement<FunctionResource>> emptyFilter, IEnumerable<ResourcePool> resourcePools) : base(filterName, emptyFilter)
        {
            this.resourcePools = resourcePools;

            var resourcePoolOptions = resourcePools.Select(p => p.Name).OrderBy(name => name).ToList();
            resourcePoolDropDown = new DropDown(resourcePoolOptions, resourcePoolOptions[0]) { IsDisplayFilterShown = true };

            GenerateUi();
        }

        /// <summary>
        /// Indicates if selected pool is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets selected resource pool.
        /// </summary>
        public override Guid Value
        {
            get => SelectedResourcePool.ID;
            set => resourcePoolDropDown.Selected = resourcePools.Single(pool => pool.ID == value).Name;
        }

        /// <summary>
        /// Resets resource pool filter to default value.
        /// </summary>
        public override void Reset()
        {
            IsActive = false;
            Value = resourcePools.First().ID;
        }

        /// <summary>
        /// Gets selected resource pool.
        /// </summary>
        private ResourcePool SelectedResourcePool => resourcePools.SingleOrDefault(pool => pool.Name == resourcePoolDropDown.Selected);

        /// <summary>
        /// Generates resource pool filter section UI.
        /// </summary>
        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(resourcePoolDropDown, 0, 1);
        }

        /// <summary>
        /// Handles default resource pool filter update.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            resourcePoolDropDown.IsEnabled = !IsDefault;
        }
    }
}
