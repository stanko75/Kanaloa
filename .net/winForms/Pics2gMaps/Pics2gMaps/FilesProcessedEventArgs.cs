namespace Pics2gMaps;

public class FilesProcessedEventArgs : EventArgs
{
    public int ProcessedFiles { get; }
    public FilesProcessedEventArgs(int processedFiles)
    {
        ProcessedFiles = processedFiles;
    }
}