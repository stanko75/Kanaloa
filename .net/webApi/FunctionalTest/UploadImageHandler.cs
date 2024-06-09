using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using FunctionalTest.Log;

namespace FunctionalTest;

public class UploadImageHandler(ILogger logger) : ICommandHandler<UploadImageCommand>

{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    public async Task Execute(UploadImageCommand command)
    {
        string kmlFileName = command.KmlFileName;
        string folderName = command.FolderName;
        string addressText = command.AddressText;
        string imagesPath = command.ImagesPath;
        if (CancellationTokenSource is not null)
        {
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            HttpClient httpClientPost = command.HttpClientPost;

            if (string.IsNullOrWhiteSpace(kmlFileName)) kmlFileName = "default";
            if (string.IsNullOrWhiteSpace(folderName)) folderName = "default";

            if (Directory.Exists(imagesPath))
            {
                foreach (string imageFile in Directory.GetFiles(imagesPath))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string base64Image = ConvertImageToBase64(imageFile);

                    JObject jObjectKmlFileFolder = new JObject();
                    jObjectKmlFileFolder["folderName"] = folderName;
                    jObjectKmlFileFolder["kmlFileName"] = kmlFileName;
                    jObjectKmlFileFolder["base64Image"] = base64Image;
                    jObjectKmlFileFolder["imageFileName"] = Path.GetFileName(imageFile);

                    string jsonContent = jObjectKmlFileFolder.ToString();
                    StringContent content = new StringContent(jsonContent);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    string requestUri = Path.Combine(addressText, @"api/UpdateCoordinates/UploadImage");
                    logger.Log("Sending");

                    try
                    {
                        HttpResponseMessage httpResponseMessage =
                            await httpClientPost.PostAsync(requestUri, content, cancellationToken);
                        logger.Log(httpResponseMessage.StatusCode.ToString());
                        if (!httpResponseMessage.IsSuccessStatusCode)
                        {
                            string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                            logger.Log(errorMessage);
                            throw new Exception(errorMessage);
                        }

                        string message = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                        logger.Log(message);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex);
                        throw;
                    }

                    JObject configJson = await StaticCommon.GetConfigJson(StaticCommon.GetConfigJsonUri(addressText).AbsoluteUri, httpClientPost, logger);
                    string? liveImageMarkersJson = configJson["LiveImageMarkersJsonUrl"]?.ToString();

                    UriBuilder liveImageMarkersJsonUri = StaticCommon.CheckConfigJson(addressText, folderName, liveImageMarkersJson, $"{kmlFileName}Thumbs", "json", logger);
                    string liveImageMarkersJsonString;
                    try
                    {
                        liveImageMarkersJsonString = await httpClientPost.GetStringAsync(liveImageMarkersJsonUri.Uri.AbsoluteUri, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(new Exception("There is error with thumbs file: " + liveImageMarkersJsonUri.Uri.AbsoluteUri));
                        logger.Log(ex);
                        throw;
                    }

                    if (!liveImageMarkersJsonString.Contains(Path.GetFileName(imageFile)))
                    {
                        string message = $"{Path.GetFileName(imageFile)} is not included in thumbs!";
                        logger.Log(new Exception(message));
                        throw new Exception(message);
                    }

                }
            }
        }
    }

    static string ConvertImageToBase64(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}