using System.Xml.Serialization;
using static KmlHandling.KmlModel;

namespace KmlHandling;

public static class Common
{
    public static async Task DoSerializationAsync(Kml kml, string? fileName)
    {
        if (fileName is null) throw new NullReferenceException("File name is empty!");

        await using FileStream stream = new(
            fileName,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        XmlSerializerNamespaces ns = new();
        ns.Add("", "http://www.opengis.net/kml/2.2");

        XmlSerializer xmlSerializer = new(typeof(Kml));
        xmlSerializer.Serialize(stream, kml, ns);
        await stream.FlushAsync();
    }
}