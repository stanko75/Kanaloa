using Newtonsoft.Json.Linq;

namespace FileHandling.Test;

[TestClass]
public class AddFileWithLastKnownGpsPositionTests
{
    [TestMethod]
    public async Task CheckIfFileWithLastKnownGpsPositionWasProperlyCreated()
    {
        AddFileWithLastKnownGpsPositionCommand command = new()
        {
            GpsCommand = new GpsCommand("50.4040365", "7.2553499", "100")
        };

        if (File.Exists(command.CurrentLocationFileName))
        {
            File.Delete(command.CurrentLocationFileName);
        }

        AddFileWithLastKnownGpsPosition addFileWithLastKnownGpsPosition = new();
        await addFileWithLastKnownGpsPosition.Execute(command);
        JObject configJson = JObject.Parse(await File.ReadAllTextAsync(command.CurrentLocationFileName));
        Assert.AreEqual(configJson["lat"], 50.4040365);
        Assert.AreEqual(configJson["lng"], 7.2553499);
        Assert.AreEqual(configJson["alt"], 100);
    }
}