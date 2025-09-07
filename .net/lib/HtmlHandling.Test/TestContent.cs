using System.Text.RegularExpressions;

namespace HtmlHandling.Test;

public static class TestContent
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

            testMethod(ogMatch, og.Value, $"og:{og.Key} is wrong", $"og:{og.Key} not found!", $"og:{og.Key} was found! and is '{og.Value}'");
        }
    }

    public static void TestJoomlaPreviewHtml(string joomlaPreviewHtml, string folderName, string imgSrc,
        string aHrefText, Action<Match, string, string, string, string, int> testMethod)
    {
        var hrefMatch = Regex.Match(
            joomlaPreviewHtml,
            @"<a\s+href=""([^""]+)""[^>]*>([^<]+)</a>",
            RegexOptions.IgnoreCase
        );
        testMethod(hrefMatch, $"/prepareForUpload/{folderName}/www/index.html", "href is wrong", "href not found!", "href OK", 1);
        testMethod(hrefMatch, aHrefText, "href text is wrong", "href text not found!", "href text is OK", 2);

        var scriptSrcMatch = Regex.Match(
            joomlaPreviewHtml,
            @"<script\s+[^>]*src=[""']([^""']+)[""']",
            RegexOptions.IgnoreCase
        );
        testMethod(scriptSrcMatch, $"/prepareForUpload/{folderName}/www/lib/", "jQuery.getJSON is wrong", "jQuery.getJSON not found!", "jQuery.getJSON OK", 1);

        var jsonMatch = Regex.Match(
            joomlaPreviewHtml,
            @"getJSON\s*\(\s*[""']([^""']+)[""']",
            RegexOptions.IgnoreCase
        );
        testMethod(jsonMatch, $"/prepareForUpload/{folderName}/www/{folderName}Thumbs.json", "src is wrong", "src not found!", "src OK", 1);

        var jqIdMatch = Regex.Match(
            joomlaPreviewHtml,
            @"jQuery\(\s*[""']([^""']+)[""']\s*\)",
            RegexOptions.IgnoreCase
        );
        testMethod(jqIdMatch, $"#jPreview{folderName}", "jQuery-Selector is wrong", "jQuery-Selector not found!", "jQuery-Selector OK", 1);

        var hrefMatch2 = Regex.Match(joomlaPreviewHtml, @"<a\s+href=""([^""]+)""[^>]*>\s*<img\s+[^>]*id=""([^""]+)""[^>]*src=""([^""]+)""", RegexOptions.IgnoreCase);
        testMethod(hrefMatch2, $"/prepareForUpload/{folderName}/www/index.html", "second href is wrong", "second href not found!", "second href OK", 1);
        testMethod(hrefMatch2, $"jPreview{folderName}", "img id is wrong", "img id not found!", "img id OK", 2);
        testMethod(hrefMatch2, $"/prepareForUpload/{folderName}/www/../{imgSrc}", "second href src is wrong", "second href src not found!", "second href src OK", 3);
    }

    public static void TestPics2mapsJs(string pics2mapsJsContent, string folderName, Action<Match, string, string, string, string, int> testMethod)
    {
        Match match = Regex.Match(pics2mapsJsContent, @"\$\.getJSON\(\s*[""']([^""']+)[""']");
        testMethod(match, $"{folderName}.json", "notEqualMessage", "notFoundMessage", $"{folderName}.json found - OK", 1);
    }
}