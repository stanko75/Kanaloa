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
                FolderName = folderName
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
            string imageFileName = data["imageFileName"]?.ToString() ?? "default.jpg";

            string imageOriginalFolderName = Path.Join(folderName, "pics");
            Directory.CreateDirectory(imageOriginalFolderName);
            string imageOriginalFileName = Path.Join(imageOriginalFolderName, imageFileName);

            string thumbsFolder = "thumbs";
            string imageThumbsFolderName = Path.Join(folderName, thumbsFolder);
            Directory.CreateDirectory(imageThumbsFolderName);

            string imageThumbsFileName = $"{imageThumbsFolderName}\\{imageFileName}";

            string base64Image = data["base64Image"]?.ToString() ?? string.Empty;
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            await System.IO.File.WriteAllBytesAsync(imageOriginalFileName, imageBytes);

            string nameOfFileForJson = $"{_kanaloaSettings.RootUrl}/{imageThumbsFolderName.Replace('\\', '/')}/{imageFileName}";

            var resizeImageCommand = new ResizeImageCommand
            {
                CanvasHeight = 200,
                CanvasWidth = 200,
                OriginalFilename = imageOriginalFileName,
                SaveTo = imageThumbsFileName
            };

            resizeImage.Execute(resizeImageCommand);

            return Ok(new
            {
                message =
                    $"Image uploaded to {Path.GetFullPath(imageOriginalFileName)}" +
                    $"{Environment.NewLine}" +
                    $"***" +
                    $"{Environment.NewLine}" +

                    //$"JSON file saved in {Path.GetFullPath(fileNameThumbsJson)}" +
                    //$"{Environment.NewLine}" +
                    //$"***" +

                    $"{Environment.NewLine}" +
                    $"ImageThumbsFileName file saved in {Path.GetFullPath(imageThumbsFileName)}"
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