using FastLoadImagesToMemoryAndProcessLater.Log;

namespace Pics2gMaps.Log;

public class UiLogger : ILogger
{
    private static DateTime _lastUpdate = DateTime.MinValue;
    public void Log(LogEntry log)
    {
        if (log.UpdateUi == null) return;
        if (log.UpdateUi.Form == null) return;

        if (log.UpdateUi.Form.InvokeRequired)
        {
            log.UpdateUi.Form.Invoke(() => Log(log));
            return;
        }

        log.UpdateUi.TextBox.AppendText(log.UpdateUi.Error);
        log.UpdateUi.TextBox.AppendText(Environment.NewLine);
    }
}