namespace FileHandling;

public class WriteConfigurationToJsonFileCommand: CommandBase
{
    private string _rootUrl = string.Empty;
    string _folderName = string.Empty;
    string _kmlFileName = string.Empty;

    public string WebFolderName
    {
        get
        {
            _folderName = string.IsNullOrWhiteSpace(_folderName) ? "default" : _folderName;
            _folderName =
                CommonStatic.ConvertRelativeWindowsPathToUri(RootUrl, _folderName).AbsoluteUri;
            if (!_folderName.EndsWith("/"))
            {
                _folderName += "/";
            }

            return _folderName;
        }
        set => _folderName = value;
    }

    public string KmlFileName
    {
        get
        {
            _kmlFileName = string.IsNullOrWhiteSpace(_kmlFileName) ? "default" : _kmlFileName;
            _kmlFileName = Path.ChangeExtension(_kmlFileName, "kml");
            return Path.Combine(WebFolderName, _kmlFileName);
        }
        set => _kmlFileName = value;
    }

    public string ConfigFileName { get; set; } = "config.json";

    public string RootUrl
    {
        get
        {
            if (!_rootUrl.StartsWith("http"))
            {
                _rootUrl = "http://" + _rootUrl;
            }

            if (!_rootUrl.EndsWith("/"))
            {
                _rootUrl += "/";
            }

            return _rootUrl;
        }
        set => _rootUrl = value;
    }
}