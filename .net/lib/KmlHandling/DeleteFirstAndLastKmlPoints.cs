using System.Diagnostics;
using Common;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace KmlHandling;

public class DeleteFirstAndLastKmlPoints: ICommandHandler<DeleteFirstAndLastKmlPointsCommand>
{
    public void Execute(DeleteFirstAndLastKmlPointsCommand command)
    {
        string fullFileName = Path.Join(command.Folder, command.KmlFileName);
        if (!File.Exists(fullFileName)) throw new FileNotFoundException($"File {Path.GetFullPath(fullFileName)} not found!");

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(KmlModel.Kml));
        using FileStream fileStream = File.OpenRead(fullFileName);
        KmlModel.Kml? kml = (KmlModel.Kml?)xmlSerializer.Deserialize(fileStream);
        string? coordinates = kml?.Document?.Placemarks?[0].LineString?.Coordinates;

        if (coordinates is not null)
        {
            List<string> splitCoordinates = coordinates
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var listOfCoordinates = new List<(string longitude, string latitude, string altitude)>();

            for (int i = 0; i < splitCoordinates.Count; i += 3)
            {
                if (splitCoordinates.Count > i + 2)
                {
                    listOfCoordinates.Add((
                        splitCoordinates[i],
                        splitCoordinates[i + 1],
                        splitCoordinates[i + 2]
                    ));
                }
            }
        }
    }
}