using Common;
using FtpHandling;
using KmlHandling;
using HtmlHandling;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostToJoomla;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadToBlogController(ICommandHandler<DeleteFirstAndLastKmlPointsCommand> deleteFirstAndLastKmlPoints) : ControllerBase
{
    [HttpPost]
    [Route("UploadToBlog")]
    public async Task<IActionResult> UploadToBlog([FromBody] JObject data)
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
            string strDeleteLastKmlPoints = CommonStaticMethods.GetValue(data, "deleteLastKmlPoints");
            string strDeleteFirstKmlPoints = CommonStaticMethods.GetValue(data, "deleteFirstKmlPoints");
            string prepareForUpload = "prepareForUpload";

            string joomlaCategoryId = CommonStaticMethods.GetValue(data, "joomlaCategoryId");
            string joomlaLoginUrl = CommonStaticMethods.GetValue(data, "joomlaLoginUrl");
            string joomlaPostUrl = CommonStaticMethods.GetValue(data, "joomlaPostUrl");
            string joomlaUserName = CommonStaticMethods.GetValue(data, "joomlaUserName");
            string joomlaPass = CommonStaticMethods.GetValue(data, "joomlaPass");
            
            string remoteRootFolder = "allWithPics/travelBuddies/";

            var copyHtmlFilesToPrepareForUploadCommand = CopyHtmlFilesToPrepareForUpload(folder, kmlFileName, prepareForUpload, @"html\templateForBlog");

            var automaticallyFillMissingValuesCommand = AutomaticallyFillMissingValues(data, folder, remoteRootFolder, kmlFileName, "jquery-3.6.4.js");

            var listOfKeyValuesToReplaceInFiles = ListOfKeyValuesToReplaceInFiles(automaticallyFillMissingValuesCommand);

            var albumRoot = ReplaceKeysInFiles(copyHtmlFilesToPrepareForUploadCommand, listOfKeyValuesToReplaceInFiles, folder);

            DeleteFirstAndLastKmlPoints(prepareForUpload, folder, kmlFileName, strDeleteFirstKmlPoints, strDeleteLastKmlPoints);

            MirrorDirAndFileRemoteOnFtp(host, user, pass, albumRoot, remoteRootFolder, folder);

            if (!string.IsNullOrWhiteSpace(joomlaCategoryId)
                && !string.IsNullOrWhiteSpace(joomlaLoginUrl)
                && !string.IsNullOrWhiteSpace(joomlaPostUrl)
                && !string.IsNullOrWhiteSpace(joomlaUserName)
                && !string.IsNullOrWhiteSpace(joomlaPass))
            {
                var ok = await PostArticleToJoomla(copyHtmlFilesToPrepareForUploadCommand
                    , joomlaCategoryId
                    , joomlaLoginUrl
                    , joomlaPostUrl
                    , joomlaUserName
                    , joomlaPass
                    , automaticallyFillMissingValuesCommand.OgTitle);
                if (ok)
                {
                    return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");
                }

                return BadRequest("Article was not saved successfully.");
            }

            return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");

        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }

    private async Task<bool> PostArticleToJoomla(CopyHtmlFilesCommand copyHtmlFilesToPrepareForUploadCommand
        , string joomlaCategoryId
        , string joomlaLoginUrl
        , string joomlaPostUrl
        , string joomlaUserName
        , string joomlaPass
        , string articleTitle)
    {
        OpenAdminPageAndPostArticleCommand openAdminPageAndPostArticleCommand = new OpenAdminPageAndPostArticleCommand
            {
                CategoryId = joomlaCategoryId,
                LoginUrl = joomlaLoginUrl,
                PostUrl = joomlaPostUrl,
                UserName = joomlaUserName,
                Pass = joomlaPass,
                ArticleText = await System.IO.File.ReadAllTextAsync(copyHtmlFilesToPrepareForUploadCommand.JoomlaPreviewHtml),
                Title = articleTitle
            };

        OpenAdminPageAndPostArticle openAdminPageAndPostArticle = new OpenAdminPageAndPostArticle();
        await openAdminPageAndPostArticle.Execute(openAdminPageAndPostArticleCommand);

        return openAdminPageAndPostArticleCommand.IsSaved;
    }

    private void DeleteFirstAndLastKmlPoints(string prepareForUpload, string folder, string kmlFileName,
        string strDeleteFirstKmlPoints, string strDeleteLastKmlPoints)
    {
        DeleteFirstAndLastKmlPointsCommand deleteFirstAndLastKmlPointsCommand =
            new DeleteFirstAndLastKmlPointsCommand();
        deleteFirstAndLastKmlPointsCommand.Folder = Path.Join(Path.Join(prepareForUpload, folder), "kml");
        deleteFirstAndLastKmlPointsCommand.KmlFileName = kmlFileName;
        if (int.TryParse(strDeleteFirstKmlPoints, out int intDeleteFirstKmlPoints))
        {
            deleteFirstAndLastKmlPointsCommand.DeleteFirstKmlPoints = intDeleteFirstKmlPoints;
        }
        else
        {
            throw new InvalidCastException("Cannot cast deleteFirstKmlPoints to int");
        }

        if (int.TryParse(strDeleteLastKmlPoints, out int intDeleteLastKmlPoints))
        {
            deleteFirstAndLastKmlPointsCommand.DeleteLastKmlPoints = intDeleteLastKmlPoints;
        }
        else
        {
            throw new InvalidCastException("Cannot cast deleteLastKmlPoints to int");
        }

        deleteFirstAndLastKmlPoints.Execute(deleteFirstAndLastKmlPointsCommand);
    }

    private static void MirrorDirAndFileRemoteOnFtp(string host, string user, string pass, string albumRoot,
        string remoteRootFolder, string folder)
    {
        //www.milosev.com/milosev.com/gallery/allWithPics/travelBuddies/kelnTest/index.html
        IFtpUpload ftpUpload = new FtpUpload(host, user, pass);
        IMirrorDirAndFileStructureOnFtp mirrorDirAndFileStructureOnFtp = new MirrorDirAndFileStructureOnFtp(ftpUpload);
        mirrorDirAndFileStructureOnFtp.Execute(albumRoot,
            $"{remoteRootFolder}/{folder}");
    }

    private static string ReplaceKeysInFiles(CopyHtmlFilesCommand copyHtmlFilesToPrepareForUploadCommand,
        Dictionary<string, string> listOfKeyValuesToReplaceInFiles, string folder)
    {
        ReplaceKeysInFilesCommand replaceKeysInFilesCommand = new ReplaceKeysInFilesCommand();

        string listOfFilesToReplaceAndCopyFileName =
            Path.Join(copyHtmlFilesToPrepareForUploadCommand.HtmlTemplateFolderWithRelativePath, "listOfFilesToReplaceAndCopy.json");
        replaceKeysInFilesCommand.ListOfFilesToReplace =
            JsonConvert.DeserializeObject<IEnumerable<string>>(
                System.IO.File.ReadAllText(listOfFilesToReplaceAndCopyFileName)) ??
            throw new InvalidOperationException();

        replaceKeysInFilesCommand.ListOfKeyValuesToReplaceInFiles = listOfKeyValuesToReplaceInFiles;

        string albumRoot = Path.Join(copyHtmlFilesToPrepareForUploadCommand.PrepareForUploadFolder, folder);
        replaceKeysInFilesCommand.TemplateRootFolder = albumRoot;
        replaceKeysInFilesCommand.TemplateRootFolder = Path.Join(replaceKeysInFilesCommand.TemplateRootFolder, "www");
        replaceKeysInFilesCommand.SaveToPath = replaceKeysInFilesCommand.TemplateRootFolder;

        ReplaceKeysInFiles replaceKeysInFiles = new ReplaceKeysInFiles();
        replaceKeysInFiles.Execute(replaceKeysInFilesCommand);
        return albumRoot;
    }

    private static Dictionary<string, string> ListOfKeyValuesToReplaceInFiles(
        AutomaticallyFillMissingValuesCommand automaticallyFillMissingValuesCommand)
    {
        var listOfKeyValuesToReplaceInFiles = typeof(AutomaticallyFillMissingValuesCommand)
            .GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(
                p => $"/*{FirstCharToLower(p.Name)}*/",
                p => p.GetValue(automaticallyFillMissingValuesCommand) == null
                    ? ""
                    : p.GetValue(automaticallyFillMissingValuesCommand)!.ToString()!
            );
        return listOfKeyValuesToReplaceInFiles;
    }

    private static AutomaticallyFillMissingValuesCommand AutomaticallyFillMissingValues(JObject data, string folder,
        string remoteRootFolder, string kmlFileName, string jqueryVersion)
    {
        var automaticallyFillMissingValuesCommand = data.ToObject<AutomaticallyFillMissingValuesCommand>() ??
                                                    throw new InvalidOperationException();
        automaticallyFillMissingValuesCommand.GalleryName = folder;
        automaticallyFillMissingValuesCommand.RootGalleryFolder = remoteRootFolder;
        automaticallyFillMissingValuesCommand.Kml = $@"kml\{Path.GetFileNameWithoutExtension(kmlFileName)}";
        automaticallyFillMissingValuesCommand.JqueryVersion = jqueryVersion;

        AutomaticallyFillMissingValues automaticallyFillMissingValues = new AutomaticallyFillMissingValues(new PathConverter());
        automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);
        return automaticallyFillMissingValuesCommand;
    }

    private static CopyHtmlFilesCommand CopyHtmlFilesToPrepareForUpload(string folder, string kmlFileName, string prepareForUploadFolder, string htmlTemplateFolderWithRelativePath)
    {
        CopyHtmlFilesCommand copyHtmlFilesToPrepareForUploadCommand = new CopyHtmlFilesCommand
        {
            HtmlTemplateFolderWithRelativePath = htmlTemplateFolderWithRelativePath,
            KmlFileName = Path.Join(folder, kmlFileName),
            NameOfAlbum = folder,
            PrepareForUploadFolder = prepareForUploadFolder
        };

        CopyHtmlFiles copyHtmlToPrepareForUploadFiles = new CopyHtmlFiles();
        copyHtmlToPrepareForUploadFiles.Execute(copyHtmlFilesToPrepareForUploadCommand);
        return copyHtmlFilesToPrepareForUploadCommand;
    }

    private static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
    
}

internal class PathConverter : IPathConverter
{
    public string Execute(string webPath)
    {
        return webPath;
    }
}