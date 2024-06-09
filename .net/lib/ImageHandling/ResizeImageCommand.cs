namespace ImageHandling;

public class ResizeImageCommand
{
    string? _folderName = string.Empty;

    public string? FolderName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_folderName)) throw new NullReferenceException("FolderName is empty!");
            return _folderName;
        }
        set => _folderName = value;
    }

    string? _imageFileName = string.Empty;

    string? _imageOriginalFolderName = string.Empty;
    public string? OriginalFileName
    {
        get => Path.Join(_imageOriginalFolderName, _imageFileName);
        set
        {
            _imageFileName = value;
            _imageOriginalFolderName = Path.Join(FolderName, "pics");
            Directory.CreateDirectory(_imageOriginalFolderName);
        }
    }

    string? _imageThumbsFolderName = string.Empty;
    public string? SaveTo
    {
        get => Path.Join(_imageThumbsFolderName, _imageFileName);
        set
        {
            _imageFileName = value;
            _imageThumbsFolderName = Path.Join(FolderName, "thumbs");
            Directory.CreateDirectory(_imageThumbsFolderName);
        } 
    }

    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
}