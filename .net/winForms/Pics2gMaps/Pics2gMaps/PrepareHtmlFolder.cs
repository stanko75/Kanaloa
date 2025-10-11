using Common;
using HtmlHandling;
using Newtonsoft.Json;

namespace Pics2gMaps;

public class PrepareHtmlFolder : ICommandHandler<PrepareHtmlFolderCommand>
{
    public void Execute(PrepareHtmlFolderCommand command)
    {
        DoPrepareHtmlFolder(command.TemplateRootFolder
            , command.ListOfKeyValuesToReplaceInFiles
            , command.SaveTo
            , command.GalleryName);
    }

    private void DoPrepareHtmlFolder(string? templateRootFolder
        , Dictionary<string, string>? listOfKeyValuesToReplaceInFiles
        , string? saveTo
        , string? galleryName)
    {
        ReplaceKeysInFilesCommand replaceKeysInFilesCommand = new ReplaceKeysInFilesCommand();

        string listOfFilesToReplaceAndCopyFileName = Path.Join(templateRootFolder, "listOfFilesToReplaceAndCopy.json");
        IEnumerable<string> listOfFilesToReplaceAndCopy = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(listOfFilesToReplaceAndCopyFileName));

        replaceKeysInFilesCommand.ListOfFilesToReplace = listOfFilesToReplaceAndCopy;
        replaceKeysInFilesCommand.ListOfKeyValuesToReplaceInFiles = listOfKeyValuesToReplaceInFiles;
        replaceKeysInFilesCommand.TemplateRootFolder = templateRootFolder;
        replaceKeysInFilesCommand.SaveToPath = Path.Join(saveTo, galleryName);
        replaceKeysInFilesCommand.SaveToPath = Path.Join(replaceKeysInFilesCommand.SaveToPath, "www");

        if (!Directory.Exists(replaceKeysInFilesCommand.SaveToPath))
        {
            Directory.CreateDirectory(replaceKeysInFilesCommand.SaveToPath);
        }

        ReplaceKeysInFiles replaceKeysInFiles = new ReplaceKeysInFiles();
        replaceKeysInFiles.Execute(replaceKeysInFilesCommand);
    }

}