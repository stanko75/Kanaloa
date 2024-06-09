using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using FunctionalTest.Log;
namespace FunctionalTest;

public class UploadToBlogHandler(ILogger logger) : ICommandHandler<UploadToBlogCommand>
{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    public async Task Execute(UploadToBlogCommand command)
    {
        string kmlFileName = command.KmlFileName;
        string folderName = command.FolderName;
        string addressText = command.AddressText;
        string ftpHost = command.FtpHost;
        string ftpUser = command.FtpUser;
        string ftpPass = command.FtpPass;

        if (CancellationTokenSource is not null)
        {
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            HttpClient httpClientPost = command.HttpClientPost;

            var jObjectKmlFileFolder = new JObject
            {
                ["folderName"] = folderName,
                ["kmlFileName"] = kmlFileName,
                ["host"] = ftpHost,
                ["user"] = ftpUser,
                ["pass"] = ftpPass
            };

            string jsonContent = jObjectKmlFileFolder.ToString();
            StringContent content = new StringContent(jsonContent);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string requestUri = Path.Combine(addressText, @"api/UpdateCoordinates/UploadToBlog");
            logger.Log("Sending");

            try
            {
                HttpResponseMessage httpResponseMessage = await httpClientPost.PostAsync(requestUri, content, cancellationToken);
                logger.Log(httpResponseMessage.StatusCode.ToString());
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                    logger.Log(new Exception(errorMessage));
                    throw new Exception(errorMessage);
                }

                string okMessage = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                logger.Log(okMessage);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                throw;
            }

            string[] fileUrls =
            [
                "www/css/index.css",
                "www/lib/jquery-3.6.4.js",
                "www/script/map.js",
                "www/script/namespaces.js",
                "www/script/namespaces.js",
                "www/config.json",
                "www/index.html"
            ];

            try
            {
                foreach (string url in fileUrls)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string milosevUrl = $"http://milosev.com/gallery/allWithPics/travelBuddies/{folderName}/";
                    Uri baseUri = new Uri(milosevUrl);
                    Uri uri = new Uri(baseUri, url);

                    HttpResponseMessage response = await httpClientPost.GetAsync(uri.AbsoluteUri, cancellationToken);
                    logger.Log(response.IsSuccessStatusCode
                        ? @$"File: {uri.AbsoluteUri} exists"
                        : @$"Request failed with status code: {response.StatusCode}, file: {uri.AbsoluteUri}");
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                throw;
            }
        }
    }
}