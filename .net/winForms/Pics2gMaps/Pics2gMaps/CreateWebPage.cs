using Common;

namespace Pics2gMaps;

public class CreateWebPage : ICommandHandlerAsync<CreateWebPageCommand>
{
    public ToolStripStatusLabel? TsslRecordCount { get; set; }
    public Form? Form { get; set; }

    public CreateWebPage()
    {
        var automaticallyFillMissingValues = new AutomaticallyFillMissingValues();
        var prepareHtmlFolder = new PrepareHtmlFolder();

        IProgress<int> recordCountProgress = new Progress<int>(UpdateRecordCount);
        var parallelForEachAndExtractGpsInfoWrapper = new ParallelForEachAndExtractGpsInfoWrapper(recordCountProgress);
        var extractGpsInfoAndResizeImageWrapper = new ExtractGpsInfoAndResizeImageWrapper(parallelForEachAndExtractGpsInfoWrapper);
    }

    private void UpdateRecordCount(int obj)
    {
/*
        if (Form is { IsDisposed: true }) return;

        if (FormInvokeRequired)
        {
            BeginInvoke(() => UpdateStatus(recordCount));
        }
        else
        {
            UpdateStatus(recordCount);
        }
*/
    }

    public Task Execute(CreateWebPageCommand command)
    {
        throw new NotImplementedException();
    }
}