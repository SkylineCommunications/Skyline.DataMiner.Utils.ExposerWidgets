namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	/// <summary>
	/// 
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		/// <param name="methodName"></param>
		/// <param name="message"></param>
		void Log(string className, string methodName, string message);
	}
}
