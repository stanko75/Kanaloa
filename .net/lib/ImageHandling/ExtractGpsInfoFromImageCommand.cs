namespace ImageHandling;

public class ExtractGpsInfoFromImageCommand
{
    public string? ImageFileNameToReadGpsFrom { get; set; }
    public LatLngModel? LatLngModel { get; set; }
}