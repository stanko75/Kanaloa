using System.Xml.Serialization;

namespace KmlHandling;

public class KmlSerializerTextWriter(Type type) : IKmlSerializer
{
    public Type Type { get; set; } = type;

    public void DoSerialization(object? kml, string? fileName)
    {
        if (fileName is null) throw new NullReferenceException("File name is empty!");

        TextWriter txtWriter = new StreamWriter(fileName);

        XmlSerializerNamespaces ns = new();
        ns.Add("", "http://www.opengis.net/kml/2.2");

        XmlSerializer xmlSerializer = new(Type);
        xmlSerializer.Serialize(txtWriter, kml, ns);
        txtWriter.Close();
    }

    public KmlModel.Kml? DoDeserialization(string? fileName)
    {
        if (fileName is null) throw new NullReferenceException("File name is empty!");

        XmlSerializer xmlSerializer = new XmlSerializer(Type);
        using FileStream fileStream = File.OpenRead(fileName);
        KmlModel.Kml? kml = (KmlModel.Kml?)xmlSerializer.Deserialize(fileStream);
        return kml;
    }
}