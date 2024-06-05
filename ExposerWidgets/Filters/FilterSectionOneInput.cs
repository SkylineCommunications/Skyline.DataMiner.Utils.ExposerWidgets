namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.ExposerWidgets.Filters;

    public abstract class FilterSectionOneInput<DataMinerObjectType, FilterInputType> : FilterSectionBase<DataMinerObjectType>
    {
        private readonly Func<FilterInputType, FilterElement<DataMinerObjectType>> filterFunctionWithOneInput;

        protected FilterSectionOneInput(string filterName, Func<FilterInputType, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName)
        {
            this.filterFunctionWithOneInput = emptyFilter;
        }

        public abstract FilterInputType Value { get; set; }

        public override FilterElement<DataMinerObjectType> FilterElement => filterFunctionWithOneInput(Value);

        public void SetDefault(FilterInputType value)
        {
            IsDefault = true;

            Value = value;
        }
    }
}