using System.Collections.Concurrent;

namespace Pics2gMaps;

public class SearchForAllFilesAndPutThemInQueueCommand
{
    public BlockingCollection<string> FileQueue { get; set; }
    public string Path { get; set; }
}