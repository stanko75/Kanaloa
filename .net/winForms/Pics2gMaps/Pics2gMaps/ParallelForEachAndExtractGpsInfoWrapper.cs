using Common;
using ImageHandling;
using System.Collections.Concurrent;

namespace Pics2gMaps;

public class ParallelForEachAndExtractGpsInfoWrapper : ICommandHandlerAsync<ParallelForEachAndExtractGpsInfoWrapperCommand>
{

    public event EventHandler<GpsInfoFromImageExtractedEventArgs>? OnGpsInfoFromImageExtracted;

    ConcurrentQueue<Exception> _exceptions = new();
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
            LatLngFileNameModel latLngFileNameModel = new LatLngFileNameModel();

            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName, "*.*", SearchOption.AllDirectories), (imageFileName, cancellationToken) =>
                {
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
                    return ValueTask.CompletedTask;
                });
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory {parallelForEachAndExtractGpsInfoWrapperCommand.FolderName} not found.");
        }

    }
}