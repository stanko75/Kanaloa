using System.Net;
using System.Text.RegularExpressions;

namespace PostToJoomla;

public static class CommonStatic
{
    public static string ExtractTokenName(string html)
    {
        var regex = new Regex(@"""csrf\.token"":""(?<token>[a-f0-9]{32})""");
        var match = regex.Match(html);

        if (match.Success)
        {
            return match.Groups["token"].Value;
        }

        throw new Exception("CSRF-Token not found.");
    }

    public static HttpClient CreateHttpClient()
    {
        HttpClientHandler httpClientHandler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true,
            AllowAutoRedirect = true
        };
        return new HttpClient(httpClientHandler);
    }

    public static async Task<string?> OpenJoomlaAdminPage(HttpClient? client, string? loginUrl)
    {
        OpenJoomlaAdminPageCommand openCommand = new OpenJoomlaAdminPageCommand
        {
            HttpClient = client,
            AdministratorUrl = loginUrl
        };
        OpenJoomlaAdminPage openJoomlaAdminPage = new OpenJoomlaAdminPage();
        await openJoomlaAdminPage.Execute(openCommand);
        return openCommand.TokenName;
    }

    public static async Task<bool> LoginToJoomla(HttpClient? client, string? loginUrl, string userName, string pass,
        string? tokenName)
    {
        LoginToJoomlaCommand loginToJoomlaCommand = new LoginToJoomlaCommand
        {
            HttpClient = client,
            Url = loginUrl,
            Username = userName,
            Password = pass,
            TokenName = tokenName,
        };
        LoginToJoomla loginToJoomla = new LoginToJoomla();
        await loginToJoomla.Execute(loginToJoomlaCommand);
        return loginToJoomlaCommand.IsLoginSuccessful;
    }

    public static async Task<string?> OpenAddArticle(HttpClient client, bool isLoginSuccessful, string? postUrl, string? tokenName)
    {
        if (!isLoginSuccessful)
        {
            throw new Exception("Login failed!");
        }

        OpenAddArticleCommand openAddArticleCommand = new OpenAddArticleCommand
        {
            AddArticleUrl = postUrl,
            HttpClient = client,
            TokenName = tokenName
        };
        OpenAddArticle openAddArticle = new OpenAddArticle();
        await openAddArticle.Execute(openAddArticleCommand);
        return openAddArticleCommand.TokenName;
    }

    public static async Task<bool> PostArticleToJoomlaCommand(HttpClient client
        , string postUrl
        , string title
        , string catId
        , string articleText
        , string? tokenName)
    {
        PostArticleToJoomlaCommand postArticleToJoomlaCommand = new PostArticleToJoomlaCommand
        {
            HttpClient = client,
            Url = postUrl,
            Title = title,
            CatId = catId,
            ArticleText = articleText,
            TokenName = tokenName
        };
        PostArticleToJoomla postArticleToJoomla = new PostArticleToJoomla();
        await postArticleToJoomla.Execute(postArticleToJoomlaCommand);
        return postArticleToJoomlaCommand.IsSaved;
    }
}