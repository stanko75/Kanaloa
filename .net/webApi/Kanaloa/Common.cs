using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kanaloa;

public class Common()
{
    public static string GetValue(JObject data, string value)
    {
        return (data[value] ?? string.Empty).Value<string>() ?? string.Empty;
    }
}