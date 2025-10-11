using Common;

namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImages(ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand> updateJsonIfExistsOrCreateNewIfNot) : ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesCommand>
{
    public void Execute(UpdateOrCreateJsonFileWithListOfImagesCommand command)
    {
        string imageThumbsFolderName = Path.Join(command.FolderName, "thumbs");
        string imagePicsFolderName = Path.Join(command.FolderName, "pics");
        string imageThumbsFileName;
        string imagePicsFileName = string.Empty;

        string jsonFileName;
        if (command.JsonThumbsFileName is null) //live
        {
            jsonFileName = Path.GetFileNameWithoutExtension(command.KmlFileName);
            jsonFileName = string.IsNullOrWhiteSpace(command.KmlFileName) ? "default" : jsonFileName;
            jsonFileName = Path.ChangeExtension(jsonFileName, "json");
            jsonFileName = Path.Join(command.FolderName, jsonFileName);
            imageThumbsFileName = $"../../{imageThumbsFolderName.Replace('\\', '/')}/{command.ImageFileName}";
            imagePicsFileName = $"../../{imagePicsFolderName.Replace('\\', '/')}/{command.ImageFileName}";
        }
        else //desktop
        {
            jsonFileName = command.JsonThumbsFileName;
            imageThumbsFileName = $"../{imageThumbsFolderName.Replace('\\', '/')}/{command.ImageFileName}";
            imagePicsFileName = $"../{imagePicsFolderName.Replace('\\', '/')}/{command.ImageFileName}";
        }

        UpdateJsonIfExistsOrCreateNewIfNotCommand updateJsonIfExistsOrCreateNewIfNotCommandThumbs = new UpdateJsonIfExistsOrCreateNewIfNotCommand
        {
            LatLngModel = command.LatLngModel,
            JsonFileName = jsonFileName,
            ImageFileName = imagePicsFileName
        };
        updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommandThumbs);

        if (command.JsonPicsFileName is not null) //desktop
        {
            UpdateJsonIfExistsOrCreateNewIfNotCommand updateJsonIfExistsOrCreateNewIfNotCommandPics = new UpdateJsonIfExistsOrCreateNewIfNotCommand
            {
                LatLngModel = command.LatLngModel,
                JsonFileName = command.JsonPicsFileName,
                ImageFileName = imagePicsFileName
            };
            updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommandPics);
        }
    }
}