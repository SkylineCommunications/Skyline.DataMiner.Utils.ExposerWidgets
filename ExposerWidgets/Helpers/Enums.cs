namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// An enum for each kind of comparison between values.
	/// </summary>
	public enum Comparers
	{
		/// <summary>
		/// 
		/// </summary>
		[Description("Equals")]
		[ComparerType(ComparerType.Active)]
		Equals,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Equal")]
		[ComparerType(ComparerType.Active)]
		NotEquals,

		/// <summary>
		/// 
		/// </summary>
		[Description("Contains")]
		[ComparerType(ComparerType.Active)]
		Contains,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Contain")]
		[ComparerType(ComparerType.Active)]
		NotContains,

		/// <summary>
		/// 
		/// </summary>
		[Description("Greater Than")]
		[ComparerType(ComparerType.Active)]
		GreaterThan,

		/// <summary>
		/// 
		/// </summary>
		[Description("Less Than")]
		[ComparerType(ComparerType.Active)]
		LessThan,

		/// <summary>
		/// 
		/// </summary>
		[Description("Exists")]
		[ComparerType(ComparerType.Passive)]
		Exists,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Exist")]
		[ComparerType(ComparerType.Passive)]
		NotExists,

		/// <summary>
		/// 
		/// </summary>
		[Description("Is Used")]
		[ComparerType(ComparerType.Passive)]
		IsUsed,

		/// <summary>
		/// 
		/// </summary>
		[Description("Is Not Used")]
		[ComparerType(ComparerType.Passive)]
		IsNotUsed,
	}

	/// <summary>
	/// An enum for the type of comparison.
	/// </summary>
	public enum ComparerType
	{
		/// <summary>
		/// Unknown type.
		/// </summary>
		Unknown,

		/// <summary>
		/// The comparison is active.
		/// </summary>
		Active,

		/// <summary>
		/// The comparison is passive.
		/// </summary>
		Passive
	}

	/// <summary>
	/// An attribute indicating which type of comparison is used
	/// </summary>
	public class ComparerTypeAttribute : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="comparerType"></param>
		public ComparerTypeAttribute(ComparerType comparerType)
		{
			ComparerType = comparerType;
		}

		/// <summary>
		/// 
		/// </summary>
		public ComparerType ComparerType { get; set; }
	}
}
