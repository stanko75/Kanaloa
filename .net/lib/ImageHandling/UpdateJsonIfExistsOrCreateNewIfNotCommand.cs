namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNotCommand
{
    public string? JsonFileName { get; set; }
    public string? ImageFileName { get; set; }

    public LatLngModel? LatLngModel { get; set; }

    public void SetJsonAndImageFileName(string kmlFileName, string folderName, string rootUrl, string imageFileName)
    {
        JsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        JsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : JsonFileName;
        JsonFileName = Path.ChangeExtension(JsonFileName, "json");
        JsonFileName = Path.Join(folderName, JsonFileName);

        ImageFileName = $"../../{folderName.Replace('\\', '/')}/{imageFileName}";
    }

    public void SetThumbJsonAndImageFileName(string kmlFileName, string folderName, string rootUrl, string imageFileName)
    {
        JsonFileName = Path.GetFileNameWithoutExtension(kmlFileName);
        JsonFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : JsonFileName;
        JsonFileName = Path.ChangeExtension($"{JsonFileName}Thumbs", "json");
        JsonFileName = Path.Join(folderName, JsonFileName);

        string imageThumbsFolderName = Path.Join(folderName, "thumbs");
        ImageFileName = $"../../{imageThumbsFolderName.Replace('\\', '/')}/{imageFileName}";
    }
}