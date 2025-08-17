using System.Text.RegularExpressions;

namespace HtmlHandling.Test;

public class TestContent
{
    public static void TestIndexHtmlOgs(string indexHtml, string baseUrl, string folderName, string ogImage, string ogTitle, Action<Match, string, string, string, string> testMethod)
    {
        var ogs = new Dictionary<string, string>
        {
            { "url", $"http://{baseUrl}/prepareForUpload/{folderName}/www/index.html" },
            { "image", $"http://{baseUrl}/prepareForUpload/{folderName}/{ogImage}" },
            { "title", ogTitle }
        };

        foreach (KeyValuePair<string, string> og in ogs)
        {
            Match ogMatch = Regex.Match(
                indexHtml,
                @$"<meta\s+property=[""']og:{og.Key}[""']\s+content=[""']([^""']+)[""']",
                RegexOptions.IgnoreCase
            );

            testMethod(ogMatch, og.Value, $"og:{og.Key} is wrong", $"og:{og.Key} not found!", $"og:{og.Key} was found!");
        }
    }
}