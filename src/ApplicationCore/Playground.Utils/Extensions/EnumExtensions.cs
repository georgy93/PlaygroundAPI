namespace Playground.Utils.Extensions;

using System.ComponentModel;

public static class EnumExtensions
{
    extension(Enum value)
    {
        public string ToDescription()
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();

            return attribute is null ? value.ToString() : attribute.Description;
        }

        private T GetAttribute<T>() where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            return (T)attributes.FirstOrDefault();
        }
    }
}