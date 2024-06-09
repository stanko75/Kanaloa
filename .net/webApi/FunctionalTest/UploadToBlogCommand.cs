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
}