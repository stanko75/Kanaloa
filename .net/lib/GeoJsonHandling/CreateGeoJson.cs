using Common;
using System.Text.Json;

namespace GeoJsonHandling;

public class CreateGeoJson: ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public async Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.GeospatialDataFileName))
            throw new Exception("GeoJson file name cannot be empty!");

        command.GeospatialDataFileNameExtension = ".geojson";

        string? coordinates = command.Coordinates;
        var geoJson = new
        {
            type = "FeatureCollection",
            features = new[]
            {
                new
                {
                    type = "Feature",
                    properties = new
                    {
                        name = "test",
                        description = "test",
                        styleUrl = "styleUrl test"
                    },
                    geometry = new
                    {
                        type = "LineString",
                        coordinates
                    }
                }
            }
        };

        string json = JsonSerializer.Serialize(geoJson, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(command.GeospatialDataFileName, json);
    }
}