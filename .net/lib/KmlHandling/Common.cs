using System.Xml.Serialization;
using static KmlHandling.KmlModel;

namespace KmlHandling;

public static class Common
{
    public static void DoSerialization(Kml kml, string? fileName)
    {
        if (fileName is null) throw new NullReferenceException("File name is empty!");

        TextWriter txtWriter = new StreamWriter(fileName);

        XmlSerializerNamespaces ns = new();
        ns.Add("", "http://www.opengis.net/kml/2.2");

        XmlSerializer xmlSerializer = new(typeof(Kml));
        xmlSerializer.Serialize(txtWriter, kml, ns);
        txtWriter.Close();
    }
}