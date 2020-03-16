using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            EnumMemberAttribute[] attributes = (EnumMemberAttribute[])fi.GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Value;
            }
            return null;
        }
    }
}
