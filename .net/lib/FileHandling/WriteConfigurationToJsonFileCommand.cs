namespace FileHandling;

public class WriteConfigurationToJsonFileCommand: CommandBase
{
    public string WebFolderName
    {
        get
        {
            field = string.IsNullOrWhiteSpace(field) ? "default" : field;
            field =
                CommonStatic.ConvertRelativeWindowsPathToUri(RootUrl, field).AbsoluteUri;
            if (!field.EndsWith("/"))
            {
                field += "/";
            }

            return field;
        }
        set;
    } = string.Empty;

    public string? GeospatialDataFileNameExtension { get; set; }

    public string GeospatialDataFileName
    {
        get
        {
            field = string.IsNullOrWhiteSpace(field) ? "default" : field;
            field = Path.ChangeExtension(field, GeospatialDataFileNameExtension);
            return Path.Combine(WebFolderName, field);
        }
        set;
    } = string.Empty;

    public string ConfigFileName { get; set; } = "config.json";

    public string RootUrl
    {
        get
        {
            if (!field.StartsWith("http"))
            {
                field = "http://" + field;
            }

            if (!field.EndsWith("/"))
            {
                field += "/";
            }

            return field;
        }
        set;
    } = string.Empty;
}