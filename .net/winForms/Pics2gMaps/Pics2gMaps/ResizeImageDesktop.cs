using Common;
using ImageHandling;
using System.Collections.Concurrent;
using System.Text.Json;
using Timer = System.Threading.Timer;

namespace Pics2gMaps;

public class ResizeImageDesktop() : ICommandHandlerAsync<ResizeImageDesktopCommand>, IDisposable
{
    public event EventHandler<FilesProcessedEventArgs>? FilesProcessed;
    private int _recordCount;
    ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();

    private readonly ConcurrentQueue<int> _queue = new();
    private readonly Timer? _timer;

    public ResizeImageDesktop(Form form) : this()
    {
        _timer = new Timer(_ =>
        {
            if (_queue.TryDequeue(out int item))
            {
                FilesProcessed?.Invoke(this, new FilesProcessedEventArgs(_recordCount));
            }
        }, null, 0, 100);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public int RecordCount
    {
        get => _recordCount;
        private set => _recordCount = value;
    }

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
            var latLngThumbsQueue = new ConcurrentBag<LatLngFileNameModel>();
            var latLngPicsQueue = new ConcurrentBag<LatLngFileNameModel>();

            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(picsFolder, "*.*", SearchOption.AllDirectories).AsParallel(), (imageFileName, cancellationToken) =>
                //foreach (string imageFileName in Directory.GetFiles(picsFolder, "*.*", SearchOption.AllDirectories))
                {
                    ExtractGpsInfoFromImage extractGpsInfoFromImage = new ExtractGpsInfoFromImage();
                    var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
                    {
                        ImageFileNameToReadGpsFrom = imageFileName
                    };

                    try
                    {
                        extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);
                        if (extractGpsInfoFromImageCommand.LatLngModel != null)
                        {
                            AddLatLngFileNameModelToQueue(folderName, imageFileName, extractGpsInfoFromImageCommand, latLngThumbsQueue, "thumbs");
                            AddLatLngFileNameModelToQueue(folderName, imageFileName, extractGpsInfoFromImageCommand, latLngPicsQueue, "pics");
                            ResizeImage(folderName, isMerged, imageFileName);
                        }
                    }
                    catch (Exception e)
                    {
                       _exceptions.Enqueue(e);
                    }

                    //ExtractGpsInfoFromImageAndUpdateOrCreateJsonFileWithListOfImages(jsonThumbsFileName, jsonPicsFileName, imageFileName);

                    RecordCount = Interlocked.Increment(ref _recordCount);
                    _queue.Enqueue(RecordCount);
                    fileNames.Add($"{RecordCount}. {imageFileName}");
                    return ValueTask.CompletedTask;
                });

            await File.WriteAllLinesAsync(@"fileListMain.txt", fileNames);
            await SaveAsJsonAsync(jsonThumbsFileName, latLngThumbsQueue);
            await SaveAsJsonAsync(jsonPicsFileName, latLngPicsQueue);
            //}
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory {picsFolder} not found.");
        }

        if (!_exceptions.IsEmpty)
        {
            throw new AggregateException(_exceptions);
        }
    }

    private void AddLatLngFileNameModelToQueue(string folderName, string imageFileName,
        ExtractGpsInfoFromImageCommand extractGpsInfoFromImageCommand, ConcurrentBag<LatLngFileNameModel> latLngThumbsQueue, string folderThumbsOrPicsName)
    {
        LatLngFileNameModel latLngFileNameThumbsModel = new LatLngFileNameModel
        {
            FileName = CreateRelativeWebPath(imageFileName, folderThumbsOrPicsName, folderName),
            Latitude = extractGpsInfoFromImageCommand.LatLngModel.Latitude,
            Longitude = extractGpsInfoFromImageCommand.LatLngModel.Longitude
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

    static async Task SaveAsJsonAsync(string filePath, IEnumerable<LatLngFileNameModel> data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        await using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, options);
    }

    private void ResizeImage(string folderName, bool isMerged, string imageFileName)
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