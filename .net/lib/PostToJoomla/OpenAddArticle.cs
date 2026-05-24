using Common;

namespace PostToJoomla;

public class OpenAddArticle: ICommandHandlerAsync<OpenAddArticleCommand>
{
    public async Task Execute(OpenAddArticleCommand command)
    {
        command.Ok = false;

        if (command.HttpClient != null)
        {
            HttpResponseMessage createResponse = await command.HttpClient.GetAsync(command.AddArticleUrl);
            string createHtml = await createResponse.Content.ReadAsStringAsync();
            command.TokenName = CommonStatic.ExtractTokenName(createHtml);
            command.Ok = true;
        }
        else
        {
            throw new ArgumentNullException(nameof(command.HttpClient), "HttpClient cannot be null.");
        }
    }
}