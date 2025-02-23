using FastLoadImagesToMemoryAndProcessLater.Log;

namespace Pics2gMaps.Log;

public interface ILogger
{
    void Log(LogEntry log);
}