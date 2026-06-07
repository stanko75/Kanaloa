namespace Common;

public interface IMirrorDirAndFileStructure
{
    public Task? Execute(string localRootFolderWithRelativePathToCopy, string remoteRootFolder);
}