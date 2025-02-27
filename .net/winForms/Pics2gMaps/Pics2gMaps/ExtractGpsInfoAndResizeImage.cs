using System.Collections.Concurrent;
using System.Data;
using System.Text.Json;
using Common;
using ImageHandling;

namespace Pics2gMaps;

public class ExtractGpsInfoAndResizeImageWrapper : ICommandHandlerAsync<ExtractGpsInfoAndResizeImageWrapperCommand>
{
    public async Task Execute(ExtractGpsInfoAndResizeImageWrapperCommand command)
    {
        string galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        string folderName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
        string jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}Thumbs.json");
        string jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}.json");

        if (File.Exists(jsonThumbsFileName))
        {
            File.Delete(jsonThumbsFileName);
        }

        if (File.Exists(jsonPicsFileName))
        {
            File.Delete(jsonPicsFileName);
        }

        bool isMerged = (bool)command.DataRow[DataTableConfigColumns.IsMerged];
        string picsFolder = isMerged ? folderName : Path.Join(folderName, "pics");
        if (Directory.Exists(picsFolder))
        {
            var parallelForEachAndExtractGpsInfoWrapperCommand = new ParallelForEachAndExtractGpsInfoWrapperCommand();
            parallelForEachAndExtractGpsInfoWrapperCommand.FolderName = folderName;
            ExtractGpsInfoAndResizeImage extractGpsInfoAndResizeImage = new ExtractGpsInfoAndResizeImage();
            extractGpsInfoAndResizeImage.FolderName = folderName;
            extractGpsInfoAndResizeImage.IsMerged = isMerged;
            command.LstOfTasksToExecuteWhenGpsInfoWasExtracted.Add(extractGpsInfoAndResizeImage);

            var parallelForEachAndExtractGpsInfoWrapper =
                new ParallelForEachAndExtractGpsInfoWrapper(command.LstOfTasksToExecuteWhenGpsInfoWasExtracted, null);
            await parallelForEachAndExtractGpsInfoWrapper.Execute(parallelForEachAndExtractGpsInfoWrapperCommand);
        }

        /*
        var t = new ParallelForEachAndExtractGpsInfoWrapper();
        await t.Execute(command);
        */
    }
}

public class ExtractGpsInfoAndResizeImage : ITaskToExecuteWhenGpsIsExtracting
{
    public bool IsMerged;
    public string? FolderName;
    public ConcurrentBag<LatLngFileNameModel> LatLngThumbsQueue = [];
    public async Task Execute(ExtractGpsInfoFromImageCommand command)
    {
        LatLngFileNameModel latLngFileNameModel = new LatLngFileNameModel();
        latLngFileNameModel.FileName = command.ImageFileNameToReadGpsFrom;
        latLngFileNameModel.Latitude = command.LatLngModel.Latitude;
        latLngFileNameModel.Longitude = command.LatLngModel.Longitude;
        LatLngThumbsQueue.Add(latLngFileNameModel);
        await Task.Run(() => ResizeImage(FolderName, IsMerged, command.ImageFileNameToReadGpsFrom));
    }

    private void ResizeImage(string? folderName, bool isMerged, string? imageFileName)
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
}

public class SaveToJson: ITaskToExecuteWhenForEachIsDone
{
    public string? jsonThumbsFileName;
    public string? jsonPicsFileName;

    public async Task Execute(ExtractGpsInfoFromImageCommand extractGpsInfoFromImageCommand)
    {
    }

    static async Task SaveAsJsonAsync(string filePath, IEnumerable<LatLngFileNameModel> data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        await using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, options);
    }

    public async Task Execute(ConcurrentBag<LatLngFileNameModel> latLngThumbsQueue)
    {
        await SaveAsJsonAsync(jsonThumbsFileName, latLngThumbsQueue);
        await SaveAsJsonAsync(jsonPicsFileName, latLngPicsQueue);
    }
}

