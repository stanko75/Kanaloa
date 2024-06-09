using FunctionalTest.Log;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;

namespace FunctionalTest;

public class StaticCommon
{
    public static async Task<JObject> GetConfigJson(string uri, HttpClient httpClient, ILogger logger)
    {
        try
        {
            string configJsonString = await httpClient.GetStringAsync(uri);
            return JObject.Parse(configJsonString);
        }
        catch (Exception ex)
        {
            logger.Log(new Exception("There is an error with config.json: " + uri, ex));
            throw new Exception(ex.Message);
        }
    }

    public static Uri GetConfigJsonUri(string addressText)
    {
        Uri baseUri = new Uri(addressText);
        return new Uri(baseUri, "config.json");
    }

    public static UriBuilder CheckConfigJson(string addressText, string folderNameString, string? fileNameInConfigJsonOnWeb,
        string localFileName,
        string extension, ILogger logger, bool checkInRoot = false)
    {
        UriBuilder uriBuilder = new UriBuilder(addressText);
        uriBuilder.Path = checkInRoot
            ? Path.ChangeExtension(localFileName, extension)
            : Path.Combine(folderNameString, Path.ChangeExtension(localFileName, extension));
        Uri fileNameUri = uriBuilder.Uri;
        if (!string.Equals(fileNameInConfigJsonOnWeb, fileNameUri.AbsoluteUri,
                StringComparison.InvariantCultureIgnoreCase))
        {
            string message =
                $"There is an error in config.json! {fileNameInConfigJsonOnWeb} is not equal {fileNameUri.AbsoluteUri}!";
            logger.Log(new Exception(message));
            throw new Exception(message);
        }

        return uriBuilder;
    }
}