using System.Globalization;

namespace FileHandling;

public class GpsCommand(string latitude, string longitude)
{
    public double Longitude => ConvertToDouble(longitude);
    public double Latitude => ConvertToDouble(latitude);

    private double ConvertToDouble(string value)
    {
        return Convert.ToDouble(value.Trim()
            , new NumberFormatInfo { NumberDecimalSeparator = "." });
    }
}