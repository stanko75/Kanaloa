using System.Globalization;

namespace FileHandling;

public class GpsCommand(string latitude, string longitude, string altitude)
{
    public double Longitude => ConvertToDouble(longitude);
    public double Latitude => ConvertToDouble(latitude);
    public double Altitude => ConvertToDouble(altitude);

    private double ConvertToDouble(string value)
    {
        return Convert.ToDouble(value.Trim()
            , new NumberFormatInfo { NumberDecimalSeparator = "." });
    }
}