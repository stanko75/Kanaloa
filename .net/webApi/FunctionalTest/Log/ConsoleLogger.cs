namespace FunctionalTest.Log;

public class ConsoleLogger : ILogger
{
    public void Log(LogEntry entry) => Console.WriteLine(
        $@"[{entry.Severity}] {DateTime.Now} {entry.Message} {entry.Exception}");
}