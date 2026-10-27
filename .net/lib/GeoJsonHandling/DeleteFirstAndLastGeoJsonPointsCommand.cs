namespace GeoJsonHandling;

public class DeleteFirstAndLastGeoJsonPointsCommand
{
    public string? GeoJsonFileName { get; set; }
    public string? Folder { get; set; }
    public int DeleteFirstGeoJsonPoints { get; set; }
    public int DeleteLastGeoJsonPoints { get; set; }
}