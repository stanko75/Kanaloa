using Common;
using ImageHandling;
using System.Collections.Concurrent;
using System;

namespace Pics2gMaps;



public class ParallelForEachAndExtractGpsInfoWrapper(
    List<ITaskToExecuteWhenGpsIsExtracting> lstOfTasksToExecuteWhenGpsInfoWasExtracted
    , List<ITaskToExecuteWhenForEachIsDone>? lstOfTasksToExecuteWhenForEachIsDone) : ICommandHandlerAsync<ParallelForEachAndExtractGpsInfoWrapperCommand>
{

    ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();
    private int _recordCount;
    public int RecordCount
    {
        get => _recordCount;
        private set => _recordCount = value;
    }

    public async Task Execute(ParallelForEachAndExtractGpsInfoWrapperCommand parallelForEachAndExtractGpsInfoWrapperCommand)
    {
        if (Directory.Exists(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName))
        {
            var fileNames = new ConcurrentBag<string>();
            var latLngThumbsQueue = new ConcurrentBag<LatLngFileNameModel>();
            var latLngPicsQueue = new ConcurrentBag<LatLngFileNameModel>();

            List<Task> tasksToExecuteWhenGpsInfoWasExtracted = new List<Task>();
            List<Task> tasksToExecuteWhenForEachIsDone = new List<Task>();

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
                            foreach (var commandHandlerAsync in lstOfTasksToExecuteWhenGpsInfoWasExtracted)
                            {
                                //ToDo here invoke rather event
                                AddLatLngFileNameModelToQueue(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName, imageFileName, extractGpsInfoFromImageCommand, latLngThumbsQueue, "thumbs");
                                AddLatLngFileNameModelToQueue(parallelForEachAndExtractGpsInfoWrapperCommand.FolderName, imageFileName, extractGpsInfoFromImageCommand, latLngPicsQueue, "pics");
                                tasksToExecuteWhenGpsInfoWasExtracted.Add(commandHandlerAsync.Execute(extractGpsInfoFromImageCommand));
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

            //ToDo: also here rather event then list
            foreach (var taskToExecuteWhenForEachIsDone in lstOfTasksToExecuteWhenForEachIsDone)
            {
                tasksToExecuteWhenForEachIsDone.Add(taskToExecuteWhenForEachIsDone.Execute(latLngPicsQueue));
            }

            await Task.WhenAll(tasksToExecuteWhenGpsInfoWasExtracted);
            //foreach (var commandHandlerAsync in lstOfTasksToExecuteWhenGpsInfoWasExtracted)
            //{
            //    await commandHandlerAsync.Execute(command);
            //}
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory {parallelForEachAndExtractGpsInfoWrapperCommand.FolderName} not found.");
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

public interface ITaskToExecuteWhenGpsIsExtracting
{
    Task Execute(ExtractGpsInfoFromImageCommand extractGpsInfoFromImageCommand);
}

public interface ITaskToExecuteWhenForEachIsDone
{
    Task Execute(ConcurrentBag<LatLngFileNameModel> latLngThumbsQueue);
}