namespace ImageHandling;

public class ResizeImageCommand
{
    public string? OriginalFileName { get; set; }
    public string? SaveTo { get; set; }
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
}