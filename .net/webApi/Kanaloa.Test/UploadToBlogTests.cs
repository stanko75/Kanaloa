using HtmlHandling.Test;
using Kanaloa.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Kanaloa.Test;

[TestClass]
public sealed class UploadToBlogTests
{
    private readonly string _folderName = "album1";
    private string _kmlFileName = "testkmlFileName";
    private readonly string _ftpHost = "testftpHost";
    private readonly string _ftpUser = "testftpUser";
    private readonly string _ftpPass = "testftpPass";
    private readonly string _ogTitle = "testogTitle";
    private readonly string _ogImage = "testogImage";
    private readonly string _baseUrl = "testbaseUrl";

    [TestMethod]
    public void TestIfAllParametersAreCorrect()
    {
        string extension = ".kml";
        _kmlFileName = Common.ChangeFileExtension(_kmlFileName, extension);

        string htmlTemplateFolderWithRelativePath = @"html\templateForBlog";
        string originalHtmlTemplateFolderWithRelativePath = @"..\..\..\..\..\..\html\templateForBlog";
        Common.CreateFolderAndFileForTestIfNotExist(htmlTemplateFolderWithRelativePath, Path.Join(_folderName, _kmlFileName), originalHtmlTemplateFolderWithRelativePath);

        UploadToBlogController uploadToBlogController = new UploadToBlogController();
        IActionResult actionResult = uploadToBlogController.UploadToBlog(new JObject
        {
            ["folderName"] = _folderName,
            ["kmlFileName"] = _kmlFileName,
            ["host"] = _ftpHost,
            ["user"] = _ftpUser,
            ["pass"] = _ftpPass,
            ["ogTitle"] = _ogTitle,
            ["ogImage"] = _ogImage,
            ["baseUrl"] = _baseUrl
        });

        Assert.IsFalse(actionResult is BadRequestObjectResult, $"actionResult is BadRequestResult {(actionResult as BadRequestObjectResult)?.Value }");
    }

    [TestMethod]
    public void TestPatternsReplace()
    {
        string extension = ".kml";
        _kmlFileName = Common.ChangeFileExtension(_kmlFileName, extension);

        UploadToBlogController uploadToBlogController = new UploadToBlogController();
        IActionResult actionResult = uploadToBlogController.UploadToBlog(new JObject
        {
            ["folderName"] = _folderName,
            ["kmlFileName"] = _kmlFileName,
            ["host"] = _ftpHost,
            ["user"] = _ftpUser,
            ["pass"] = _ftpPass,
            ["ogTitle"] = _ogTitle,
            ["ogImage"] = _ogImage,
            ["baseUrl"] = _baseUrl
        });

        string wwwFolder = Path.Join(_folderName, "www");
        string prepareForUploadFolder = Path.Join("prepareForUpload", wwwFolder);
        string htmlFileName = Path.Join(prepareForUploadFolder, "index.html");

        string indexHtml = File.ReadAllText(htmlFileName);
        TestOgs(indexHtml);
    }

    private void TestOgs(string indexHtml)
    {
        /*
        var ogs = new List<string>
        {
            "url",
            "image",
            "title"
        };
        */

        var ogs = new Dictionary<string, string>();
        ogs.Add("url", $"http://{_baseUrl}/prepareForUpload/{_folderName}/www/index.html");
        ogs.Add("image", $"http://{_baseUrl}/prepareForUpload/{_folderName}/{_ogImage}");
        ogs.Add("title", _ogTitle);

        foreach (KeyValuePair<string, string> og in ogs)
        {
            var ogMatch = Regex.Match(
                indexHtml,
                @$"<meta\s+property=[""']og:{og.Key}[""']\s+content=[""']([^""']+)[""']",
                RegexOptions.IgnoreCase
            );
            if (ogMatch.Success)
            {
                string ogValue = ogMatch.Groups[1].Value;
                Assert.AreEqual(ogValue, og.Value, "og:title is wrong");
            }
            else
            {
                Assert.Fail($"og:{og.Key} not found!");
            }
        }
    }
}