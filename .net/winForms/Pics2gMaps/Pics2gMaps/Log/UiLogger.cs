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
            Task.Run(() =>
            {
                var timeElapsed = (DateTime.Now - _lastUpdate).TotalMilliseconds;
                if (timeElapsed > 100)
                {
                    log.UpdateUi.Form.Invoke(() =>
                    {
                        Log(log);
                    });
                    _lastUpdate = DateTime.Now;
                }
            });
            return;
        }

        if (log.UpdateUi.TextBox is null) return;

        log.UpdateUi.TextBox.AppendText(log.UpdateUi.Error);
        log.UpdateUi.TextBox.AppendText(Environment.NewLine);
    }
}