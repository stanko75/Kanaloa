using ImageHandling;

namespace Pics2gMaps;

public class GpsInfoFromImageExtractedEventArgs(LatLngFileNameModel latLngFileName)
{
    public LatLngFileNameModel LatLngFileName { get; } = latLngFileName;
}