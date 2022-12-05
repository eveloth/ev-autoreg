using System.Xml.Serialization;

namespace Extensions;

public static class StringExtensions
{
    public static string Subtract(this string input, string substring)
    {
        return input.Contains(substring) ? input.Replace(substring, string.Empty) : input;
    }

    public static T DeserializeXmlString<T>(this string xml)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        return (T)xmlSerializer.Deserialize(reader)!;
    }
}