namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// A static class providing tools for handling enums.
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the value of the Description attribute on an enum value.
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
		/// Gets the value of the ComparerType attribute on an enum value.
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
		/// Gets the enum value where the Description attribute matches the string.
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
