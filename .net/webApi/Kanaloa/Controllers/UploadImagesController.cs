using Common;
using ImageHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using static Kanaloa.Common;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadImagesController(
    IOptions<KanaloaSettings> kanaloaSettings
    , ICommandHandler<ResizeImageCommand> resizeImage
    , ICommandHandler<ExtractGpsInfoFromImageCommand> extractGpsInfoFromImage
    , ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand> updateOrCreateJsonFileWithListOfImagesForThumbs
    , ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesCommand> updateOrCreateJsonFileWithListOfImages)
    : ControllerBase
{

    private readonly KanaloaSettings _kanaloaSettings = kanaloaSettings.Value;

    [HttpPost]
    [Route("UploadImage")]
    public async Task<IActionResult> UploadImage([FromBody] JObject data)
    {
        try
        {
            string folderName = GetValue(data, "folderName");
            string kmlFileName = GetValue(data, "kmlFileName");

            string imageFileName = data["imageFileName"]?.ToString() ?? "default.jpg";

            var resizeImageCommand = new ResizeImageCommand
            {
                CanvasHeight = 200,
                CanvasWidth = 200,
                OriginalFileName = imageFileName,
                SaveTo = imageFileName
            };
            resizeImageCommand.CreateDirectories(folderName);

            string nameOfFileForJson =
                $"{_kanaloaSettings.RootUrl}/{resizeImageCommand.SaveTo.Replace('\\', '/')}/{imageFileName}";
            //ToDo decorate
            string base64Image = data["base64Image"]?.ToString() ?? string.Empty;
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            await System.IO.File.WriteAllBytesAsync(resizeImageCommand.OriginalFileName, imageBytes);

            resizeImage.Execute(resizeImageCommand);

            var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
            {
                ImageFileNameToReadGpsFrom = resizeImageCommand.OriginalFileName
            };
            extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);

            var updateOrCreateJsonFileWithListOfImagesForThumbsCommand =
                new UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand
                {
                    KmlFileName = kmlFileName,
                    FolderName = folderName,
                    LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                    ImageFileName = imageFileName
                };
            updateOrCreateJsonFileWithListOfImagesForThumbs.Execute(
                updateOrCreateJsonFileWithListOfImagesForThumbsCommand);

            var updateOrCreateJsonFileWithListOfImagesCommand = new UpdateOrCreateJsonFileWithListOfImagesCommand
            {
                KmlFileName = kmlFileName,
                FolderName = folderName,
                LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                ImageFileName = imageFileName
            };
            updateOrCreateJsonFileWithListOfImages.Execute(updateOrCreateJsonFileWithListOfImagesCommand);

            return Ok(new
            {
                message =
                    $"Image uploaded to {Path.GetFullPath(resizeImageCommand.OriginalFileName)}" +
                    $"{Environment.NewLine}" +
                    $"***" +
                    $"{Environment.NewLine}" +

                    //$"JSON file saved in {Path.GetFullPath(fileNameThumbsJson)}" +
                    //$"{Environment.NewLine}" +
                    //$"***" +

                    $"{Environment.NewLine}" +
                    $"ImageThumbsFileName file saved in {Path.GetFullPath(resizeImageCommand.SaveTo)}"
            });
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }
}