namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	public enum Comparers
	{
		[Description("Equals")]
		Equals,
		[Description("Does not equal")]
		NotEquals,
		[Description("Contains")]
		Contains,
		[Description("Does not contain")]
		NotContains,
		[Description("Greater than")]
		GreaterThan,
		[Description("Less than")]
		LessThan,
		[Description("Exists")]
		Exists,
		[Description("Does not exist")]
		NotExists,
	}

	public static class EnumExtensions
	{
		public static string GetDescription(this Enum value)
		{
			var enumField = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException($"Value '{value}' is not a valid enum value");

			var attribute = enumField.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static T GetEnumValue<T>(this string value)
		{
			var type = typeof(T);
			if (!type.IsEnum) throw new ArgumentException();

			FieldInfo[] fields = type.GetFields();
			var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DescriptionAttribute)a.Att).Description == value);

			return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
		}
	}
}
