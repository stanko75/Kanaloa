using Common;

namespace HtmlHandling;

public class CopyHtmlFiles: ICommandHandler<CopyHtmlFilesCommand>
{

    public void Execute(CopyHtmlFilesCommand command)
    {
        string joomlaPreviewHtml = string.Empty;
        CopyHtmlTemplateForBlog(
            command.HtmlTemplateFolderWithRelativePath //"html\blog\www"
            , command.PrepareForUploadFolder //"prepareForUpload"
            , command.NameOfAlbum
            , command.KmlFileName
            , out joomlaPreviewHtml);
        command.JoomlaPreviewHtml = joomlaPreviewHtml;
    }

    /*
      copy existing structure:

     \nameOfAlbum\nameOfAlbumThumbs.json
     \nameOfAlbum\pics
     \nameOfAlbum\thumbs
     \nameOfAlbum\nameOfAlbum.kml

    to 

       string[] fileAndFolderStructure =
       [
           @"kml\nameOfAlbum.kml",
           "pics",
           "thumbs",
           "www",
           @"www\css\index.css",
           @"www\lib\jquery-3.3.1.js",
           @"www\script\map.js",
           @"www\script\namespaces.js",
           @"www\script\pics2maps.js",
           @"www\script\thumbnails.js",
           @"www\index.html",
           @"www\joomlaPreview.html",
           @"www\/*albumName* /.js",
           @"www\/*albumName* /Thumbs.json"
       ];

     */

    public void CopyHtmlTemplateForBlog(
        string htmlTemplateFolderWithRelativePath //"html\blog\www"
        , string prepareForUploadFolder //"prepareForUpload"
        , string nameOfAlbum
        , string geospatialDataFileName
        , out string joomlaPreviewHtml)
    {
        joomlaPreviewHtml = string.Empty;
        if (!Directory.Exists(htmlTemplateFolderWithRelativePath))
        {
            throw new DirectoryNotFoundException($"The folder {Path.GetFullPath(htmlTemplateFolderWithRelativePath)} does not exist!");
        }

        if (!File.Exists(geospatialDataFileName))
        {
            throw new FileNotFoundException(
                $"The file {geospatialDataFileName} does not exist. Absolute path: {Path.GetFullPath(geospatialDataFileName)}");
        }

        if (Directory.Exists(prepareForUploadFolder))
        {
            Directory.Delete(prepareForUploadFolder, true);
        }

        prepareForUploadFolder = Path.Join(prepareForUploadFolder, nameOfAlbum);
        string wwwFolder = Path.Join(prepareForUploadFolder, "www");
        Directory.CreateDirectory(prepareForUploadFolder);
        Directory.CreateDirectory(Path.Join(wwwFolder, "script"));
        Directory.CreateDirectory(Path.Join(wwwFolder, "css"));
        Directory.CreateDirectory(Path.Join(wwwFolder, "lib"));
        Directory.CreateDirectory(Path.Join(prepareForUploadFolder, "geojson"));
        Directory.CreateDirectory(Path.Join(prepareForUploadFolder, "pics"));
        Directory.CreateDirectory(Path.Join(prepareForUploadFolder, "thumbs"));

        //copy from "html\blog\www" to prepareForUpload
        string[] allFiles = Directory.GetFiles(htmlTemplateFolderWithRelativePath, "*.*", SearchOption.AllDirectories);
        foreach (string file in allFiles)
        {
            string fileNameOnly = Path.GetFileName(file);
            string savewwwFiles = Path.Join(wwwFolder, fileNameOnly);
            if (file.ToLower().Contains(@"script"))
            {
                savewwwFiles = Path.Join(wwwFolder, Path.Join("script", fileNameOnly));
            } 
            else if (file.ToLower().Contains(@"css"))
            {
                savewwwFiles = Path.Join(wwwFolder, Path.Join("css", fileNameOnly));
            }
            else if (file.ToLower().Contains(@"lib"))
            {
                savewwwFiles = Path.Join(wwwFolder, Path.Join("lib", fileNameOnly));
            }

            File.Copy(file, savewwwFiles);

            if (file.ToLower().Contains("joomlaPreview.html".ToLower()))
            {
                joomlaPreviewHtml = savewwwFiles;
            }
        }

        string sourceFileName = Path.Join(nameOfAlbum, $"{nameOfAlbum}Thumbs.json");
        if (File.Exists(sourceFileName))
        {
            File.Copy(sourceFileName, Path.Join(wwwFolder, $"{nameOfAlbum}Thumbs.json"));
        }

        sourceFileName = Path.Join(nameOfAlbum, $"{nameOfAlbum}.json");
        if (File.Exists(sourceFileName))
        {
            File.Copy(sourceFileName, Path.Join(wwwFolder, $"{nameOfAlbum}.json"));
        }

        string geospatialDataFileNameSaveTo = Path.GetFileName(geospatialDataFileName);
        string geoJson = string.Join("/", prepareForUploadFolder, "geojson");
        geospatialDataFileNameSaveTo = string.Join("/", geoJson, geospatialDataFileNameSaveTo);
        if (File.Exists(geospatialDataFileName))
        {
            File.Copy(geospatialDataFileName, geospatialDataFileNameSaveTo);
        }

        string pics = string.Join("/", nameOfAlbum, "pics");
        if (Directory.Exists(pics))
        {
            string[] picsFiles = Directory.GetFiles(pics);
            string picsDestination = string.Join("/", prepareForUploadFolder, "pics");
            foreach (string picsFile in picsFiles)
            {
                File.Copy(picsFile, string.Join("/", picsDestination, Path.GetFileName(picsFile)));
            }
        }

        string thumbs = string.Join("/", nameOfAlbum, "thumbs");
        if (Directory.Exists(thumbs))
        {
            string[] thumbsFiles = Directory.GetFiles(thumbs);
            string thumbsDestination = string.Join("/", prepareForUploadFolder, "thumbs");
            foreach (string thumbsFile in thumbsFiles)
            {
                File.Copy(thumbsFile, Path.Join(thumbsDestination, Path.GetFileName(thumbsFile)));
            }
        }
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        Directory.CreateDirectory(targetPath);

        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}