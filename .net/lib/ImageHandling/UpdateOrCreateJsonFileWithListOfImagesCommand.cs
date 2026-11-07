namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImagesCommand
{
    public string? GeospatialDataFileName { get; set; }
    public string? FolderName { get; set; }
    public string? ImageFileName { get; set; }
    public LatLngModel? LatLngModel { get; set; }
    public string? JsonThumbsFileName { get; set; }
    public string? JsonPicsFileName { get; set; }
}