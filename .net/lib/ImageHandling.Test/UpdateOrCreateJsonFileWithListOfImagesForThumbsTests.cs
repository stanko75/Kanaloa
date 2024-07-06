using Newtonsoft.Json.Linq;

namespace ImageHandling.Test;

[TestClass]
public class UpdateOrCreateJsonFileWithListOfImagesForThumbsTests
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
        UpdateOrCreateJsonFileWithListOfImagesForThumbs updateOrCreateJsonFileWithListOfImagesForThumbs =
            new UpdateOrCreateJsonFileWithListOfImagesForThumbs(updateJsonIfExistsOrCreateNewIfNot);

        var updateOrCreateJsonFileWithListOfImagesForThumbsCommand =
            new UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand
            {
                KmlFileName = "test.kml",
                FolderName = "testFolderName",
                LatLngModel = latLngModel,
                ImageFileName = "testImageFileName.jpg"
            };

        string jsonFileNameWithoutExtension =
            Path.GetFileNameWithoutExtension(updateOrCreateJsonFileWithListOfImagesForThumbsCommand.KmlFileName);
        string jsonFileName = Path.Join(updateOrCreateJsonFileWithListOfImagesForThumbsCommand.FolderName,
            Path.ChangeExtension($"{jsonFileNameWithoutExtension}Thumbs", "json"));
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }

        updateOrCreateJsonFileWithListOfImagesForThumbs.Execute(updateOrCreateJsonFileWithListOfImagesForThumbsCommand);
        string json = File.ReadAllText(jsonFileName);
        JArray result = JArray.Parse(json);
        Assert.AreEqual(result[0]["FileName"], @"..\..\testFolderName\thumbs\testImageFileName.jpg");
    }
}