using Common;
using System.Net.Http.Headers;
using System.Text;

namespace UploadWithPhpScriptHandling;

public class PhpUpload: ICommandHandlerAsync<PhpUploadCommand>
{
    public async Task Execute(PhpUploadCommand command)
    {
        await UploadFileAsync(command.Url
            , command.FullFileName
            , command.UploadPath
            , command.UserName
            , command.Password
        );
    }

    private async Task<string> UploadFileAsync(string? url
        , string? fullFileName
        , string? uploadPath
        , string? userName
        , string? password
        )
    {
        using var client = new HttpClient();

        string auth = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{userName}:{password}")
        );

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", auth);

        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(uploadPath), "folder");
        form.Add(new StringContent(Path.GetFileName(fullFileName)), "fileName");

        byte[] fileBytes = await File.ReadAllBytesAsync(fullFileName);

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        form.Add(fileContent, "file", Path.GetFileName(fullFileName));

        HttpResponseMessage response = await client.PostAsync(url, form);

        string result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(result);

        return result;
    }
}