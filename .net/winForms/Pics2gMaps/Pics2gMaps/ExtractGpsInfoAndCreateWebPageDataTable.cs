using Common;

namespace Pics2gMaps;

public class ExtractGpsInfoAndCreateWebPageDataTable(AutomaticallyFillMissingValuesInDataTable automaticallyFillMissingValues
, ExtractGpsInfoAndResizeImageWrapper extractGpsInfoAndResizeImageWrapper) : ICommandHandlerAsync<CreateWebPageDataTableCommand>
{

    public async Task Execute(CreateWebPageDataTableCommand command)
    {
        AutomaticallyFillMissingValuesInDataTableCommand automaticallyFillMissingValuesCommand =
            new AutomaticallyFillMissingValuesInDataTableCommand
            {
                DataRow = command.DataRow
                , BaseUrl = command.BaseUrl
                , JqueryVersion = command.JqueryVersion
            };
        automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);

        PrepareHtmlFolderDataTableCommand prepareHtmlFolderCommand = new PrepareHtmlFolderDataTableCommand
        {
            DataRow = command.DataRow
            , TemplateRootFolder = command.TemplateRootFolder
            , Columns = command.Columns
        };
        PrepareHtmlFolderDataTable prepareHtmlFolder = new PrepareHtmlFolderDataTable();
        prepareHtmlFolder.Execute(prepareHtmlFolderCommand);

        var extractGpsInfoAndResizeImageWrapperCommand =
            new ExtractGpsInfoAndResizeImageWrapperCommand
            {
                DataRow = command.DataRow
                , RecordCountProgress = command.RecordCountProgress
            };

        await extractGpsInfoAndResizeImageWrapper.Execute(extractGpsInfoAndResizeImageWrapperCommand);
    }
}