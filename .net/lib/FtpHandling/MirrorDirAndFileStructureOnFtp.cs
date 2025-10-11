namespace FtpHandling;

public class MirrorDirAndFileStructureOnFtp : IMirrorDirAndFileStructureOnFtp
{
    private readonly IFtpUpload _ftpUpload;

    public MirrorDirAndFileStructureOnFtp(IFtpUpload ftpUpload)
    {
        _ftpUpload = ftpUpload;
    }

    public Task? Execute(string localRootFolderWithRelativePathToCopy, string remoteRootFolder)
    {
        CreateRemoteDir(remoteRootFolder).Wait();
        Task<string>? result = UploadFiles(localRootFolderWithRelativePathToCopy, remoteRootFolder);
        result.Wait();
            
        foreach (string dirPath in Directory.GetDirectories(localRootFolderWithRelativePathToCopy, "*",
                     SearchOption.AllDirectories))
        {
            result = Upload(dirPath);
        }

        return result;

        async Task<string> UploadFiles(string folder, string remoteFolder)
        {
            string[] listOfFilesForUpload = Directory.GetFiles(folder);
            foreach (string file in listOfFilesForUpload)
            {
                Task<string> ftpUploadFileTask = _ftpUpload.FtpFileUploadAsync(file, $"{remoteFolder}/{Path.GetFileName(file)}");
                string exceptionMessage = await ftpUploadFileTask;

                if (!string.IsNullOrWhiteSpace(exceptionMessage))
                {
                    throw new Exception(exceptionMessage);
                }
            }

            return "uploading files completed";
        }

        async Task<string> CreateRemoteDir(string remoteFolder)
        {
            Task<string> ftpCreateDirectoryTask = _ftpUpload.FtpFileCreateDirectoryAsync(remoteFolder);
            string ftpCreateDirectoryTaskExceptionMessage = await ftpCreateDirectoryTask;
            if (!string.IsNullOrWhiteSpace(ftpCreateDirectoryTaskExceptionMessage))
            {
                throw new Exception(ftpCreateDirectoryTaskExceptionMessage);
            }

            return "dir created";
        }

        async Task<string>? Upload(string folder)
        {
            string pathWithoutRoot = folder.Replace(localRootFolderWithRelativePathToCopy + "\\", string.Empty);

            string remoteFolder = $"{remoteRootFolder}/{pathWithoutRoot}";
            remoteFolder = remoteFolder.Replace('\\', '/');

            CreateRemoteDir(remoteFolder).Wait();

            await UploadFiles(folder, remoteFolder);

            return "completed";
        }
    }
}