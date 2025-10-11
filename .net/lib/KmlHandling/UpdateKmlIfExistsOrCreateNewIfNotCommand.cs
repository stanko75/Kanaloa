namespace KmlHandling;

public class UpdateKmlIfExistsOrCreateNewIfNotCommand
{
    private string _kmlFileName = string.Empty;
    public string KmlFileName
    {
        get
        {
            _kmlFileName = string.IsNullOrWhiteSpace(_kmlFileName) ? "default" : _kmlFileName;
            _kmlFileName = Path.ChangeExtension(_kmlFileName, "kml");
            return Path.Combine(FolderName, _kmlFileName);
        }
        set => _kmlFileName = value;
    }

    public string? Coordinates { get; set; }

    private string _folderName = string.Empty;
    public string FolderName
    {
        get
        {
            _folderName = string.IsNullOrWhiteSpace(_folderName) ? "default" : _folderName;
            return _folderName;
        }
        set
        {
            _folderName = value;
            if (!string.IsNullOrWhiteSpace(_folderName) && !Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }
        }
    }
}