namespace FunctionalTest.Log;

public struct LogEntry
{
    public LoggingEventType Severity { get; }
    public string Message { get; }
    public Exception? Exception { get; }

    public LogEntry(LoggingEventType severity, string msg, Exception? ex = null)
    {
        switch (msg)
        {
            case null:
                throw new ArgumentNullException("msg");
            case "":
                throw new ArgumentException("empty", "msg");
        }

        Severity = severity;
        Message = msg;
        Exception = ex;
    }
}