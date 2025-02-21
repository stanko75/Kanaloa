using Common;
using System.Collections.Concurrent;

namespace Pics2gMaps;

public class SearchForAllFilesAndPutThemInQueue : ICommandHandlerAsync<SearchForAllFilesAndPutThemInQueueCommand>
{
    public Task Execute(SearchForAllFilesAndPutThemInQueueCommand command)
    {
        return Task.Run(() => DoSearchForAllFilesAndPutThemInQueue(command.FileQueue, command.Path));
    }

    private void DoSearchForAllFilesAndPutThemInQueue(BlockingCollection<string> fileQueue, string path)
    {
        Parallel.ForEach(Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories),
            fileQueue.Add);
        fileQueue.CompleteAdding();
    }
}