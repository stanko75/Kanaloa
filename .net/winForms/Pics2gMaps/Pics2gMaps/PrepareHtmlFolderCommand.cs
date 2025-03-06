namespace Pics2gMaps;

public class PrepareHtmlFolderCommand
{
    public string? TemplateRootFolder { get; set; }
    public Dictionary<string, string>? ListOfKeyValuesToReplaceInFiles { get; set; }
    public string? SaveTo { get; set; }
    public string? GalleryName { get; set; }
}