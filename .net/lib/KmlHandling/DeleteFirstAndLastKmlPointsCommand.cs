namespace KmlHandling;

public class DeleteFirstAndLastKmlPointsCommand
{
    public string? KmlFileName { get; set; }
    public string? Folder { get; set; }
    public int DeleteFirstKmlPoints { get; set; }
    public int DeleteLastKmlPoints { get; set; }
}