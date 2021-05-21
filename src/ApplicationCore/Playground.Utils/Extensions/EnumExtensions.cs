namespace Playground.Utils.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();

            return attribute == null ? value.ToString() : attribute.Description;
        }

        private static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            return (T)attributes.FirstOrDefault();
        }
    }
}