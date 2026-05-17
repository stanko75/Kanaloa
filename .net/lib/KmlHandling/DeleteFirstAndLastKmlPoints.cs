using Common;

namespace KmlHandling;

public class DeleteFirstAndLastKmlPoints(IKmlSerializer kmlSerializer) : ICommandHandler<DeleteFirstAndLastKmlPointsCommand>
{
    public void Execute(DeleteFirstAndLastKmlPointsCommand command)
    {
        string fullFileName = Path.Join(command.Folder, command.KmlFileName);
        if (!File.Exists(fullFileName)) throw new FileNotFoundException($"File {Path.GetFullPath(fullFileName)} not found!");

        KmlModel.Kml? kml = kmlSerializer.DoDeserialization(fullFileName);
        string? coordinates = kml?.Document?.Placemarks?[0].LineString?.Coordinates;

        if (coordinates is not null)
        {
            List<string> parts = coordinates
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            int skipStart = command.DeleteFirstKmlPoints * 3;
            int skipEnd = command.DeleteLastKmlPoints * 3;

            coordinates = string.Join(",",
                parts.Skip(skipStart)
                    .Take(parts.Count - skipStart - skipEnd)
            );

            coordinates = string.Join(",",
                coordinates
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(command.DeleteFirstKmlPoints * 3)
                    .Select(x => x.Trim())
            );

            kml?.Document?.Placemarks?[0].LineString?.Coordinates = coordinates;

            kmlSerializer.DoSerialization(kml, fullFileName);
        }
    }
}