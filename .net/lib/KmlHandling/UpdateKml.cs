using Common;
using System.Xml.Serialization;
using static KmlHandling.KmlModel;

namespace KmlHandling;

public class UpdateKml : ICommandHandler<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        Kml? kml = DoDeserialization(command.GeospatialDataFileName);
        LineString? lineString = kml?.Document?.Placemarks?[0].LineString;
        if (lineString is not null)
        {
            lineString.Coordinates = lineString.Coordinates + "," + command.Coordinates;
        }
        Common.DoSerialization(kml, command.GeospatialDataFileName);
    }

    private Kml? DoDeserialization(string? fileName)
    {
        if (fileName is null) throw new NullReferenceException("File name is empty!");

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Kml));
        using FileStream fileStream = File.OpenRead(fileName);
        Kml? kml = (Kml?)xmlSerializer.Deserialize(fileStream);
        return kml;
    }
}