using Newtonsoft.Json.Linq;

namespace ImageHandling.Test;

[TestClass]
public class UpdateJsonIfExistsOrCreateNewIfNotTests
{
    [TestMethod]
    public void CheckJsonWithLatLngAndFileName()
    {
        LatLngModel latLngModel = new LatLngModel
        {
            Latitude = 1,
            Longitude = 2
        };

        UpdateJsonIfExistsOrCreateNewIfNot updateJsonIfExistsOrCreateNewIfNot = new UpdateJsonIfExistsOrCreateNewIfNot();
        var updateOrCreateJsonFileWithListOfImagesCommand = new UpdateJsonIfExistsOrCreateNewIfNotCommand
        {
            LatLngModel = latLngModel,
            ImageFileName = "testImageFileName",
            JsonFileName = "testJsonFileName.json"
        };

        if (File.Exists(updateOrCreateJsonFileWithListOfImagesCommand.JsonFileName))
        {
            File.Delete(updateOrCreateJsonFileWithListOfImagesCommand.JsonFileName);
        }

        updateJsonIfExistsOrCreateNewIfNot.Execute(updateOrCreateJsonFileWithListOfImagesCommand);

        string json = File.ReadAllText(updateOrCreateJsonFileWithListOfImagesCommand.JsonFileName);
        JArray result = JArray.Parse(json);
        Assert.AreEqual(result[0]["FileName"], "testImageFileName");
    }
}