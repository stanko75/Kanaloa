using Common;
using FileHandling;
using FtpHandling;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using HtmlHandling;

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

            CopyHtmlFilesCommand copyHtmlFilesCommand = new CopyHtmlFilesCommand();
            copyHtmlFilesCommand.HtmlTemplateFolderWithRelativePath = @"html\templateForBlog";
            copyHtmlFilesCommand.KmlFileName = Path.Join(folder, kmlFileName);
            copyHtmlFilesCommand.NameOfAlbum = folder;
            copyHtmlFilesCommand.PrepareForUploadFolder = "prepareForUpload";

            CopyHtmlFiles copyHtmlFiles = new CopyHtmlFiles();
            copyHtmlFiles.Execute(copyHtmlFilesCommand);

            //DoAutomaticallyFillMissingValues
            //galleryName = folder
            //rootGalleryFolder not allowed to be empty, but it will be
            //List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();
            //    var jsonObj = new Dictionary<string, string>
            //    {
            //        { "/*galleryName*/", data[DataTableConfigColumns.GalleryName].ToString() },
            //        { "/*rootGalleryFolder*/", data[DataTableConfigColumns.RootGalleryFolder].ToString() },
            //        { "/*webPath*/", data[DataTableConfigColumns.WebPath].ToString() },
            //        { "/*gapikey*/", data[DataTableConfigColumns.Gapikey].ToString() },
            //        { "/*ogTitle*/", data[DataTableConfigColumns.OgTitle].ToString() },
            //        { "/*ogDescription*/", data[DataTableConfigColumns.OgDescription].ToString() },
            //        { "/*ogImage*/", data[DataTableConfigColumns.OgImage].ToString() },
            //        { "/*ogUrl*/", data[DataTableConfigColumns.OgUrl].ToString() },
            //        { "/*picsJson*/", data[DataTableConfigColumns.PicsJson].ToString() },
            //        { "/*zoom*/", data[DataTableConfigColumns.Zoom].ToString() },
            //        { "/*resizeImages*/", string.IsNullOrWhiteSpace(data[DataTableConfigColumns.ResizeImages].ToString()) ? "false" : data[DataTableConfigColumns.ResizeImages].ToString() },
            //        { "/*joomlaThumbsPath*/", data[DataTableConfigColumns.JoomlaThumbsPath].ToString() },
            //        { "/*joomlaImgSrcPath*/", data[DataTableConfigColumns.JoomlaImgSrcPath].ToString() },
            //        { "/*isMerged*/", string.IsNullOrWhiteSpace(data[DataTableConfigColumns.IsMerged].ToString()) ? "false" : data[DataTableConfigColumns.IsMerged].ToString() },
            //        { "/*jqueryVersion*/", data[DataTableConfigColumns.JqueryVersion].ToString() },
            //        { "/*ogImageFullPath*/", data[DataTableConfigColumns.OgImageFullPath].ToString() }
            //    };

            //    jsonList.Add(jsonObj);


            return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }

    public static T? GetValue<T>(object source, string columnName)
    {
        return source switch
        {
            DataRow row => (T?)row[columnName],
            JObject jObject => jObject[columnName] is not null
                ? jObject[columnName]!.ToObject<T>()
                : default,
            _ => throw new ArgumentException("Unsupported object type.")
        };
    }
}