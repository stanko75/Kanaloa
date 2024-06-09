namespace FtpHandling;

public interface IMirrorDirAndFileStructureOnFtp
{
    public Task? Execute(string localRootFolderWithRelativePathToCopy, string remoteRootFolder);
}