using Common;
using GeoJsonHandling;
using System.Text.Json;

namespace GeospatialData.Test;

[TestClass]
public class UpdateGeospatialDataIfExistsOrCreateNewIfNotTests
{
    private readonly string _testFolder = Path.Combine(Path.GetTempPath(), "GeospatialData");
    private readonly string _geospatialDataFileName = "test";

    [TestMethod]
    public async Task CreateGeoJson()
    {
        UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand =
            new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
            {
                Coordinates = "6.641421,43.1869746,2357",
                GeospatialDataFileName = _geospatialDataFileName,
                GeospatialDataFileNameExtension = "geojson",
                FolderName = _testFolder
            };

        CreateGeoJson createJson = new CreateGeoJson();
        await createJson.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);

        Assert.IsTrue(IsExpectedGeoJson(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName));
    }

    [TestMethod]
    public async Task UpdateGeoJson()
    {
        var command = new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
        {
            Coordinates = "6.641421,43.1869746,2357",
            GeospatialDataFileName = _geospatialDataFileName,
            GeospatialDataFileNameExtension = "geojson",
            FolderName = _testFolder
        };

        string geospatialDataFullFileName = Path.ChangeExtension(command.GeospatialDataFileName, command.GeospatialDataFileNameExtension);
        geospatialDataFullFileName = Path.Join(command.FolderName, geospatialDataFullFileName);

        if (File.Exists(geospatialDataFullFileName))
        {
            File.Delete(geospatialDataFullFileName);
        }

        CreateGeoJson createGeoJson = new CreateGeoJson();
        await createGeoJson.Execute(command);

        command.Coordinates = "6.982319,50.2130308,501.0483676407712";
        UpdateGeoJson updateGeoJson = new UpdateGeoJson();
        await updateGeoJson.Execute(command);

        Assert.IsTrue(IsExpectedGeoJson(command.GeospatialDataFileName, "6.641421,43.1869746,2357,6.982319,50.2130308,501.0483676407712"));
    }

    private static bool IsExpectedGeoJson(string geoJsonFileName, string expectedCoordinates = "6.641421,43.1869746,2357")
    {
        try
        {
            if (!File.Exists(geoJsonFileName))
                return false;

            string json = File.ReadAllText(geoJsonFileName);
            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement root = document.RootElement;
            if (!root.TryGetProperty("type", out JsonElement rootType)
                || rootType.GetString() != "FeatureCollection")
                return false;

            if (!root.TryGetProperty("features", out JsonElement features)
                || features.ValueKind != JsonValueKind.Array
                || features.GetArrayLength() == 0)
                return false;

            JsonElement feature = features[0];
            if (!feature.TryGetProperty("type", out JsonElement featureType)
                || featureType.GetString() != "Feature")
                return false;

            if (!feature.TryGetProperty("properties", out JsonElement properties))
                return false;

            if (!properties.TryGetProperty("name", out JsonElement name)
                || name.GetString() != "test")
                return false;

            if (!properties.TryGetProperty("description", out JsonElement description)
                || description.GetString() != "test")
                return false;

            if (!properties.TryGetProperty("styleUrl", out JsonElement styleUrl)
                || styleUrl.GetString() != "styleUrl test")
                return false;

            if (!feature.TryGetProperty("geometry", out JsonElement geometry))
                return false;

            if (!geometry.TryGetProperty("type", out JsonElement geometryType)
                || geometryType.GetString() != "LineString")
                return false;

            if (!geometry.TryGetProperty("coordinates", out JsonElement coordinates)
                || coordinates.ValueKind != JsonValueKind.String
                || coordinates.GetString() != expectedCoordinates)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}