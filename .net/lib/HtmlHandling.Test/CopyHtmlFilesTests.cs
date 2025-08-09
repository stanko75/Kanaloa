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
        fileName = Common.ChangeFileExtension(fileName, extension);

        string htmlTemplateFolderWithRelativePath = @"html\blog";
        Common.CreateFolderAndFileForTestIfNotExist(htmlTemplateFolderWithRelativePath, fileName);

        CopyHtmlFiles copyHtmlFiles = new CopyHtmlFiles();
        copyHtmlFiles.CopyHtmlTemplateForBlog(@"html\blog"
            , "prepareForUpload"
            , folder
            , fileName);
        string pathToKmlShouldBe = Path.Join("prepareForUpload", Path.GetDirectoryName(fileName));
        Assert.IsTrue(Directory.Exists(pathToKmlShouldBe), $"File: {Path.GetFullPath(pathToKmlShouldBe)} not exists.");
    }
}