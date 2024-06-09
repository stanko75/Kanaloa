namespace FunctionalTest.Log;

public class TextBoxLogger(TextBox textBox) : ILogger
{
    public void Log(LogEntry entry)
    {
        textBox.AppendText(
            $@"[{entry.Severity}] {DateTime.Now} {entry.Message} {entry.Exception}");
        textBox.AppendText(Environment.NewLine);
    }
}