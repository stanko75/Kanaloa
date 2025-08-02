using System.Data;

namespace Kanaloa.Controllers.uploadToBlog;

public class AutomaticallyFillMissingValuesInDataTableCommand
{
    public DataRow DataRow { get; set; }
    public string BaseUrl { get; set; }
    public string JqueryVersion { get; set; }
}