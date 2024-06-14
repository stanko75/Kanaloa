namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNotCommand
{
    private string? _jsonFileName = string.Empty;
    public string? JsonFileName { get; set; }

    public LatLngModel? LatLngModel { get; set; }

    public void SetJsonFileName(string kmlFileName, string folderName)
    {
        _jsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        _jsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : _jsonFileName;
        _jsonFileName = Path.ChangeExtension(_jsonFileName, "json");
        _jsonFileName = Path.Join(folderName, _jsonFileName);
    }

    public void SetThumbJsonFileName(string kmlFileName, string folderName)
    {
        _jsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        _jsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : _jsonFileName;
        _jsonFileName = Path.ChangeExtension($"{_jsonFileName}Thumbs", "json");
        _jsonFileName = Path.Join(folderName, _jsonFileName);
    }
}