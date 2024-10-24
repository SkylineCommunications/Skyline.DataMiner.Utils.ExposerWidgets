namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	public class DropDownOption<InternalValueType> : IDropDownOption<InternalValueType>
    {
		public DropDownOption(string displayValue, InternalValueType internalValue)
		{
			DisplayValue = displayValue;
			InternalValue = internalValue;
		}

		public string DisplayValue { get; set; }

		public InternalValueType InternalValue { get; }
	}
}