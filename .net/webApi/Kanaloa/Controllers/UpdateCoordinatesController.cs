using Common;
using FileHandling;
using ImageHandling;
using KmlHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UpdateCoordinatesController(
    IOptions<KanaloaSettings> kanaloaSettings
    , ISaveKmlUpdateLivePositionSaveConfigFile saveKmlUpdateLivePositionSaveConfigFile
    , ICommandHandler<ResizeImageCommand> resizeImage
    , ICommandHandler<ExtractGpsInfoFromImageCommand> extractGpsInfoFromImage
    , ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand> updateJsonIfExistsOrCreateNewIfNot
    )
    : ControllerBase
{
    private readonly KanaloaSettings _kanaloaSettings = kanaloaSettings.Value;

    // POST api/<UpdateCoordinatesController>
    [HttpPost]
    [Route("PostFileFolder")]
    public async Task<IActionResult> PostFileFolder([FromBody] JObject data)
    {
        try
        {
            string kmlFileName = GetValue(data, "kmlFileName");
            string folderName = GetValue(data, "folderName");
            string latitude = GetValue(data, "Latitude");
            string longitude = GetValue(data, "Longitude");
            string coordinates = string.Join(',', latitude, longitude, "2357");

            var gps = new GpsCommand(latitude, longitude);
            var addFileWithLastKnownGpsPositionCommand = new AddFileWithLastKnownGpsPositionCommand
            {
                GpsCommand = gps
            };

            var writeConfigurationToJsonFileCommand = new WriteConfigurationToJsonFileCommand
            {
                RootUrl = _kanaloaSettings.RootUrl,
                KmlFileName = kmlFileName,
                WebFolderName = folderName
            };

            var updateKmlIfExistsOrCreateNewIfNotCommand = new UpdateKmlIfExistsOrCreateNewIfNotCommand
            {
                KmlFileName = kmlFileName,
                Coordinates = coordinates,
                FolderName = folderName
            };

            await saveKmlUpdateLivePositionSaveConfigFile.Execute(updateKmlIfExistsOrCreateNewIfNotCommand
                , addFileWithLastKnownGpsPositionCommand
                , writeConfigurationToJsonFileCommand);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }

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

            string nameOfFileForJson = $"{_kanaloaSettings.RootUrl}/{resizeImageCommand.SaveTo.Replace('\\', '/')}/{imageFileName}";

            string base64Image = data["base64Image"]?.ToString() ?? string.Empty;
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            await System.IO.File.WriteAllBytesAsync(resizeImageCommand.OriginalFileName, imageBytes);

            resizeImage.Execute(resizeImageCommand);

            var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
            {
                ImageFileNameToReadGpsFrom = resizeImageCommand.OriginalFileName
            };
            extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);


            UpdateJsonIfExistsOrCreateNewIfNotCommand updateJsonIfExistsOrCreateNewIfNotCommand = new UpdateJsonIfExistsOrCreateNewIfNotCommand
            {
                LatLngModel = extractGpsInfoFromImageCommand.LatLngModel
            };

            updateJsonIfExistsOrCreateNewIfNotCommand.SetJsonFileName(kmlFileName, folderName);
            updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommand);

            updateJsonIfExistsOrCreateNewIfNotCommand.SetThumbJsonFileName(kmlFileName, folderName);
            updateJsonIfExistsOrCreateNewIfNot.Execute(updateJsonIfExistsOrCreateNewIfNotCommand);

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

    private static string GetValue(JObject data, string value)
    {
        return (data[value] ?? string.Empty).Value<string>() ?? string.Empty;
    }
}