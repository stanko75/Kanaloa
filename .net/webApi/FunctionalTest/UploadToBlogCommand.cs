namespace FunctionalTest;

public class UploadToBlogCommand
{
    public string KmlFileName { get; set; }
    public string FolderName { get; set; }
    public string AddressText { get; set; }
    public string FtpHost { get; set; }
    public string FtpUser { get; set; }
    public string FtpPass { get; set; }
    public HttpClient HttpClientPost { get; set; }
    public string OgTitle { get; set; }
    public string OgImage { get; set; }
    public string BaseUrl { get; set; }
    public string ExpectedUrl { get; set; }
    public string PrepareForUpload { get; set; }
    public string DeleteFirstKmlPoints { get; set; }
    public string DeleteLastKmlPoints { get; set; }
    public string JoomlaCategoryId { get; set; }
    public string JoomlaLoginUrl { get; set; }
    public string JoomlaPostUrl { get; set; }
    public string JoomlaUserName { get; set; }
    public string JoomlaPass { get; set; }
}