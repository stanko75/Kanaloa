using System.Data;

namespace Pics2gMaps;

public class PrepareHtmlFolderDataTableCommand
{
    public string? TemplateRootFolder { get; set; }
    public DataRow? DataRow { get; set; }
    public DataColumnCollection? Columns { get; set; }
    public Dictionary<string, string>? ListOfKeyValuesToReplaceInFiles { get; set; }
    public string? SaveTo { get; set; }
    public string? GalleryName { get; set; }
}