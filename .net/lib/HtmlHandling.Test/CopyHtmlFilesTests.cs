namespace HtmlHandling.Test;

[TestClass]
public class CopyHtmlFilesTests
{
    [TestMethod]
    public void SimulateRealLifeSituation()
    {
        string folder = "album1";
        string fileName = @"album1\test";

        string extension = ".kml";
        fileName = ChangeFileExtension(fileName, extension);

        string htmlTemplateFolderWithRelativePath = @"html\blog";
        CreateFolderAndFileForTestIfNotExist(htmlTemplateFolderWithRelativePath, fileName);

        CopyHtmlFiles copyHtmlFiles = new CopyHtmlFiles();
        copyHtmlFiles.CopyHtmlTemplateForBlog(@"html\blog"
            , "prepareForUpload"
            , folder
            , fileName);
        string pathToKmlShouldBe = Path.Join("prepareForUpload", Path.GetDirectoryName(fileName));
        Assert.IsTrue(Directory.Exists(pathToKmlShouldBe), $"File: {Path.GetFullPath(pathToKmlShouldBe)} not exists.");
    }

    private static void CreateFolderAndFileForTestIfNotExist(string htmlTemplateFolderWithRelativePath, string fileName)
    {
        if (!Directory.Exists(htmlTemplateFolderWithRelativePath))
        {
            string originalHtmlTemplateFolderWithRelativePath = @"..\..\..\..\..\..\html\templateForBlog";

            Assert.IsTrue(Directory.Exists(originalHtmlTemplateFolderWithRelativePath), $"Folder: {Path.GetFullPath(originalHtmlTemplateFolderWithRelativePath)} not exists.");
            DirectoryCopy(originalHtmlTemplateFolderWithRelativePath, htmlTemplateFolderWithRelativePath, true);
        }

        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException());
            File.WriteAllText(fileName, "test");
        }
    }

    private static string ChangeFileExtension(string fileName, string extension)
    {
        if (!Path.GetExtension(fileName).Equals(extension, StringComparison.OrdinalIgnoreCase))
        {
            fileName = Path.ChangeExtension(fileName, extension);
        }

        return fileName;
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}