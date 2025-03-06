using System.Collections.Concurrent;
using System.Text.Json;
using Common;
using ImageHandling;

namespace Pics2gMaps;

public class ExtractGpsInfoAndResizeImageWrapper(ParallelForEachAndExtractGpsInfoWrapper parallelForEachAndExtractGpsInfoWrapper) : ICommandHandlerAsync<ExtractGpsInfoAndResizeImageWrapperCommand>
{
    private string? _galleryName;
    private string? _folderName;
    private string? _jsonThumbsFileName;
    private string? _jsonPicsFileName;
    private bool _isMerged;
    private readonly ConcurrentBag<LatLngFileNameModel> _latLngThumbsQueue = new();
    private readonly ConcurrentBag<LatLngFileNameModel> _latLngPicsQueue = new();
    public readonly ConcurrentQueue<Exception> _exceptions = new();

    public async Task Execute(ExtractGpsInfoAndResizeImageWrapperCommand command)
    {
        _galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        _folderName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), _galleryName);
        _jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{_galleryName}\www\{_galleryName}Thumbs.json");
        _jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{_galleryName}\www\{_galleryName}.json");

        if (File.Exists(_jsonThumbsFileName))
        {
            File.Delete(_jsonThumbsFileName);
        }

        if (File.Exists(_jsonPicsFileName))
        {
            File.Delete(_jsonPicsFileName);
        }

        if (!command.DataRow.IsNull(DataTableConfigColumns.IsMerged))
        {
            _isMerged = (bool)command.DataRow[DataTableConfigColumns.IsMerged];
        }
        else
        {
            _isMerged = false;
        }

        string picsFolder = _isMerged ? _folderName : Path.Join(_folderName, "pics");
        if (Directory.Exists(picsFolder))
        {
            var parallelForEachAndExtractGpsInfoWrapperCommand = new ParallelForEachAndExtractGpsInfoWrapperCommand
            {
                FolderName = _folderName
                , RecordCountProgress = command.RecordCountProgress
            };

            parallelForEachAndExtractGpsInfoWrapper.OnGpsInfoFromImageExtracted += OnGpsInfoFromImageExtracted;
            parallelForEachAndExtractGpsInfoWrapper.OnGpsInfoFromImageExtracted += OnAddLatLngFileNameModelToThumbsQueue;
            parallelForEachAndExtractGpsInfoWrapper.OnGpsInfoFromImageExtracted += OnAddLatLngFileNameModelToPicsQueue;
            try
            {
                await parallelForEachAndExtractGpsInfoWrapper.Execute(parallelForEachAndExtractGpsInfoWrapperCommand);
            }
            finally
            {
                await SaveAsJsonAsync(_jsonThumbsFileName, _latLngThumbsQueue);
                await SaveAsJsonAsync(_jsonPicsFileName, _latLngPicsQueue);

                if (!_exceptions.IsEmpty)
                {
                    throw new AggregateException("Error in der ResizeImage", _exceptions);
                }

            }
        }
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

    private void OnAddLatLngFileNameModelToPicsQueue(object? sender, GpsInfoFromImageExtractedEventArgs e)
    {
        AddLatLngFileNameModelToQueue(_folderName, e.LatLngFileName.FileName, e.LatLngFileName.Latitude, e.LatLngFileName.Longitude, _latLngPicsQueue, "pics");
    }

    private void OnAddLatLngFileNameModelToThumbsQueue(object? sender, GpsInfoFromImageExtractedEventArgs e)
    {
        AddLatLngFileNameModelToQueue(_folderName, e.LatLngFileName.FileName, e.LatLngFileName.Latitude, e.LatLngFileName.Longitude, _latLngThumbsQueue, "thumbs");
    }

    private async void OnGpsInfoFromImageExtracted(object? sender, GpsInfoFromImageExtractedEventArgs e)
    {
        await Task.Run(() => ResizeImage(_folderName, _isMerged, e.LatLngFileName.FileName));
    }

    private void ResizeImage(string? folderName, bool isMerged, string? imageFileName)
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
        catch (Exception e)
        {
            _exceptions.Enqueue(e);
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