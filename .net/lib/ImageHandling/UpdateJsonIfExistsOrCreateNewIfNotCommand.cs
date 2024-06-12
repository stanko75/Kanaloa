namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNotCommand
{
    private string? _jsonFileName = string.Empty;
    public string? JsonFileName { get; set; }

    public LatLngModel? LatLngModel { get; set; }
}