namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNotCommand
{
    public string? JsonFileName { get; set; }
    public string? ImageFileName { get; set; }

    public LatLngModel? LatLngModel { get; set; }

    public void SetJsonFileName(string kmlFileName, string folderName)
    {
        JsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        JsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : JsonFileName;
        JsonFileName = Path.ChangeExtension(JsonFileName, "json");
        JsonFileName = Path.Join(folderName, JsonFileName);
    }

    public void SetThumbJsonFileName(string kmlFileName, string folderName)
    {
        JsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        JsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : JsonFileName;
        JsonFileName = Path.ChangeExtension($"{JsonFileName}Thumbs", "json");
        JsonFileName = Path.Join(folderName, JsonFileName);
    }
}