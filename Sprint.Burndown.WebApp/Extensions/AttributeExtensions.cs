using System;
using System.Linq;

using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class AttributeExtensions
    {
        public static string GetShortName(this Enum enumMember)
        {
            var type = enumMember.GetType();
            var memberInfo = type.GetMember(enumMember.ToString());
            var attr = memberInfo[0]
                .GetCustomAttributes(typeof(ShortNameAttribute), false)
                .FirstOrDefault() as ShortNameAttribute;

            return attr?.ShortName ?? string.Empty;
        }
    }
}
