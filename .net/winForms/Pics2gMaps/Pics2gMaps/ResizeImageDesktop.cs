﻿using Common;
using FastLoadImagesToMemoryAndProcessLater.Log;
using ImageHandling;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;

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
            var latLngQueue = new ConcurrentBag<LatLngFileNameModel>();

            await Parallel.ForEachAsync(
                Directory.EnumerateFiles(picsFolder, "*.*", SearchOption.AllDirectories).AsParallel(),
                async (imageFileName, cancellationToken) =>
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
                            CreateRelativeWebPath(imageFileName, "thumbs", folderName);
                            LatLngFileNameModel latLngFileNameModel = new LatLngFileNameModel
                            {
                                FileName = CreateRelativeWebPath(imageFileName, "thumbs", folderName),
                                Latitude = extractGpsInfoFromImageCommand.LatLngModel.Latitude,
                                Longitude = extractGpsInfoFromImageCommand.LatLngModel.Longitude
                            };
                            latLngQueue.Add(latLngFileNameModel);

                            ResizeImage(folderName, isMerged, imageFileName);
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        //throw;
                    }

                    //ExtractGpsInfoFromImageAndUpdateOrCreateJsonFileWithListOfImages(jsonThumbsFileName, jsonPicsFileName, imageFileName);

                    RecordCount = Interlocked.Increment(ref _recordCount);
                    fileNames.Add($"{RecordCount}. {imageFileName}");
                });

            await File.WriteAllLinesAsync(@"fileListMain.txt", fileNames);
            await SaveAsJsonAsync(@"test.json", latLngQueue);
            //}
        }
        else
        {
            //logger.Log($"Folder {picsFolder} does not exist!");
        }
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