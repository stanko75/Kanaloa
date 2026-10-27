namespace FunctionalTest;

public class PostGpsPositionsFromFilesWithFileNameCommand
{
    public string AddressText { get; set; }
    public string GpsLocationsPath { get; set; }
    public string FolderName { get; set; }
    public string GeospatialDataFileName { get; set; }
    public string GeospatialDataFileNameExtension { get; set; }
    public HttpClient HttpClientPost { get; set; }
}