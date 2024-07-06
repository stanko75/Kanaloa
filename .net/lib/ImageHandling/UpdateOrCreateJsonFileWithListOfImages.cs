using Common;

namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImages(ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand> updateJsonIfExistsOrCreateNewIfNot) : ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesCommand>
{
    public void Execute(UpdateOrCreateJsonFileWithListOfImagesCommand command)
    {
        var jsonFileName = Path.GetFileNameWithoutExtension(command.KmlFileName);
        jsonFileName = string.IsNullOrWhiteSpace(command.KmlFileName) ? "default" : jsonFileName;
        jsonFileName = Path.ChangeExtension(jsonFileName, "json");
        jsonFileName = Path.Join(command.FolderName, jsonFileName);

        string imageThumbsFolderName = Path.Join(command.FolderName, "pics");
        string imageFileName = $"../../{imageThumbsFolderName.Replace('\\', '/')}/{command.ImageFileName}";

        UpdateJsonIfExistsOrCreateNewIfNotCommand updateJsonIfExistsOrCreateNewIfNotCommand = new UpdateJsonIfExistsOrCreateNewIfNotCommand
        {
            LatLngModel = command.LatLngModel,
            JsonFileName = jsonFileName,
            ImageFileName = imageFileName
        };

        updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommand);
    }
}