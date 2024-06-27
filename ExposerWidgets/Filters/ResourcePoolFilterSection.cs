namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents filter section for resource pools.
	/// </summary>
	public class ResourcePoolFilterSection : FilterSectionOneInput<Resource, Guid>, IDataMinerObjectFilter<Resource>
    {
        private IEnumerable<ResourcePool> resourcePools;
        private DropDown resourcePoolDropDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePoolFilterSection"/>"/> class.
        /// </summary>
        /// <param name="filterName">Name of filter.</param>
        /// <param name="filterFunctions">Filter that will be applied.</param>
        /// <param name="resourcePools">Available resource pools.</param>
        public ResourcePoolFilterSection(string filterName, Dictionary<Comparers, Func<Guid, FilterElement<Resource>>> filterFunctions, IEnumerable<ResourcePool> resourcePools) : base(filterName, filterFunctions)
        {
            Initialize(resourcePools);

            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected ResourcePoolFilterSection(ResourcePoolFilterSection other) : base(other)
		{
			Initialize(other.resourcePools);

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
		/// The widget that allows the user to input a value for the filter.
		/// </summary>
		protected override InteractiveWidget InputWidget => resourcePoolDropDown;

		private ResourcePool SelectedResourcePool => resourcePools.SingleOrDefault(pool => pool.Name == resourcePoolDropDown.Selected);

		/// <summary>
		/// Creates a clone of the current instance.
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<Resource> Clone()
		{
            return new ResourcePoolFilterSection(this);
		}

		private void Initialize(IEnumerable<ResourcePool> resourcePools)
		{
			this.resourcePools = resourcePools ?? new List<ResourcePool>();

			var resourcePoolOptions = resourcePools.Select(p => p.Name).OrderBy(name => name).ToList();
			resourcePoolDropDown = new DropDown(resourcePoolOptions, resourcePoolOptions[0]) { IsDisplayFilterShown = true };
		}
	}
}
