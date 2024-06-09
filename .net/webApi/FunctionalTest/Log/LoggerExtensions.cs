namespace FunctionalTest.Log;

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string message) =>
        logger.Log(new LogEntry(LoggingEventType.Information, message));

    public static void Log(this ILogger logger, Exception? ex)
    {
        if (ex?.Message != null) logger.Log(new LogEntry(LoggingEventType.Error, ex.Message, ex));
    }

    // More methods here.
}