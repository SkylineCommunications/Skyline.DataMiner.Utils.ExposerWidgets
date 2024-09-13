namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class DropDownOption<InternalValueType> : IDropDownOption<InternalValueType>
    {
		public DropDownOption(string displayValue, InternalValueType internalValue)
		{
			DisplayValue = displayValue;
			InternalValue = internalValue;
		}

		public string DisplayValue { get; }

		public InternalValueType InternalValue { get; }
    }
}
