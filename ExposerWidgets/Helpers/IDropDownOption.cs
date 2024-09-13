namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface IDropDownOption<InternalValueType>
	{
		string DisplayValue { get; }

		InternalValueType InternalValue { get; }
	}
}
