using Common;

namespace PostToJoomla;

public class OpenAdminPageAndPostArticle: ICommandHandlerAsync<OpenAdminPageAndPostArticleCommand>
{
    public async Task Execute(OpenAdminPageAndPostArticleCommand command)
    {
        var client = CommonStatic.CreateHttpClient();
        string? tokenName = await CommonStatic.OpenJoomlaAdminPage(client, command.LoginUrl);
        bool isLoginSuccessful = await CommonStatic.LoginToJoomla(client, command.LoginUrl, command.UserName, command.Pass, tokenName);
        tokenName = await CommonStatic.OpenAddArticle(client, isLoginSuccessful, command.LoginUrl, tokenName);
        command.IsSaved = await CommonStatic.PostArticleToJoomlaCommand(client, command.PostUrl, command.Title, command.CategoryId, command.ArticleText, tokenName);
    }
}