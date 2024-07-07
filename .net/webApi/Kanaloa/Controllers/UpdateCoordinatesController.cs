﻿using FileHandling;
using KmlHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using static Kanaloa.Common;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UpdateCoordinatesController(
    ISaveKmlUpdateLivePositionSaveConfigFile saveKmlUpdateLivePositionSaveConfigFile,
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
            string kmlFileName = GetValue(data, "kmlFileName");
            string folderName = GetValue(data, "folderName");
            string latitude = GetValue(data, "Latitude");
            string longitude = GetValue(data, "Longitude");
            string coordinates = string.Join(',', longitude, latitude, "2357");

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
}