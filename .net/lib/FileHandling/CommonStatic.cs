using System.Text.Json;

namespace FileHandling;

public static class CommonStatic
{
    public static string WriteConfigurationToJsonForBlog(string strAbsolutePath, string kmlFileNameWithRelativePath)
    {
        Uri kmlUrl =
            ConvertRelativeWindowsPathToUri(strAbsolutePath, kmlFileNameWithRelativePath);
        var objConfig = new
        {
            kmlUrl = kmlUrl.AbsoluteUri
        };
        return JsonSerializer.Serialize(objConfig);
    }

    public static Uri ConvertRelativeWindowsPathToUri(string strAbsolutePath, string relativeWindowsPath)
    {
        Uri uriAbsolute = new Uri(strAbsolutePath);
        return new Uri(uriAbsolute, relativeWindowsPath);
    }
}