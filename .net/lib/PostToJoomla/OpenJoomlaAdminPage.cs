using Common;

namespace PostToJoomla;

public class OpenJoomlaAdminPage : ICommandHandlerAsync<OpenJoomlaAdminPageCommand>
{
    public async Task Execute(OpenJoomlaAdminPageCommand command)
    {
        if (command.HttpClient != null)
        {
            HttpResponseMessage getResponse = await command.HttpClient.GetAsync(command.AdministratorUrl);
            string html = await getResponse.Content.ReadAsStringAsync();
            command.TokenName = CommonStatic.ExtractTokenName(html);
            command.Ok = true;
        }

        throw new ArgumentNullException(nameof(command.HttpClient), "HttpClient cannot be null.");
    }
}