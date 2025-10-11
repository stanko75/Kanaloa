namespace FtpHandling;

public interface IFtpUpload
{
    public Task<string> FtpFileUploadAsync(string localFileName, string remoteFileName);

    public Task<string> FtpFileDeleteAsync(string remoteFileName);

    public Task<string> FtpFileCreateDirectoryAsync(string remoteFileName);
}