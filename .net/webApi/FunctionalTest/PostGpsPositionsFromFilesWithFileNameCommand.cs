namespace FunctionalTest;

public class PostGpsPositionsFromFilesWithFileNameCommand
{
    public string AddressText { get; set; }
    public string GpsLocationsPath { get; set; }
    public string FolderName { get; set; }
    public string KmlFileName { get; set; }
    public HttpClient HttpClientPost { get; set; }
}