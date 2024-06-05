namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

#pragma warning disable S2436 // Types and methods should not have too many generic parameters
    public abstract class FilterSectionThreeInputs<DataMinerObjectType, FilterInputType1, FilterInputType2, FilterInputType3> : FilterSectionBase<DataMinerObjectType>
#pragma warning restore S2436 // Types and methods should not have too many generic parameters
    {
        private readonly Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> filterFunctionWithThreeInputs;

        protected FilterSectionThreeInputs(string filterName, Func<FilterInputType1, FilterInputType2, FilterInputType3, FilterElement<DataMinerObjectType>> emptyFilter) : base(filterName)
        {
            filterFunctionWithThreeInputs = emptyFilter;
        }

        public override FilterElement<DataMinerObjectType> FilterElement => filterFunctionWithThreeInputs(FirstValue, SecondValue, ThirdValue);

        public abstract FilterInputType1 FirstValue { get; set; }

        public abstract FilterInputType2 SecondValue { get; set; }

        public abstract FilterInputType3 ThirdValue { get; set; }

        public void SetDefault(FilterInputType1 value, FilterInputType2 secondValue, FilterInputType3 thirdValue)
        {
            IsDefault = true;

            FirstValue = value;
            SecondValue = secondValue;
            ThirdValue = thirdValue;
        }
    }

}
