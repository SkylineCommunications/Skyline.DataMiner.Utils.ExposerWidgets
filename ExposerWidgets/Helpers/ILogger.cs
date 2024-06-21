namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface ILogger
	{
		void Log(string message);

		void Log(string className, string methodName, string message);
	}
}
