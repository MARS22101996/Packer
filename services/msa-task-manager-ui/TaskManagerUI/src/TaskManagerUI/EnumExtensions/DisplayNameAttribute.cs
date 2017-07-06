using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace TaskManagerUI.EnumExtensions
{
    public static class DisplayNameAttribute
    {
        public static string GetName<TEnum>(this TEnum enumValue)
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            var attribute = (DisplayAttribute)attributes.ElementAt(0);
            return attribute.GetName();
        }
    }
}