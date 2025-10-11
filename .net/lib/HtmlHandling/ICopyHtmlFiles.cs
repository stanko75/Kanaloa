namespace HtmlHandling;

public interface ICopyHtmlFiles
{
    public void CopyHtmlTemplateForBlog(
        string htmlTemplateFolderWithRelativePath
        , string rootFolderWithRelativePathToCopy
        , string nameOfAlbum
        , string kmlFileName);
}