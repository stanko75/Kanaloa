namespace FastLoadImagesToMemoryAndProcessLater.Log;

public class UpdateUi
{
    public LoggingEventType LoggingEventType { get; set; }
    public Form? Form { get; set; }
    public TextBox? TextBox { get; set; }
    public string? Error { get; set; }
    public string? Name { get; set; }
}