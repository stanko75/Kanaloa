using Common;

namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImagesForThumbs(ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand> updateJsonIfExistsOrCreateNewIfNot) : ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand>
{
    public void Execute(UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand command)
    {
        var jsonFileName = Path.GetFileNameWithoutExtension(command.KmlFileName);
        jsonFileName = string.IsNullOrWhiteSpace(command.KmlFileName) ? "default" : jsonFileName;
        jsonFileName = Path.ChangeExtension($"{jsonFileName}Thumbs", "json");
        jsonFileName = Path.Join(command.FolderName, jsonFileName);

        string imageThumbsFolderName = Path.Join(command.FolderName, "thumbs");
        string imageFileName = $@"..\..\{imageThumbsFolderName}\{command.ImageFileName}";

        UpdateJsonIfExistsOrCreateNewIfNotCommand updateJsonIfExistsOrCreateNewIfNotCommand = new UpdateJsonIfExistsOrCreateNewIfNotCommand
        {
            LatLngModel = command.LatLngModel,
            JsonFileName = jsonFileName,
            ImageFileName = imageFileName
        };

        updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommand);
    }
}