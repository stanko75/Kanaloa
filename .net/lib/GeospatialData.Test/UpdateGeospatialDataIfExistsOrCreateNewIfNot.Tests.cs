using Common;
using GeoJsonHandling;
using KmlHandling;
using static GeoJsonHandling.Test.GeoJsonTests;
using static KmlHandling.Test.KmlTests;

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

        if (File.Exists(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName))
        {
            File.Delete(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName);
        }

        CreateGeoJson createJson = new CreateGeoJson();
        UpdateGeospatialDataIfExistsOrCreateNewIfNot updateGeospatialDataIfExistsOrCreateNewIfNot = new UpdateGeospatialDataIfExistsOrCreateNewIfNot(null, 
            createJson);
        await updateGeospatialDataIfExistsOrCreateNewIfNot.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);

        Assert.IsTrue(IsExpectedGeoJson(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName));
    }

    [TestMethod]
    public async Task CreateKml()
    {
        UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand =
            new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
            {
                Coordinates = "6.641421,43.1869746,2357",
                GeospatialDataFileName = _geospatialDataFileName,
                GeospatialDataFileNameExtension = "kml",
                FolderName = _testFolder
            };

        if (File.Exists(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName))
        {
            File.Delete(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName);
        }

        CreateKml createKml = new CreateKml();
        UpdateGeospatialDataIfExistsOrCreateNewIfNot updateGeospatialDataIfExistsOrCreateNewIfNot = new UpdateGeospatialDataIfExistsOrCreateNewIfNot(null,
            createKml);
        await updateGeospatialDataIfExistsOrCreateNewIfNot.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);

        Assert.IsTrue(IsExpectedKml(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName));
    }

}