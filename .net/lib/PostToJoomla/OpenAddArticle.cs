using Common;

namespace PostToJoomla;

public class OpenAddArticle: ICommandHandlerAsync<OpenAddArticleCommand>
{
    public async Task Execute(OpenAddArticleCommand command)
    {
        if (command.HttpClient != null)
        {
            HttpResponseMessage createResponse = await command.HttpClient.GetAsync(command.AddArticleUrl);
            string createHtml = await createResponse.Content.ReadAsStringAsync();
            command.TokenName = CommonStatic.ExtractTokenName(createHtml);
            return true;
        }

        throw new ArgumentNullException(nameof(command.HttpClient), "HttpClient cannot be null.");
    }
}