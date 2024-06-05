namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

#pragma warning disable S2436 // Types and methods should not have too many generic parameters
    public abstract class FilterSectionTwoInputs<DataMinerObjectType, FilterInputType1, FilterInputType2> : FilterSectionBase<DataMinerObjectType>
#pragma warning restore S2436 // Types and methods should not have too many generic parameters
    {
        private readonly Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> filterFunctionWithTwoInputs;

        protected FilterSectionTwoInputs(string filterName, Func<FilterInputType1, FilterInputType2, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName)
        {
            this.filterFunctionWithTwoInputs = emptyFilter;
        }

        public override FilterElement<DataMinerObjectType> FilterElement => filterFunctionWithTwoInputs(FirstValue, SecondValue);

        public abstract FilterInputType1 FirstValue { get; set; }

        public abstract FilterInputType2 SecondValue { get; set; }

        public void SetDefault(FilterInputType1 value, FilterInputType2 secondValue)
        {
            IsDefault = true;

            FirstValue = value;
            SecondValue = secondValue;
        }
    }
}
