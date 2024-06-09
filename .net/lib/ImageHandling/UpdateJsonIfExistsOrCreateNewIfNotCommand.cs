namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNotCommand
{
    public string? JsonFileName { get; set; }
    public LatLngModel? LatLngModel { get; set; }
}