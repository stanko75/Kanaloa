using System.Collections.Concurrent;
using Common;
using ImageHandling;

namespace Pics2gMaps;

public class ExtractGpsInfoAndResizeImageWrapper : ICommandHandlerAsync<ExtractGpsInfoAndResizeImageWrapperCommand>
{
    private string galleryName;
    private string _folderName;
    private string jsonThumbsFileName;
    private string jsonPicsFileName;
    private bool isMerged;
    private ConcurrentBag<LatLngFileNameModel> _latLngThumbsQueue = new();

    public async Task Execute(ExtractGpsInfoAndResizeImageWrapperCommand command)
    {
        galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        _folderName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
        jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}Thumbs.json");
        jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}.json");

        if (File.Exists(jsonThumbsFileName))
        {
            File.Delete(jsonThumbsFileName);
        }

        if (File.Exists(jsonPicsFileName))
        {
            File.Delete(jsonPicsFileName);
        }

        isMerged = (bool)command.DataRow[DataTableConfigColumns.IsMerged];
        string picsFolder = isMerged ? _folderName : Path.Join(_folderName, "pics");
        if (Directory.Exists(picsFolder))
        {
            var parallelForEachAndExtractGpsInfoWrapperCommand = new ParallelForEachAndExtractGpsInfoWrapperCommand
            {
                FolderName = _folderName
            };
            var parallelForEachAndExtractGpsInfoWrapper =
                new ParallelForEachAndExtractGpsInfoWrapper();
            parallelForEachAndExtractGpsInfoWrapper.OnGpsInfoFromImageExtracted += OnGpsInfoFromImageExtracted;
            await parallelForEachAndExtractGpsInfoWrapper.Execute(parallelForEachAndExtractGpsInfoWrapperCommand);
        }
    }

    private async void OnGpsInfoFromImageExtracted(object? sender, GpsInfoFromImageExtractedEventArgs e)
    {
        await Task.Run(() => ResizeImage(_folderName, isMerged, e.LatLngFileName.FileName));
        AddLatLngFileNameModelToQueue(_folderName, e.LatLngFileName.FileName, e.LatLngFileName.Latitude, e.LatLngFileName.Longitude, _latLngThumbsQueue, "thumbs");
        AddLatLngFileNameModelToQueue(_folderName, e.LatLngFileName.FileName, e.LatLngFileName.Latitude, e.LatLngFileName.Longitude, _latLngThumbsQueue, "pics");
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

    private void AddLatLngFileNameModelToQueue
    (
        string folderName
        , string imageFileName
        , double latitude
        , double longitude
        , ConcurrentBag<LatLngFileNameModel> latLngThumbsQueue
        , string folderThumbsOrPicsName
    )
    {
        LatLngFileNameModel latLngFileNameThumbsModel = new LatLngFileNameModel
        {
            FileName = CreateRelativeWebPath(imageFileName, folderThumbsOrPicsName, folderName),
            Latitude = latitude,
            Longitude = longitude
        };
        latLngThumbsQueue.Add(latLngFileNameThumbsModel);
    }

    private string CreateRelativeWebPath(string fullImageFileName, string folderName, string baseGalleryPath)
    {
        string baseFolderToCreateRelativeFrom = Directory.GetParent(fullImageFileName)?.FullName ?? string.Empty;
        baseFolderToCreateRelativeFrom = Directory.GetParent(baseFolderToCreateRelativeFrom)?.FullName ?? string.Empty;
        baseFolderToCreateRelativeFrom = Path.Join(baseFolderToCreateRelativeFrom, folderName);
        baseFolderToCreateRelativeFrom = Path.Join(baseFolderToCreateRelativeFrom, Path.GetFileName(fullImageFileName));
        //string basePath = @"C:\projects\KanaloaGalleryTest\gallery\allWithPics";
        string relativePath = Path.GetRelativePath(baseGalleryPath, baseFolderToCreateRelativeFrom);
        return $"../{relativePath.Replace("\\", "/")}";
    }

}