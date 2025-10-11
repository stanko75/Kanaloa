using FunctionalTest.Log;
using HtmlHandling.Test;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Policy;

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
        string ogTitle = command.OgTitle;
        string ogImage = command.OgImage;
        string baseUrl = command.BaseUrl;
        string expectedUrl = command.ExpectedUrl;
        string prepareForUpload = command.PrepareForUpload;

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
                ["pass"] = ftpPass,
                ["ogTitle"] = ogTitle,
                ["ogImage"] = ogImage,
                ["baseUrl"] = baseUrl
            };

            string jsonContent = jObjectKmlFileFolder.ToString();
            StringContent content = new StringContent(jsonContent);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Uri addressTextUri = new Uri(addressText.EndsWith("/") ? addressText : addressText + "/");
            Uri requestUri = new Uri(addressTextUri, "api/UploadToBlog/UploadToBlog");

            logger.Log($"Sending: {requestUri}");

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
                "www/script/pics2maps.js",
                "www/script/thumbnails.js",
                "www/script/namespaces.js",
                "www/index.html",
                "www/joomlaPreview.html"
            ];

            try
            {
                Uri prepareForUploadUri;
                if (Uri.TryCreate(prepareForUpload, UriKind.Absolute, out var uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    prepareForUploadUri = new Uri(prepareForUpload.TrimEnd('/'));
                }
                else
                {
                    prepareForUploadUri = new Uri(new Uri(addressText.TrimEnd('/')), prepareForUpload);
                }
                if (!prepareForUploadUri.AbsoluteUri.EndsWith("/"))
                {
                    prepareForUploadUri = new Uri(prepareForUploadUri.AbsoluteUri + "/");
                }
                prepareForUploadUri = new Uri(prepareForUploadUri, folderName + "/");

                logger.Log("***");
                logger.Log("check if files are uploaded");
                logger.Log("***");
                foreach (string url in fileUrls)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    //string milosevUrl = $"http://milosev.com/gallery/allWithPics/travelBuddies/{folderName}/";
                    string milosevUrl = $"{expectedUrl.TrimEnd('/')}/{folderName}/";
                    Uri baseUri = new Uri(milosevUrl);
                    Uri uri = new Uri(baseUri, url);
                    logger.Log($"Sending: {uri}");

                    HttpResponseMessage response = await httpClientPost.GetAsync(uri.AbsoluteUri, cancellationToken);
                    logger.Log(response.IsSuccessStatusCode
                        ? @$"File: {uri.AbsoluteUri} exists"
                        : @$"Request failed with status code: {response.StatusCode}, file: {uri.AbsoluteUri}");
                }

                logger.Log("***");
                logger.Log("check if files are in prepareForUpload ");
                logger.Log("***");
                foreach (string url in fileUrls)
                {
                    Uri prepareForUploadFileUri = new Uri(prepareForUploadUri, url);
                    HttpResponseMessage prepareForUploadResponse = await httpClientPost.GetAsync(prepareForUploadFileUri.AbsoluteUri, cancellationToken);
                    logger.Log(prepareForUploadResponse.IsSuccessStatusCode
                        ? @$"File: {prepareForUploadFileUri.AbsoluteUri} exists"
                        : @$"Request failed with status code: {prepareForUploadResponse.StatusCode}, file: {prepareForUploadFileUri.AbsoluteUri}");
                }

                //check content of files in prepareForUpload
                foreach (string url in fileUrls)
                {
                    Uri prepareForUploadFileUri = new Uri(prepareForUploadUri, url);
                    HttpResponseMessage prepareForUploadResponse = await httpClientPost.GetAsync(prepareForUploadFileUri.AbsoluteUri, cancellationToken);
                    string prepareForUploadContent = await prepareForUploadResponse.Content.ReadAsStringAsync(cancellationToken);

                    if (url == "www/index.html")
                    {
                        logger.Log("***");
                        logger.Log($"check content of {url} ");
                        logger.Log("***");
                        TestContent.TestIndexHtmlOgs(prepareForUploadContent, baseUrl, expectedUrl, folderName, ogImage, ogTitle,
                            (match, expected, wrongMsg, notFoundMsg, foundMsg) =>
                            {
                                if (!match.Success)
                                    logger.Log(notFoundMsg);
                                else if (match.Groups[1].Value != expected)
                                    logger.Log(wrongMsg);
                                else
                                    logger.Log(foundMsg);
                            });
                    }

                    if (url == "www/joomlaPreview.html")
                    {
                        logger.Log("***");
                        logger.Log($"check content of {url} ");
                        logger.Log("***");

                        TestContent.TestJoomlaPreviewHtml(prepareForUploadContent, expectedUrl, folderName, ogImage, ogTitle,
                            (match, expected, wrongMsg, notFoundMsg, foundMsg, index) =>
                            {
                                if (match.Success)
                                {
                                    string regExValue = match.Groups[index].Value;
                                    logger.Log(regExValue == expected ? foundMsg : wrongMsg);
                                }
                                else
                                    logger.Log(notFoundMsg);
                            });
                    }

                    if (url == "www/script/pics2maps.js")
                    {
                        logger.Log("***");
                        logger.Log($"check content of {url} ");
                        logger.Log("***");

                        TestContent.TestPics2mapsJs(prepareForUploadContent, folderName, (match, expected, wrongMsg, notFoundMsg, foundMsg, index) =>
                        {
                            if (match.Success)
                            {
                                string regExValue = match.Groups[index].Value;
                                logger.Log(regExValue == expected ? foundMsg : wrongMsg);
                            }
                            else
                                logger.Log(notFoundMsg);
                        });
                    }

                    if (url == "www/script/thumbnails.js")
                    {
                        logger.Log("***");
                        logger.Log($"check content of {url} ");
                        logger.Log("***");

                        TestContent.TestThumbnails(prepareForUploadContent, folderName, (match, expected, wrongMsg, notFoundMsg, foundMsg) =>
                        {
                            if (match.Success)
                            {
                                string regExValue = match.Groups[1].Value;
                                logger.Log(regExValue == expected ? foundMsg : wrongMsg);
                            }
                            else
                                logger.Log(notFoundMsg);
                        });
                    }
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