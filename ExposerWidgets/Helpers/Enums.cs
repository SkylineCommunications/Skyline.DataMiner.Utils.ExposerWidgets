namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// 
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
	/// 
	/// </summary>
	public enum ComparerType
	{
		/// <summary>
		/// 
		/// </summary>
		Unknown,

		/// <summary>
		/// 
		/// </summary>
		Active,

		/// <summary>
		/// 
		/// </summary>
		Passive
	}

	public class ComparerTypeAttribute : Attribute
	{
		public ComparerTypeAttribute(ComparerType comparerType)
		{
			ComparerType = comparerType;
		}

		public ComparerType ComparerType { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetDescription(this Enum value)
		{
			var enumField = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException($"Value '{value}' is not a valid enum value");

			var attribute = enumField.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static ComparerType GetComparerType(this Enum value)
		{
			var enumField = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException($"Value '{value}' is not a valid enum value");

			var attribute = enumField.GetCustomAttributes(typeof(ComparerTypeAttribute), false).SingleOrDefault() as ComparerTypeAttribute;

			return attribute?.ComparerType ?? ComparerType.Unknown;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static T GetEnumValue<T>(this string value)
		{
			var type = typeof(T);
			if (!type.IsEnum) throw new InvalidOperationException();

			FieldInfo[] fields = type.GetFields();
			var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DescriptionAttribute)a.Att).Description == value);

			return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
		}
	}
}
