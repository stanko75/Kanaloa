using Common;
using HtmlHandling;
using Newtonsoft.Json;
using System.Data;


namespace Pics2gMaps;

public class PrepareHtmlFolderDataTable : ICommandHandler<PrepareHtmlFolderDataTableCommand>
{
    public void Execute(PrepareHtmlFolderDataTableCommand command)
    {
        Dictionary<string, string> listOfKeyValuesToReplaceInFiles = new Dictionary<string, string>();
        foreach (DataColumn dataColumn in command.Columns)
        {
            listOfKeyValuesToReplaceInFiles.Add($"/*{dataColumn}*/", command.DataRow[dataColumn].ToString());
        }

        DoPrepareHtmlFolder(command.TemplateRootFolder
            , listOfKeyValuesToReplaceInFiles
            , command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString()
            , command.DataRow[DataTableConfigColumns.GalleryName].ToString());

    }

    private void DoPrepareHtmlFolder(string templateRootFolder
        , Dictionary<string, string> listOfKeyValuesToReplaceInFiles
        , string? saveTo
        , string galleryName)
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