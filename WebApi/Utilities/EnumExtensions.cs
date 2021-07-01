using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApi.Utilities
{
    public static class EnumExtensions
    {

        public static string GetDisplay(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            return field.GetCustomAttribute<DisplayAttribute>()?.Name ?? field.Name;
        }

        public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
        {
            Assert.NotNull(value, nameof(value));

            var attribute = value.GetType().GetField(value.ToString())
                .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

            if (attribute == null)
                return value.ToString();

            var propValue = attribute.GetType().GetProperty(property.ToString()).GetValue(attribute, null);
            return propValue.ToString();
        }

        public static Dictionary<int, string> ToDictionary(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(p => Convert.ToInt32(p), q => ToDisplay(q));
        }
    }

    public enum DisplayProperty
    {
        Description,
        GroupName,
        Name,
        Prompt,
        ShortName,
        Order
    }
}
