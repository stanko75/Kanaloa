using System.Reflection.Metadata.Ecma335;

namespace FastLoadImagesToMemoryAndProcessLater.Log;

public interface ILogger
{
    void Log(LogEntry log);
}