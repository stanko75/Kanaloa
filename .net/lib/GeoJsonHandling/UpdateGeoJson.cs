using Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GeoJsonHandling;

public class UpdateGeoJson: ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public async Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.GeospatialDataFileName))
            throw new Exception("GeoJson file name cannot be empty!");

        command.GeospatialDataFileNameExtension = ".geojson";

        string json = await File.ReadAllTextAsync(command.GeospatialDataFileName);
        JsonObject geoJson = JsonNode.Parse(json)!.AsObject();

        geoJson["features"]![0]!["geometry"]!["coordinates"] = geoJson["features"]![0]!["geometry"]!["coordinates"] + "," + command.Coordinates;

        string updatedJson = geoJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(command.GeospatialDataFileName, updatedJson);
    }
}