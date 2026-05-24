using Common;

namespace PostToJoomla;

public class OpenJoomlaAdminPage : ICommandHandlerAsync<OpenJoomlaAdminPageCommand>
{
    public async Task Execute(OpenJoomlaAdminPageCommand command)
    {
        command.Ok = false;
        if (command.HttpClient != null)
        {
            HttpResponseMessage getResponse = await command.HttpClient.GetAsync(command.AdministratorUrl);
            string html = await getResponse.Content.ReadAsStringAsync();

            while (html.ToLower().Contains("Please wait while".ToLower()))
            {
                await Task.Delay(500);
                html = await getResponse.Content.ReadAsStringAsync();
            }

            command.TokenName = CommonStatic.ExtractTokenName(html);
            command.Ok = true;
        }
        else
        {
            throw new ArgumentNullException(nameof(command.HttpClient), "HttpClient cannot be null.");
        }
    }
}