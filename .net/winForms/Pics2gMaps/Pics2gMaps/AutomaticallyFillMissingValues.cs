using System.Data;
using Common;

namespace Pics2gMaps;

public class AutomaticallyFillMissingValues : ICommandHandler<AutomaticallyFillMissingValuesCommand>
{
    public void Execute(AutomaticallyFillMissingValuesCommand command)
    {
        DoAutomaticallyFillMissingValues(command);
    }

    private void DoAutomaticallyFillMissingValues(AutomaticallyFillMissingValuesCommand command)
    {
        if (
            string.IsNullOrWhiteSpace(command.GalleryName)
            || string.IsNullOrWhiteSpace(command.RootGalleryFolder)
            || string.IsNullOrWhiteSpace(command.OgTitle)
            || string.IsNullOrWhiteSpace(command.OgImage)
        )
        {
            throw new NoNullAllowedException("GalleryName, RootGalleryFolder, OgTitle or OgImage are not allowed to be null");
        }

        string relativeWebPath = ConvertWindowsPathToWebPath(command.RootGalleryFolder);

        if (string.IsNullOrWhiteSpace(command.WebPath))
        {
            command.WebPath = ConvertToUrl(command.RootGalleryFolder, command.BaseUrl ?? string.Empty);
        }

        string galleryFullWebPath = command.WebPath +
                                    command.GalleryName;

        if (string.IsNullOrWhiteSpace(command.OgUrl))
        {
            command.OgUrl = galleryFullWebPath + "/www/index.html";
        }

        if (string.IsNullOrWhiteSpace(command.PicsJson))
        {
            command.PicsJson = command.GalleryName;
        }

        if (string.IsNullOrWhiteSpace(command.JoomlaImgSrcPath))
        {
            command.JoomlaImgSrcPath = relativeWebPath + command.GalleryName + "/www/";
        }

        if (string.IsNullOrWhiteSpace(command.JoomlaThumbsPath))
        {
            command.JoomlaThumbsPath = command.JoomlaImgSrcPath + command.GalleryName + "Thumbs.json";
        }

        if (string.IsNullOrWhiteSpace(command.OgImageFullPath))
        {
            galleryFullWebPath = GetGalleryFullWebPath(command.WebPath
                , command.RootGalleryFolder
                , command.BaseUrl ?? string.Empty
                , command.GalleryName);

            command.OgImageFullPath = galleryFullWebPath + "/" + command.OgImage;
        }

    }

    private string GetGalleryFullWebPath(string webPath, string rootGalleryFolder, string baseUrl, string galleryName)
    {
        string galleryFullWebPath;
        if (string.IsNullOrWhiteSpace(webPath))
        {
            galleryFullWebPath = ConvertToUrl(rootGalleryFolder, baseUrl) + galleryName;
        }
        else
        {
            galleryFullWebPath = webPath + galleryName;
        }

        return galleryFullWebPath;
    }

    static string ConvertToUrl(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("URL is not allowed to be empty.");

        baseUrl = AddHttp(baseUrl);

        return $"{baseUrl.TrimEnd('/')}{ConvertWindowsPathToWebPath(path)}";
    }

    private static string AddHttp(string baseUrl)
    {
        if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = "http://" + baseUrl;
        }

        return baseUrl;
    }

    static string ConvertWindowsPathToWebPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is not allowed to be empty.");

        string webPath = path.Replace("\\", "/");

        int index = webPath.IndexOf("/", StringComparison.Ordinal);
        if (index != -1)
        {
            webPath = webPath.Substring(index);
        }

        return $"{webPath.TrimEnd('/')}/";
    }
}