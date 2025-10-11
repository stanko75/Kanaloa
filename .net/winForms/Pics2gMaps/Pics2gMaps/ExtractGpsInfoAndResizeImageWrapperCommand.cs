using System.Data;

namespace Pics2gMaps;

public class ExtractGpsInfoAndResizeImageWrapperCommand
{
    public DataRow DataRow { get; set; }
    public IProgress<int>? RecordCountProgress { get; set; }
}