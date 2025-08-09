using HtmlHandling.Test;
using Kanaloa.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Diagnostics;

namespace Kanaloa.Test;

[TestClass]
public sealed class UploadToBlogTests
{
    [TestMethod]
    public void TestIfAllParametersAreCorrect()
    {
        string folderName = "album1";
        string kmlFileName = "testkmlFileName";
        string ftpHost = "testftpHost";
        string ftpUser = "testftpUser";
        string ftpPass = "testftpPass";
        string ogTitle = "testogTitle";
        string ogImage = "testogImage";
        string baseUrl = "testbaseUrl";

        string extension = ".kml";
        kmlFileName = Common.ChangeFileExtension(kmlFileName, extension);

        string htmlTemplateFolderWithRelativePath = @"html\templateForBlog";
        string originalHtmlTemplateFolderWithRelativePath = @"..\..\..\..\..\..\html\templateForBlog";
        Common.CreateFolderAndFileForTestIfNotExist(htmlTemplateFolderWithRelativePath, Path.Join(folderName, kmlFileName), originalHtmlTemplateFolderWithRelativePath);

        UploadToBlogController uploadToBlogController = new UploadToBlogController();
        IActionResult actionResult = uploadToBlogController.UploadToBlog(new JObject
        {
            ["folderName"] = folderName,
            ["kmlFileName"] = kmlFileName,
            ["host"] = ftpHost,
            ["user"] = ftpUser,
            ["pass"] = ftpPass,
            ["ogTitle"] = ogTitle,
            ["ogImage"] = ogImage,
            ["baseUrl"] = baseUrl
        });

        Assert.IsFalse(actionResult is BadRequestObjectResult, $"actionResult is BadRequestResult {(actionResult as BadRequestObjectResult)?.Value }");
    }
}