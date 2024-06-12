using System.Data;
using System.IO;

namespace ImageHandling;

public class ResizeImageCommand
{
    private string? _folderName;
    private string? _imageFileName;
    private string? _imageOriginalFolderName;
    private string? _imageThumbsFolderName;

    public string? OriginalFileName
    {
        get => Path.Join(_imageOriginalFolderName, _imageFileName);
        set
        {
            _imageFileName = value;
            UpdateFolderPaths();
        }
    }

    public string? SaveTo
    {
        get => Path.Join(_imageThumbsFolderName, _imageFileName);
        set
        {
            _imageFileName = value;
            UpdateFolderPaths();
        }
    }

    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }

    public void CreateDirectories(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            throw new NoNullAllowedException("folderName is null or empty!");

        _folderName = folderName;
        UpdateFolderPaths();

        if (string.IsNullOrWhiteSpace(_imageOriginalFolderName))
            throw new NoNullAllowedException("_imageOriginalFolderName is null!");

        if (string.IsNullOrWhiteSpace(_imageThumbsFolderName))
            throw new NoNullAllowedException("_imageThumbsFolderName is null!");

        Directory.CreateDirectory(_folderName);
        Directory.CreateDirectory(_imageOriginalFolderName);
        Directory.CreateDirectory(_imageThumbsFolderName);
    }

    private void UpdateFolderPaths()
    {
        if (!string.IsNullOrWhiteSpace(_folderName))
        {
            _imageOriginalFolderName = Path.Join(_folderName, "pics");
            _imageThumbsFolderName = Path.Join(_folderName, "thumbs");
        }
    }
}