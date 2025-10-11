using Common;
using FtpHandling;
using HtmlHandling;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            string remoteRootFolder = "allWithPics/travelBuddies/";

            var copyHtmlFilesToPrepareForUploadCommand = CopyHtmlFilesToPrepareForUpload(folder, kmlFileName, "prepareForUpload", @"html\templateForBlog");

            var automaticallyFillMissingValuesCommand = AutomaticallyFillMissingValues(data, folder, remoteRootFolder, kmlFileName);

            var listOfKeyValuesToReplaceInFiles = ListOfKeyValuesToReplaceInFiles(automaticallyFillMissingValuesCommand);

            var albumRoot = ReplaceKeysInFiles(copyHtmlFilesToPrepareForUploadCommand, listOfKeyValuesToReplaceInFiles, folder);

            MirrorDirAndFileRemoteOnFtp(host, user, pass, albumRoot, remoteRootFolder, folder);

            return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
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
        string remoteRootFolder, string kmlFileName)
    {
        var automaticallyFillMissingValuesCommand = data.ToObject<AutomaticallyFillMissingValuesCommand>() ??
                                                    throw new InvalidOperationException();
        automaticallyFillMissingValuesCommand.GalleryName = folder;
        automaticallyFillMissingValuesCommand.RootGalleryFolder = remoteRootFolder;
        automaticallyFillMissingValuesCommand.Kml = $@"kml\{Path.GetFileNameWithoutExtension(kmlFileName)}";

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