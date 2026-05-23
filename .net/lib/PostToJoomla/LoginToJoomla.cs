using Common;

namespace PostToJoomla;

public class LoginToJoomla: ICommandHandlerAsync<LoginToJoomlaCommand>
{
    public async Task Execute(LoginToJoomlaCommand command)
    {
        var tokenValue = "1";
        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string?>("username", command.Username),
            new KeyValuePair<string, string?>("passwd", command.Password),
            new KeyValuePair<string, string>("option", "com_login"),
            new KeyValuePair<string, string>("task", "login"),
            new KeyValuePair<string?, string>(command.TokenName, tokenValue)
        ]);

        if (command.HttpClient != null)
        {
            HttpResponseMessage postResponse = await command.HttpClient.PostAsync(command.Url, formContent);
            string postResult = await postResponse.Content.ReadAsStringAsync();
            command.IsLoginSuccessful = postResult.Contains("mod_quickicon") || postResult.Contains("cpanel");
        }
        else
        {
            throw new ArgumentNullException(nameof(command.HttpClient), "HttpClient cannot be null.");
        }
    }
}