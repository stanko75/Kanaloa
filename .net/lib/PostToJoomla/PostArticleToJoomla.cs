using Common;

namespace PostToJoomla;

public class PostArticleToJoomla: ICommandHandlerAsync<PostArticleToJoomlaCommand>
{
    public async Task Execute(PostArticleToJoomlaCommand command)
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("jform[title]", command.Title),
            new KeyValuePair<string, string>("jform[catid]", command.CatId),
            new KeyValuePair<string, string>("jform[language]", "*"),
            new KeyValuePair<string, string>("jform[state]", "1"),
            new KeyValuePair<string, string>("jform[articletext]", command.ArticleText),
            new KeyValuePair<string, string>("task", "article.save"),
            new KeyValuePair<string, string>(command.TokenName, "1")
        });

        HttpResponseMessage postResponse = await command.HttpClient.PostAsync(command.Url, formData);
        string postResultHtml = await postResponse.Content.ReadAsStringAsync();

        command.IsSaved = postResultHtml.Contains("Article saved.");
    }
}