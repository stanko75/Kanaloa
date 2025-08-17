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
        TestContent.TestIndexHtmlOgs(indexHtml, _baseUrl, _folderName, _ogImage, _ogTitle,
            (match, expected, wrongMsg, notFoundMsg, foundMsg) =>
            {
                AssertTest(match, expected, wrongMsg, notFoundMsg);
            });

        string joomlaPreviewName = Path.Join(prepareForUploadFolder, "joomlaPreview.html");
        string joomlaPreviewHtml = File.ReadAllText(joomlaPreviewName);
        TestContent.TestJoomlaPreviewHtml(joomlaPreviewHtml, _folderName, _ogImage, _ogTitle,
            (match, testValue, notEqualMessage, notFoundMessage, _, index) =>
            {
                AssertTest(match, testValue, notEqualMessage, notFoundMessage, index);
            });

    }

    private void AssertTest(Match match, string testValue, string notEqualMessage, string notFoundMessage, int index = 1)
    {
        if (match.Success)
        {
            string regExValue = match.Groups[index].Value;
            Assert.AreEqual(regExValue, testValue, notEqualMessage);
        }
        else
        {
            Assert.Fail(notFoundMessage);
        }
    }
}