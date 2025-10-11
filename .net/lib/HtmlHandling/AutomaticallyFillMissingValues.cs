using Common;
using System.Data;

namespace HtmlHandling;

public class AutomaticallyFillMissingValues(IPathConverter pathConverter) : ICommandHandler<AutomaticallyFillMissingValuesCommand>
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

        command.WebPath = ConvertToUrl("gallery/" + command.RootGalleryFolder, command.BaseUrl ?? string.Empty);
        command.WebPath = string.IsNullOrWhiteSpace(command.WebPath) ? ConvertToUrl(command.RootGalleryFolder, command.BaseUrl ?? string.Empty) : command.WebPath;

        string galleryFullWebPath = command.WebPath +
                                    command.GalleryName;

        command.OgUrl = string.IsNullOrWhiteSpace(command.OgUrl) ? galleryFullWebPath + "/www/index.html" : command.OgUrl;
        command.PicsJson = string.IsNullOrWhiteSpace(command.PicsJson) ? command.GalleryName : command.PicsJson;
        command.JoomlaImgSrcPath = string.IsNullOrWhiteSpace(command.JoomlaImgSrcPath) ? relativeWebPath + command.GalleryName + "/www/" : command.JoomlaImgSrcPath;
        command.JoomlaImgSrcPath = command.JoomlaImgSrcPath.StartsWith('/')
            ? command.JoomlaImgSrcPath
            : "/" + command.JoomlaImgSrcPath;
        command.JoomlaThumbsPath = string.IsNullOrWhiteSpace(command.JoomlaThumbsPath) ? command.JoomlaImgSrcPath + command.GalleryName + "Thumbs.json" : command.JoomlaThumbsPath;
        command.JoomlaThumbsPath = command.JoomlaThumbsPath.StartsWith('/')
            ? command.JoomlaThumbsPath
            : "/" + command.JoomlaThumbsPath;

        command.Kml = string.IsNullOrWhiteSpace(command.Kml) ? galleryFullWebPath + "/kml/kml" : galleryFullWebPath + '/' + ConvertWindowsPathToWebPath(command.Kml).TrimEnd('/').TrimStart('/');

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

    string ConvertToUrl(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("URL is not allowed to be empty.");

        baseUrl = AddHttp(baseUrl);
        baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";

        return $"{baseUrl}{ConvertWindowsPathToWebPath(path).TrimStart('/')}";
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

    string ConvertWindowsPathToWebPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is not allowed to be empty.");

        string webPath = path.Replace("\\", "/");

        webPath = pathConverter.Execute(webPath);

        return $"{webPath.TrimEnd('/')}/";
    }
}