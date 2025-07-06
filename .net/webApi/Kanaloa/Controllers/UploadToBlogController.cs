using Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadToBlogController : ControllerBase
{
    [HttpPost]
    [Route("UploadToBlog")]
    public IActionResult UploadToBlog([FromBody] JObject data)
    {
        try
        {
            string folder = CommonStaticMethods.GetValue(data, "folderName");
            folder = string.IsNullOrWhiteSpace(folder) ? "default" : folder;

            string kmlFileName = CommonStaticMethods.GetValue(data, "kmlFileName");
            kmlFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : kmlFileName;

            string host = CommonStaticMethods.GetValue(data, "host");
            string user = CommonStaticMethods.GetValue(data, "user");
            string pass = CommonStaticMethods.GetValue(data, "pass");

            string remoteRootFolder = "/allWithPics/travelBuddies";
            return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }
}