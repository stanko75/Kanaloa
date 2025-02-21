using Common;
using FastLoadImagesToMemoryAndProcessLater.Log;
using ImageHandling;
using System.Collections.Concurrent;

namespace Pics2gMaps;

public class ResizeImageDesktop(ILogger logger) : ICommandHandlerAsync<ResizeImageDesktopCommand>
{
    public UpdateUi UpdateUi { get; set; }
    private int _recordCount;
    public event EventHandler<int>? RecordCountChanged;

    public int RecordCount
    {
        get => _recordCount;
        private set
        {
            _recordCount = value;
            RaiseRecordCountChanged(_recordCount);
        }
        // Sicheres Event-Handling
    }

    private void RaiseRecordCountChanged(int count)
    {
        if (UpdateUi.Form is { IsHandleCreated: true, InvokeRequired: true })
        {
            UpdateUi.Form.BeginInvoke(() => RecordCountChanged?.Invoke(this, count));
        }
        else
        {
            RecordCountChanged?.Invoke(this, count);
        }
    }


    public async Task Execute(ResizeImageDesktopCommand command)
    {
        string galleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();
        string folderName =
            Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
        string jsonThumbsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(),
            $@"{galleryName}\www\{galleryName}Thumbs.json");
        string jsonPicsFileName = Path.Join(command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString(),
            $@"{galleryName}\www\{galleryName}.json");

        await ResizeImage(jsonThumbsFileName, jsonPicsFileName, folderName,
            (bool)command.DataRow[DataTableConfigColumns.IsMerged], command.FileQueue);
    }

    private async Task ResizeImage(string jsonThumbsFileName, string jsonPicsFileName, string folderName, bool isMerged,
        BlockingCollection<string>? fileQueue)
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
        if (Directory.Exists(picsFolder) && fileQueue is not null)
        {
            await Parallel.ForEachAsync(
                fileQueue.GetConsumingEnumerable().AsParallel(), async (imageFileName, cancellationToken) =>
                //foreach (string imageFileName in Directory.GetFiles(picsFolder, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        UpdateUi.Error = $"{imageFileName}";
                        logger.Log(UpdateUi);
                        Task resizeTask = Task.Run(() =>
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
                                UpdateUi.Error = $"{imageFileName}: {ex.Message}";
                                logger.Log(UpdateUi);
                            }
                        }, cancellationToken);

                        Task gpsAndJsonTask = Task.Run(() =>
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
                                UpdateUi.Error = $"{imageFileName}: {ex.Message}";
                                logger.Log(UpdateUi);
                            }
                        }, cancellationToken);

                        await Task.WhenAll(resizeTask, gpsAndJsonTask);
                    }
                    catch (Exception ex)
                    {
                        UpdateUi.Error = $"{imageFileName}: {ex.Message}";
                        logger.Log(UpdateUi);
                    }

                    RecordCount = Interlocked.Increment(ref _recordCount);
                });
            //}
        }
        else
        {
            logger.Log($"Folder {picsFolder} does not exist!");
        }
    }

}