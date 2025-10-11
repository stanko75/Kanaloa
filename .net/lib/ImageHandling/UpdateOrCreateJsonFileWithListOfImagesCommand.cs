namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImagesCommand
{
    public string? KmlFileName { get; set; }
    public string? FolderName { get; set; }
    public string? ImageFileName { get; set; }
    public LatLngModel? LatLngModel { get; set; }
    public string? JsonThumbsFileName { get; set; }
    public string? JsonPicsFileName { get; set; }
}