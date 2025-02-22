using Common;
using FastLoadImagesToMemoryAndProcessLater.Log;
using ImageHandling;
using System.Collections.Concurrent;

namespace Pics2gMaps;

public class ResizeImageDesktop/*(ILogger logger)*/ : ICommandHandlerAsync<ResizeImageDesktopCommand>
{
    public UpdateUi UpdateUi { get; set; }
    private int _recordCount;

    public int RecordCount { get; private set; }

    public async Task Execute(ResizeImageDesktopCommand command)
    {
        string galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        string folderName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
        string jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}Thumbs.json");
        string jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}.json");

        await ResizeImage(jsonThumbsFileName, jsonPicsFileName, folderName, (bool)command.DataRow[DataTableConfigColumns.IsMerged]);
    }

    private async Task ResizeImage(string jsonThumbsFileName, string jsonPicsFileName, string folderName, bool isMerged)
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
            var fileNames = new ConcurrentBag<string>();

            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(picsFolder, "*.*", SearchOption.AllDirectories).AsParallel(),
                async (imageFileName, cancellationToken) =>
                //foreach (string imageFileName in Directory.GetFiles(picsFolder, "*.*", SearchOption.AllDirectories))
            {
                ResizeImage(folderName, isMerged, imageFileName);
                ExtractGpsInfoFromImageAndUpdateOrCreateJsonFileWithListOfImages(jsonThumbsFileName, jsonPicsFileName, imageFileName);

                RecordCount = Interlocked.Increment(ref _recordCount);
            });

            await File.WriteAllLinesAsync(@"fileListMain.txt", fileNames);
            //}
        }
        else
        {
            //logger.Log($"Folder {picsFolder} does not exist!");
        }
    }

    private void ExtractGpsInfoFromImageAndUpdateOrCreateJsonFileWithListOfImages(string jsonThumbsFileName,
        string jsonPicsFileName, string imageFileName)
    {
        try
        {

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
            //var updateUi = new UpdateUi
            //{
            //    Form = UpdateUi.Form,
            //    TextBox = UpdateUi.TextBox,
            //    Error = $"{imageFileName}: {ex.Message}",
            //    Name = UpdateUi.Name
            //};

            //logger.Log(updateUi);
        }

    }

    private void ResizeImage(string folderName, bool isMerged, string imageFileName)
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
        }
        catch (Exception ex)
        {
            //var updateUi = new UpdateUi
            //{
            //    Form = UpdateUi.Form,
            //    TextBox = UpdateUi.TextBox,
            //    Error = $"{imageFileName}: {ex.Message}",
            //    Name = UpdateUi.Name
            //};

            //logger.Log(updateUi);
        }
    }
}