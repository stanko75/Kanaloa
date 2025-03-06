namespace Pics2gMaps;

public class ParallelForEachAndExtractGpsInfoWrapperCommand
{
    public string? FolderName { get; set; }
    public string? ImageFileNameToReadGpsFrom { get; set; }
    public IProgress<int>? RecordCountProgress { get; set; }
}