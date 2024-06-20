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
		Equals,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Equal")]
		NotEquals,

		/// <summary>
		/// 
		/// </summary>
		[Description("Contains")]
		Contains,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Contain")]
		NotContains,

		/// <summary>
		/// 
		/// </summary>
		[Description("Greater Than")]
		GreaterThan,

		/// <summary>
		/// 
		/// </summary>
		[Description("Less Than")]
		LessThan,

		/// <summary>
		/// 
		/// </summary>
		[Description("Exists")]
		Exists,

		/// <summary>
		/// 
		/// </summary>
		[Description("Does Not Exist")]
		NotExists,
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
