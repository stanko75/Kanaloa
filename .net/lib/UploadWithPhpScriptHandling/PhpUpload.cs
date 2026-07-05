using System.Diagnostics;
using Common;
using Microsoft.Playwright;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace UploadWithPhpScriptHandling;

public class PhpUpload: ICommandHandlerAsync<PhpUploadCommand>
{
    public async Task Execute(PhpUploadCommand command)
    {
        command.Result = await UploadFileAsync(command.Url
            , command.FullFileName
            , command.RemoteRootFolder
            , command.UserName
            , command.Password
            , command.AlbumRoot
        );
    }

    private async Task<string> UploadFileAsync(
        string url,
        string fullFileName,
        string remoteRootFolder,
        string userName,
        string password,
        string albumRoot)
    {

        string pathWithoutRoot = Path.GetDirectoryName(fullFileName).Replace(albumRoot + "\\", string.Empty);
        string remoteFolder = $"{remoteRootFolder}/{pathWithoutRoot}";

        var browserPath = Path.Combine(
            AppContext.BaseDirectory,
            "ms-playwright");

        if (!Directory.Exists(browserPath))
        {
            throw new InvalidOperationException(
                $"Browser path {browserPath} does not exist");
        }

        Environment.SetEnvironmentVariable(
            "PLAYWRIGHT_BROWSERS_PATH",
            browserPath);

        using var playwright = await Playwright.CreateAsync();

        await using var browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

        var context =
            await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    HttpCredentials = new HttpCredentials
                    {
                        Username = userName,
                        Password = password
                    }
                });

        var page = await context.NewPageAsync();

        page.Console += (_, msg) =>
        {
            Console.WriteLine(
                $"BROWSER CONSOLE [{msg.Type}]: {msg.Text}");
        };

        page.PageError += (_, msg) =>
        {
            Console.WriteLine(
                $"PAGE ERROR: {msg}");
        };

        // Startseite laden damit Browser komplett initialisiert ist
        await page.GotoAsync(url);
        


            Console.WriteLine("test");

        await page.WaitForFunctionAsync(
            "() => !document.body.innerText.includes('Please wait while your request is being verified...')",
            null,
            new()
            {
                Timeout = 30000
            });

        byte[] fileBytes =
            await File.ReadAllBytesAsync(fullFileName);

        string base64 =
            Convert.ToBase64String(fileBytes);

        var result =
            await page.EvaluateAsync<string>(
                @"async (data) => {

                try {

                    const bytes = Uint8Array.from(
                        atob(data.fileBase64),
                        c => c.charCodeAt(0));

                    const blob = new Blob(
                        [bytes],
                        { type: 'application/octet-stream' });

                    const form = new FormData();

                    form.append(
                        'folder',
                        data.folder);

                    form.append(
                        'fileName',
                        data.fileName);

                    form.append(
                        'file',
                        blob,
                        data.fileName);

                    const response =
                        await fetch(
                            data.url,
                            {
                                method: 'POST',
                                headers: {
                                    'Authorization':
                                        'Basic ' +
                                        btoa(
                                            data.user +
                                            ':' +
                                            data.password)
                                },
                                body: form
                            });

                    const text =
                        await response.text();

                    return JSON.stringify({
                        status: response.status,
                        ok: response.ok,
                        text: text
                    });
                }
                catch(ex) {
                    return 'JS ERROR: ' +
                           ex.toString();
                }
            }",
                new
                {
                    url,
                    folder = remoteFolder,
                    fileName = Path.GetFileName(fullFileName),
                    fileBase64 = base64,
                    user = userName,
                    password
                });

        return result;
    }

    private async Task<string> UploadFileAsyncHttpClient(string? url
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