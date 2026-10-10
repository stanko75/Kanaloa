using Common;
using System.Xml;
using System.Xml.Serialization;
using static KmlHandling.KmlModel;

namespace KmlHandling;

public class CreateKml: ICommandHandlerAsync<UpdateKmlIfExistsOrCreateNewIfNotCommand>
{
    public async Task Execute(UpdateKmlIfExistsOrCreateNewIfNotCommand command)
    {
        Placemark[]? placeMarks;
        placeMarks =
        [
            new Placemark
            {
                Name = "test"
                , Description = new XmlDocument().CreateCDataSection("test")
                , StyleUrl= "styleUrl test"
                , LineString = new LineString
                {
                    Extrude = "1"
                    , Tessellate = "1"
                    , AltitudeMode = "absolute"
                    , Coordinates = command.Coordinates
                }
            }
        ];
        
        Kml kml = GenerateKml("test", "test", null, placeMarks);
        DoSerialization(kml, command.KmlFileName);
    }

    public Kml GenerateKml(string name, string description, Style? style, Placemark[]? placeMarks)
    {
        Kml kmlModel = new()
        {
            Document = new Document
            {
                Name = name,
                Description = description,
                Style = style,
                Placemarks = placeMarks
            }
        };

        return kmlModel;
    }

    private void DoSerialization(Kml kml, string? fileName)
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