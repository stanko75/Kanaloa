using Common;
using ImageHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadImagesController(
    IOptions<KanaloaSettings> kanaloaSettings
    , ICommandHandlerAsync<PrepareToResizeImageDecoratorCommand> prepareToResizeImageDecorator
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
            string kmlFileName = CommonStaticMethods.GetValue(data, "kmlFileName");

            var prepareToResizeImageDecoratorCommand = new PrepareToResizeImageDecoratorCommand
            {
                Data = data
            };
            await prepareToResizeImageDecorator.Execute(prepareToResizeImageDecoratorCommand);

            var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
            {
                ImageFileNameToReadGpsFrom = prepareToResizeImageDecoratorCommand.ImageFileNameWithFullPath
            };
            extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);

            var updateOrCreateJsonFileWithListOfImagesForThumbsCommand =
                new UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand
                {
                    KmlFileName = kmlFileName,
                    FolderName = prepareToResizeImageDecoratorCommand.FolderName,
                    LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                    ImageFileName = prepareToResizeImageDecoratorCommand.ImageFileName
                };
            updateOrCreateJsonFileWithListOfImagesForThumbs.Execute(
                updateOrCreateJsonFileWithListOfImagesForThumbsCommand);

            var updateOrCreateJsonFileWithListOfImagesCommand = new UpdateOrCreateJsonFileWithListOfImagesCommand
            {
                KmlFileName = kmlFileName,
                FolderName = prepareToResizeImageDecoratorCommand.FolderName,
                LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                ImageFileName = prepareToResizeImageDecoratorCommand.ImageFileName
            };
            updateOrCreateJsonFileWithListOfImages.Execute(updateOrCreateJsonFileWithListOfImagesCommand);

            return Ok(new
            {
                message =
                    $"Image uploaded to {Path.GetFullPath(prepareToResizeImageDecoratorCommand.ImageFileNameWithFullPath)}" +
                    $"{Environment.NewLine}" +
                    $"***" +
                    $"{Environment.NewLine}" +

                    //$"JSON file saved in {Path.GetFullPath(fileNameThumbsJson)}" +
                    //$"{Environment.NewLine}" +
                    //$"***" +

                    $"{Environment.NewLine}" +
                    $"ImageThumbsFileName file saved in {Path.GetFullPath(prepareToResizeImageDecoratorCommand.ImageFileNameWithFullPath)}"
            });
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }
}