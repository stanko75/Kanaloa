using System.Data;

namespace ImageHandling;

public class ResizeImageCommand
{
    private string? _imageFileName = string.Empty;
    private string? _imageOriginalFolderName = string.Empty;

    public string? OriginalFileName
    {
        get => Path.Join(_imageOriginalFolderName, _imageFileName);
        set => _imageFileName = value;
    }

    string? _imageThumbsFolderName = string.Empty;
    public string? SaveTo
    {
        get => Path.Join(_imageThumbsFolderName, _imageFileName);
        set => _imageFileName = value;
    }

    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }

    public void CreateDirectories(string folderName)
    {
        _imageOriginalFolderName = Path.Join(folderName, "pics");
        _imageThumbsFolderName = Path.Join(folderName, "thumbs");

        if (string.IsNullOrWhiteSpace(_imageOriginalFolderName)) throw new NoNullAllowedException("_imageOriginalFolderName is null!");
        if (string.IsNullOrWhiteSpace(_imageThumbsFolderName)) throw new NoNullAllowedException("_imageThumbsFolderName is null!");
        Directory.CreateDirectory(folderName);
        Directory.CreateDirectory(_imageOriginalFolderName);
        Directory.CreateDirectory(_imageThumbsFolderName);
    }
}