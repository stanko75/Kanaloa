using Common;
using ImageHandling;
using System.Collections.Concurrent;
using System;

namespace Pics2gMaps;

public class ParallelForEachAndExtractGpsInfoWrapper<TCommand>(ICommandHandlerAsync<TCommand>[] arrayOfTasksToExecuteWhenGpsInfoWasExtracted, ICommandHandlerAsync<TCommand>[] arrayOfTasksToExecuteWhenForEachIsDone) : ICommandHandlerAsync<TCommand>
{
    public string FolderName { get; set; }

    ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();
    private int _recordCount;
    public int RecordCount
    {
        get => _recordCount;
        private set => _recordCount = value;
    }

    public async Task Execute(TCommand command)
    {
        if (Directory.Exists(FolderName))
        {
            var fileNames = new ConcurrentBag<string>();

            List<Task> tasksToExecuteWhenGpsInfoWasExtracted = new List<Task>();

            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(FolderName, "*.*", SearchOption.AllDirectories), (imageFileName, cancellationToken) =>
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
                            foreach (ICommandHandlerAsync<TCommand> commandHandlerAsync in arrayOfTasksToExecuteWhenGpsInfoWasExtracted)
                            {
                                tasksToExecuteWhenGpsInfoWasExtracted.Add(commandHandlerAsync.Execute(command));
                            }
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
                    fileNames.Add($"{RecordCount}. {imageFileName}");
                    return ValueTask.CompletedTask;
                });

            await Task.WhenAll(tasksToExecuteWhenGpsInfoWasExtracted);
            foreach (ICommandHandlerAsync<TCommand> commandHandlerAsync in arrayOfTasksToExecuteWhenGpsInfoWasExtracted)
            {
                await commandHandlerAsync.Execute(command);
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory {FolderName} not found.");
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
}