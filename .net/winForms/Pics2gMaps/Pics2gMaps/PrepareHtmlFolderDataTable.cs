using Common;
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

        PrepareHtmlFolderCommand prepareHtmlFolderCommand = new PrepareHtmlFolderCommand();
        prepareHtmlFolderCommand.TemplateRootFolder = command.TemplateRootFolder;
        prepareHtmlFolderCommand.ListOfKeyValuesToReplaceInFiles = listOfKeyValuesToReplaceInFiles;
        prepareHtmlFolderCommand.SaveTo = command.DataRow[DataTableConfigColumns.RootGalleryFolder].ToString();
        prepareHtmlFolderCommand.GalleryName = command.DataRow[DataTableConfigColumns.GalleryName].ToString();

        PrepareHtmlFolder prepareHtmlFolder = new PrepareHtmlFolder();
        prepareHtmlFolder.Execute(prepareHtmlFolderCommand);
    }
}