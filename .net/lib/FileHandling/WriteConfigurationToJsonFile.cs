using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileHandling;

public class WriteConfigurationToJsonFile : ICommandHandlerAsync<WriteConfigurationToJsonFileCommand>
{
    public async Task Execute(WriteConfigurationToJsonFileCommand command)
    {
        LiveConfigModel liveExistingConfigModel;

        if (File.Exists(command.ConfigFileName))
        {
            //System.IO.IOException: 'The process cannot access the file 'C:\home\site\wwwroot\config.json' because it is being used by another process.'
            string strExistingConfig = await File.ReadAllTextAsync(command.ConfigFileName);
            JObject jsonObjectExistingConfig = JObject.Parse(strExistingConfig);
            liveExistingConfigModel = jsonObjectExistingConfig.ToObject<LiveConfigModel>() ??
                                      throw new InvalidOperationException();
        }
        else
        {
            liveExistingConfigModel = new LiveConfigModel();
        }

        liveExistingConfigModel.LiveImageMarkersJsonUrl =
            CommonStatic.ConvertRelativeWindowsPathToUri($"{command.WebFolderName}",
                    $"{Path.GetFileNameWithoutExtension(command.KmlFileName)}Thumbs.json")
                .AbsoluteUri;

        liveExistingConfigModel.KmlFileName =
            CommonStatic.ConvertRelativeWindowsPathToUri(command.WebFolderName, command.KmlFileName)
                .AbsoluteUri;

        if (!string.IsNullOrWhiteSpace(command.CurrentLocationFileName))
        {
            liveExistingConfigModel.CurrentLocation = CommonStatic.ConvertRelativeWindowsPathToUri(command.RootUrl,
                    command.CurrentLocationFileName)
                .AbsoluteUri;
        }

        string liveConfigJson = JsonConvert.SerializeObject(liveExistingConfigModel, Formatting.Indented);
        await File.WriteAllTextAsync(command.ConfigFileName, liveConfigJson);
    }
}