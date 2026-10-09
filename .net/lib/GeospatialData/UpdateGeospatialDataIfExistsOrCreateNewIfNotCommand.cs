namespace Common;

public class UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
{
    public string GeospatialDataFileNameExtension { get; set; }

    public string GeospatialDataFileName
    {
        get
        {
            field = string.IsNullOrWhiteSpace(field) ? "default" : field;
            field = Path.ChangeExtension(field, GeospatialDataFileNameExtension);
            return Path.Combine(FolderName, field);
        }
        set;
    } = string.Empty;

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