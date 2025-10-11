using FluentFTP;

namespace FtpHandling;

public class FtpUpload : IFtpUpload
{
    private readonly string _host;
    private readonly string _user;
    private readonly string _pass;

    public FtpUpload(string host, string user, string pass)
    {
        _host = host;
        _user = user;
        _pass = pass;
    }

    public Task<string> FtpFileUploadAsync(string fileName, string remoteFileName)
    {
        return FtpFileUploadAsyncImpl(fileName, remoteFileName);
    }

    public Task<string> FtpFileDeleteAsync(string remoteFileName)
    {
        return FtpFileDeleteAsyncImpl(remoteFileName);
    }

    public Task<string> FtpDirectoryDeleteAsync(string remoteFileName)
    {
        return FtpDirectoryDeleteAsyncImpl(remoteFileName);
    }

    public Task<string> FtpFileCreateDirectoryAsync(string remoteFileName)
    {
        return FtpFileCreateDirectoryAsyncImpl(remoteFileName);
    }

    private async Task<string> FtpFileUploadAsyncImpl(string localFileName, string remoteFileName)
    {
        string exceptionMessage = string.Empty;
        AsyncFtpClient client = new AsyncFtpClient(_host, _user, _pass);
        try
        {
            await client.AutoConnect();
            await client.UploadFile(localFileName, remoteFileName);
        }
        catch (Exception ex)
        {
            exceptionMessage = $"{ex.Message}, Inner exception: {ex.InnerException?.Message}";
        }
        finally
        {
            await client.Disconnect();
        }

        return exceptionMessage;
    }

    private async Task<string> FtpFileDeleteAsyncImpl(string remoteFileName)
    {
        string exceptionMessage = string.Empty;
        AsyncFtpClient client = new AsyncFtpClient(_host, _user, _pass);
        try
        {
            await client.AutoConnect();
            await client.DeleteFile(remoteFileName);
        }
        catch (Exception ex)
        {
            exceptionMessage = $"{ex.Message}, Inner exception: {ex.InnerException?.Message}";
        }
        finally
        {
            await client.Disconnect();
        }

        return exceptionMessage;
    }

    private async Task<string> FtpDirectoryDeleteAsyncImpl(string remoteDirectoryName)
    {
        string exceptionMessage = string.Empty;
        AsyncFtpClient client = new AsyncFtpClient(_host, _user, _pass);
        try
        {
            await client.AutoConnect();
            await client.DeleteDirectory(remoteDirectoryName);
        }
        catch (Exception ex)
        {
            exceptionMessage = $"{ex.Message}, Inner exception: {ex.InnerException?.Message}";
        }
        finally
        {
            await client.Disconnect();
        }

        return exceptionMessage;
    }

    private async Task<string> FtpFileCreateDirectoryAsyncImpl(string remoteDirectoryName)
    {
        string exceptionMessage = string.Empty;
        AsyncFtpClient client = new AsyncFtpClient(_host, _user, _pass);
        try
        {
            await client.AutoConnect();
            await client.CreateDirectory(remoteDirectoryName);
        }
        catch (Exception ex)
        {
            exceptionMessage = $"{ex.Message}, Inner exception: {ex.InnerException?.Message}";
        }
        finally
        {
            await client.Disconnect();
        }

        return exceptionMessage;
    }
}