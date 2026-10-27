using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using FunctionalTest.Log;

namespace FunctionalTest;

public class PostGpsPositionsFromFilesWithFileNameHandler(ILogger logger)
    : ICommandHandler<PostGpsPositionsFromFilesWithFileNameCommand>
{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    public async Task Execute(PostGpsPositionsFromFilesWithFileNameCommand command)
    {
        string addressText = command.AddressText;
        string gpsLocationsPath = command.GpsLocationsPath;
        CancellationToken? cancellationToken = CancellationTokenSource?.Token;
        HttpClient httpClientPost = command.HttpClientPost;

        JObject jObjectGeospatialDataFileFolderLatLng = new JObject
        {
            ["folderName"] = command.FolderName,
            ["geospatialDataFileName"] = command.GeospatialDataFileName
        };

        if (Directory.Exists(gpsLocationsPath))
        {
            foreach (string file in Directory.GetFiles(gpsLocationsPath))
            {
                cancellationToken?.ThrowIfCancellationRequested();
                JObject myJObject = JObject.Parse(await File.ReadAllTextAsync(file));
                jObjectGeospatialDataFileFolderLatLng["Longitude"] = myJObject["lng"] ?? myJObject["Longitude"];
                jObjectGeospatialDataFileFolderLatLng["Latitude"] = myJObject["lat"] ?? myJObject["Latitude"];

                string requestUri = Path.Combine(addressText, @"api/UpdateCoordinates/PostFileFolder");
                StringContent content = new StringContent($@"{jObjectGeospatialDataFileFolderLatLng}", Encoding.UTF8, "text/json");

                logger.Log($"Sending: {jObjectGeospatialDataFileFolderLatLng}");
                logger.Log($"requestUri: {requestUri}");

                HttpResponseMessage httpResponseMessage = await httpClientPost.PostAsync(requestUri, content);

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    logger.Log($"{httpResponseMessage.StatusCode.ToString()}");
                }
                else
                {
                    string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        throw new Exception($"""
                            Status: {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.StatusCode})
                            Reason: {httpResponseMessage.ReasonPhrase}
                            Version: {httpResponseMessage.Version}
                            Request: {httpResponseMessage.RequestMessage?.Method} {httpResponseMessage.RequestMessage?.RequestUri}
                            Headers:
                            {httpResponseMessage.Headers}
                            Content Headers:
                            {httpResponseMessage.Content.Headers}
                            Body:
                            {httpResponseMessage.Content.ReadAsStringAsync().Result}
                        """);                    }
                    else 
                    {
                        throw new Exception($"{errorMessage}");
                    }
                }

                JObject configJson =
                    await StaticCommon.GetConfigJson(StaticCommon.GetConfigJsonUri(addressText).AbsoluteUri,
                        httpClientPost, logger);

                string? klmFileName = configJson["GeospatialDataFileName"]?.ToString();
                string? currentLocation = configJson["CurrentLocation"]?.ToString();
                //string? liveImageMarkersJsonUrl = configJson?["LiveImageMarkersJsonUrl"]?.ToString();

                UriBuilder geospatialDataUri = StaticCommon.CheckConfigJson(addressText, command.FolderName, klmFileName,
                    command.GeospatialDataFileName, command.GeospatialDataFileNameExtension, logger);
                UriBuilder testJsonUri =
                    StaticCommon.CheckConfigJson(addressText, command.FolderName, currentLocation, "live", "json",
                        logger, true);
                try
                {
                    await httpClientPost.GetStringAsync(geospatialDataUri.Uri.AbsoluteUri);
                }
                catch (Exception ex)
                {
                    logger.Log(new Exception("There is error with geospatialDataFileName:" + geospatialDataUri.Uri.AbsoluteUri));
                    logger.Log(ex);
                    throw;
                }

                try
                {
                    await httpClientPost.GetStringAsync(testJsonUri.Uri.AbsoluteUri);
                }
                catch (Exception ex)
                {
                    logger.Log(new Exception("There is error with test.json:" + testJsonUri.Uri.AbsoluteUri));
                    logger.Log(ex);
                    throw;
                }

                await Task.Delay(2000);

            }
        }
    }
}