namespace ImageHandling;

public class UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand
{
    public string? GeospatialDataFileName { get; set;}
    public string? FolderName { get; set; }
    public string? ImageFileName { get; set; }
    public LatLngModel? LatLngModel { get; set; }
}