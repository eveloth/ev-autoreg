using System.Reflection;

namespace EvAutoreg.Extensions;

public static class PropertyInfoExtensions
{
    public static bool IsValueNullOnObject<T>(this PropertyInfo property, T obj)
    {
        return property.GetValue(obj) is null;
    }
}