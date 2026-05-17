using System.Diagnostics;
using Common;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace KmlHandling;

public class DeleteFirstAndLastKmlPoints(IKmlSerializer kmlSerializer) : ICommandHandler<DeleteFirstAndLastKmlPointsCommand>
{
    public void Execute(DeleteFirstAndLastKmlPointsCommand command)
    {
        string fullFileName = Path.Join(command.Folder, command.KmlFileName);
        if (!File.Exists(fullFileName)) throw new FileNotFoundException($"File {Path.GetFullPath(fullFileName)} not found!");

        KmlModel.Kml? kml = kmlSerializer.DoDeserialization(fullFileName);
        var coordinates = kml?.Document?.Placemarks?[0].LineString?.Coordinates;

        if (coordinates is not null)
        {
            coordinates = string.Join(",",
                coordinates
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(100 * 3)
                    .Select(x => x.Trim())
            );

            kml?.Document?.Placemarks?[0].LineString?.Coordinates = coordinates;

            kmlSerializer.DoSerialization(kml, fullFileName);
        }
    }
}