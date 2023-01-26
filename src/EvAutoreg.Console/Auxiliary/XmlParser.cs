using System.Xml.Serialization;

namespace EvAutoreg.Console.Auxiliary;

public static class XmlParser
{
    public static T Deserialize<T>(string xml) where T : class
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        using var reader = new StringReader(xml);

        return (T)xmlSerializer.Deserialize(reader)!;
    }
}
