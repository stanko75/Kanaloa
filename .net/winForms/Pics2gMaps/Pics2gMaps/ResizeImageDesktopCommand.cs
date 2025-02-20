using System.Collections.Concurrent;
using System.Data;

namespace Pics2gMaps;

public class ResizeImageDesktopCommand
{
    public DataRow DataRow { get; set; }
    public BlockingCollection<string>? FileQueue { get; set; }
}