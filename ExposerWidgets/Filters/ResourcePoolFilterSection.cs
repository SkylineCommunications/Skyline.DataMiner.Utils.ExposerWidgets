namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.ResourceManager.Objects;
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ResourcePoolFilterSection : FilterSectionOneInput<FunctionResource, Guid>, IDataMinerObjectFilter<FunctionResource>
    {
        private readonly IEnumerable<ResourcePool> resourcePools;
        private readonly DropDown resourcePoolDropDown;

        public ResourcePoolFilterSection(string filterName, Func<Guid, FilterElement<FunctionResource>> emptyFilter, IEnumerable<ResourcePool> resourcePools) : base(filterName, emptyFilter)
        {
            this.resourcePools = resourcePools;

            var resourcePoolOptions = resourcePools.Select(p => p.Name).OrderBy(name => name).ToList();
            resourcePoolDropDown = new DropDown(resourcePoolOptions, resourcePoolOptions[0]) { IsDisplayFilterShown = true };

            GenerateUi();
        }

        public override bool IsValid => true;

        public override Guid Value
        {
            get => SelectedResourcePool.ID;
            set => resourcePoolDropDown.Selected = resourcePools.Single(pool => pool.ID == value).Name;
        }

        public override void Reset()
        {
            IsActive = false;
            Value = resourcePools.First().ID;
        }

        private ResourcePool SelectedResourcePool => resourcePools.SingleOrDefault(pool => pool.Name == resourcePoolDropDown.Selected);

        protected override void GenerateUi()
        {
            base.GenerateUi();

            AddWidget(resourcePoolDropDown, 0, 1);
        }

        protected override void HandleDefaultUpdate()
        {
            filterNameCheckBox.IsChecked = IsDefault;
            filterNameCheckBox.IsEnabled = !IsDefault;
            resourcePoolDropDown.IsEnabled = !IsDefault;
        }
    }
}
