using Common;
using System.Xml;
using static KmlHandling.KmlModel;

namespace KmlHandling;

public class CreateKml: ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public async Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.GeospatialDataFileName))
            throw new Exception("KML file name cannot be empty!");

        command.GeospatialDataFileNameExtension = ".kml";

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
        await Common.DoSerializationAsync(kml, command.GeospatialDataFileName);
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
}