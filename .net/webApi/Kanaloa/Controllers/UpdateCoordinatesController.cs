using Common;
using FileHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UpdateCoordinatesController(
    ISaveGeospatialDataUpdateLivePositionSaveConfigFile saveGeospatialDataUpdateLivePositionSaveConfigFile,
    IOptions<KanaloaSettings> kanaloaSettings)
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
            string geospatialDataFileName = CommonStaticMethods.GetValue(data, "geospatialDataFileName");
            string folderName = CommonStaticMethods.GetValue(data, "folderName");
            string latitude = CommonStaticMethods.GetValue(data, "Latitude");
            string longitude = CommonStaticMethods.GetValue(data, "Longitude");
            string altitude = CommonStaticMethods.GetValue(data, "Altitude") == string.Empty ? "0" : CommonStaticMethods.GetValue(data, "Altitude");
            string coordinates = string.Join(',', longitude, latitude, altitude);

            var gps = new GpsCommand(latitude, longitude, altitude);
            var addFileWithLastKnownGpsPositionCommand = new AddFileWithLastKnownGpsPositionCommand
            {
                GpsCommand = gps
            };

            var updateGeospatialDataIfExistsOrCreateNewIfNotCommand = new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
            {
                GeospatialDataFileNameExtension = "geojson",
                GeospatialDataFileName = geospatialDataFileName,
                Coordinates = coordinates,
                FolderName = folderName
            };

            var writeConfigurationToJsonFileCommand = new WriteConfigurationToJsonFileCommand
            {
                RootUrl = _kanaloaSettings.RootUrl,
                GeospatialDataFileName = geospatialDataFileName,
                WebFolderName = folderName,
                GeospatialDataFileNameExtension = updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileNameExtension
            };

            await saveGeospatialDataUpdateLivePositionSaveConfigFile.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand
                , addFileWithLastKnownGpsPositionCommand
                , writeConfigurationToJsonFileCommand);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }
}