using Common;
using FastLoadImagesToMemoryAndProcessLater.Log;
using ImageHandling;

namespace Pics2gMaps;

public class ResizeImageDesktop(ILogger logger): ICommandHandler<ResizeImageDesktopCommand>
{
    public UpdateUi UpdateUi { get; set; }

    public void Execute(ResizeImageDesktopCommand command)
    {
        string galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        string folderName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
        string jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}Thumbs.json");
        string jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}.json");

        ResizeImage(jsonThumbsFileName, jsonPicsFileName, folderName, (bool)command.DataRow[DataTableConfigColumns.IsMerged]);
    }

    private void ResizeImage(string jsonThumbsFileName, string jsonPicsFileName, string folderName, bool isMerged)
    {
        if (File.Exists(jsonThumbsFileName))
        {
            File.Delete(jsonThumbsFileName);
        }

        if (File.Exists(jsonPicsFileName))
        {
            File.Delete(jsonPicsFileName);
        }

        string picsFolder = isMerged ? folderName : Path.Join(folderName, "pics");
        if (Directory.Exists(picsFolder))
        {
            //Parallel.ForEach(Directory.EnumerateFiles(picsFolder), imageFileName =>
            foreach (string imageFileName in Directory.GetFiles(picsFolder, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    if (!isMerged)
                    {
                        var resizeImageCommand = new ResizeImageCommand
                        {
                            CanvasHeight = 200,
                            CanvasWidth = 200,
                            OriginalFileName = Path.GetFileName(imageFileName),
                            //SaveTo = Path.Join(@"C:\projects\KanaloaGalleryTest\mariaLaach\thumbs", imageFileName)
                            SaveTo = Path.GetFileName(imageFileName)
                        };
                        resizeImageCommand.CreateDirectories(folderName);

                        ResizeImage resizeImage = new ResizeImage();
                        resizeImage.Execute(resizeImageCommand);
                    }

                    ExtractGpsInfoFromImage extractGpsInfoFromImage = new ExtractGpsInfoFromImage();
                    var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
                    {
                        ImageFileNameToReadGpsFrom = imageFileName
                    };
                    extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);

                    var updateOrCreateJsonFileWithListOfImagesCommand =
                        new UpdateOrCreateJsonFileWithListOfImagesCommand
                        {
                            FolderName = string.Empty,
                            LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                            ImageFileName = Path.GetFileName(imageFileName),
                            JsonThumbsFileName = jsonThumbsFileName,
                            JsonPicsFileName = jsonPicsFileName
                        };

                    UpdateOrCreateJsonFileWithListOfImages updateOrCreateJsonFileWithListOfImages =
                        new UpdateOrCreateJsonFileWithListOfImages(new UpdateJsonIfExistsOrCreateNewIfNot());
                    updateOrCreateJsonFileWithListOfImages.Execute(updateOrCreateJsonFileWithListOfImagesCommand);
                }
                catch (Exception ex)
                {
                    UpdateUi.Error = $"{imageFileName}: {ex.Message}";
                    logger.Log(UpdateUi);
                }
                //});
            }
        }
        else
        {
            logger.Log($"Folder {picsFolder} does not exist!");
        }
    }

}