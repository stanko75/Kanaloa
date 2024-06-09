using Common;
using Newtonsoft.Json.Linq;

namespace FileHandling.Test;

[TestClass]
public class WriteConfigurationToJsonFileTests
{
    [TestMethod]
    public async Task CheckIfConfigurationFileWasProperlyCreated()
    {
        ICommandHandlerAsync<WriteConfigurationToJsonFileCommand> writeConfigurationToJsonFile =
            new WriteConfigurationToJsonFile();

        string rootUrl = "milosev.com";

        WriteConfigurationToJsonFileCommand command = new()
        {
            RootUrl = rootUrl
        };

        if (File.Exists(command.ConfigFileName))
        {
            File.Delete(command.ConfigFileName);
        }

        await writeConfigurationToJsonFile.Execute(command);

        JObject configJson = JObject.Parse(await File.ReadAllTextAsync(command.ConfigFileName));
        Assert.AreEqual(configJson["KmlFileName"], "http://milosev.com/default/default.kml");
        Assert.AreEqual(configJson["CurrentLocation"], "http://milosev.com/live.json");
        Assert.AreEqual(configJson["LiveImageMarkersJsonUrl"], "http://milosev.com/default/defaultThumbs.json");
    }
}