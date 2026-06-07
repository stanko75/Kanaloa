using Common;

namespace UploadWithPhpScriptHandling;

public class MirrorDirAndFileStructureWithPhpScript(PhpUpload phpUpload, PhpUploadCommand phpUploadCommand) : IMirrorDirAndFileStructure
{
    public async Task? Execute(string localRootFolderWithRelativePathToCopy, string remoteRootFolder)
    {
        foreach (string dirPath in Directory.GetDirectories(localRootFolderWithRelativePathToCopy, "*",
                     SearchOption.AllDirectories))
        {
            string[] listOfFilesForUpload = Directory.GetFiles(dirPath);
            foreach (string file in listOfFilesForUpload)
            {
                phpUploadCommand.FullFileName = file;
                await phpUpload.Execute(phpUploadCommand);
            }
        }
    }
}