using Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GeoJsonHandling;

public class DeleteFirstAndLastGeoJsonPointsHandler: ICommandHandler<DeleteFirstAndLastGeoJsonPointsCommand>
{
    public void Execute(DeleteFirstAndLastGeoJsonPointsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.DeleteFirstGeoJsonPoints < 0)
            throw new ArgumentOutOfRangeException(nameof(command.DeleteFirstGeoJsonPoints));

        if (command.DeleteLastGeoJsonPoints < 0)
            throw new ArgumentOutOfRangeException(nameof(command.DeleteLastGeoJsonPoints));

        string fullFileName = Path.Join(command.Folder, command.GeoJsonFileName);
        if (!File.Exists(fullFileName))
            throw new FileNotFoundException($"File {Path.GetFullPath(fullFileName)} not found!");

        JsonNode? root = JsonNode.Parse(File.ReadAllText(fullFileName));
        JsonArray coordinates = root?["features"]?[0]?["geometry"]?["coordinates"]?.AsArray()
            ?? throw new JsonException("The GeoJSON file does not contain a LineString coordinates array.");

        int pointsToRemove = command.DeleteFirstGeoJsonPoints + command.DeleteLastGeoJsonPoints;
        if (pointsToRemove > coordinates.Count)
            throw new ArgumentOutOfRangeException(nameof(command), "The number of points to delete exceeds the number of points in the GeoJSON file.");

        coordinates.RemoveRange(0, command.DeleteFirstGeoJsonPoints);
        coordinates.RemoveRange(
            coordinates.Count - command.DeleteLastGeoJsonPoints,
            command.DeleteLastGeoJsonPoints);

        File.WriteAllText(
            fullFileName,
            root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}