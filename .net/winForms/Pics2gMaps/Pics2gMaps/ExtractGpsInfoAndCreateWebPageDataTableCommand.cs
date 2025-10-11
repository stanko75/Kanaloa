using System.Data;

namespace Pics2gMaps;

public class ExtractGpsInfoAndCreateWebPageDataTableCommand
{
    public DataRow? DataRow { get; set; }
    public string? BaseUrl { get; set; }
    public string? JqueryVersion { get; set; }
    public string? TemplateRootFolder { get; set; }
    public DataColumnCollection? Columns { get; set; }
    public IProgress<int>? RecordCountProgress { get; set; }
}