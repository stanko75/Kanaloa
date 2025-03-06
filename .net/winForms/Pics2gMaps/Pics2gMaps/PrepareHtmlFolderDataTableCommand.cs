using System.Data;

namespace Pics2gMaps;

public class PrepareHtmlFolderDataTableCommand
{
    public string TemplateRootFolder { get; set; }
    public DataRow DataRow { get; set; }
    public DataColumnCollection Columns { get; set; }
}