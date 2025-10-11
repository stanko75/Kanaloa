using Common;
using ImageHandling;
using System.Collections.Concurrent;

namespace Pics2gMaps;

public class ParallelForEachAndExtractGpsInfoWrapper
    : ICommandHandlerAsync<ParallelForEachAndExtractGpsInfoWrapperCommand>
{

    public event EventHandler<GpsInfoFromImageExtractedEventArgs>? OnGpsInfoFromImageExtracted;

    public readonly ConcurrentQueue<Exception> _exceptions = new();
    private int _recordCount;

    public int RecordCount
    {
        get => _recordCount;
        private set => _recordCount = value;
    }

    protected virtual void GpsInfoFromImageExtracted(GpsInfoFromImageExtractedEventArgs e)
    {
        OnGpsInfoFromImageExtracted?.Invoke(this, e);
    }

    public async Task Execute(ParallelForEachAndExtractGpsInfoWrapperCommand parallelForEachAndExtractGpsInfoWrapperCommand)
    {
        if (Directory.Exists(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName))
        {
            var imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp"
            };
            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName, "*.*",
                    SearchOption.AllDirectories), async (imageFileName, ct) =>
                {
                    if (imageExtensions.Contains(Path.GetExtension(imageFileName).ToLower()))
                    {
                        ExtractGpsInfoFromImageAndFireEvent(parallelForEachAndExtractGpsInfoWrapperCommand, imageFileName);
                    }
                });

            if (!_exceptions.IsEmpty)
            {
                throw new AggregateException("Error in der Parallel.ForEachAsync", _exceptions);
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory {parallelForEachAndExtractGpsInfoWrapperCommand.FolderName} not found.");
        }

    }

    private void ExtractGpsInfoFromImageAndFireEvent(
        ParallelForEachAndExtractGpsInfoWrapperCommand parallelForEachAndExtractGpsInfoWrapperCommand, string imageFileName)
    {
        LatLngFileNameModel latLngFileNameModel = new LatLngFileNameModel();
        ExtractGpsInfoFromImage extractGpsInfoFromImage = new ExtractGpsInfoFromImage();
        var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
        {
            ImageFileNameToReadGpsFrom = imageFileName
        };
        parallelForEachAndExtractGpsInfoWrapperCommand.ImageFileNameToReadGpsFrom = imageFileName;

        try
        {
            extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);
            if (extractGpsInfoFromImageCommand.LatLngModel != null)
            {
                latLngFileNameModel.FileName = imageFileName;
                latLngFileNameModel.Latitude = extractGpsInfoFromImageCommand.LatLngModel.Latitude;
                latLngFileNameModel.Longitude = extractGpsInfoFromImageCommand.LatLngModel.Longitude;
                GpsInfoFromImageExtracted(new GpsInfoFromImageExtractedEventArgs(latLngFileNameModel));
            }
            else
            {
                throw new Exception($"No GPS info found in {imageFileName}");
            }
        }
        catch (Exception e)
        {
            _exceptions.Enqueue(e);
        }

        RecordCount = Interlocked.Increment(ref _recordCount);
        //if (RecordCount % progressStep == 0)
        {
            parallelForEachAndExtractGpsInfoWrapperCommand.RecordCountProgress?.Report(RecordCount);
        }
    }
}