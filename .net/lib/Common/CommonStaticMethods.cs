using Newtonsoft.Json.Linq;

namespace Common;

public static class CommonStaticMethods
{
    public static string GetValue(JObject? data, string value)
    {
        return (data?[value] ?? string.Empty).Value<string>() ?? string.Empty;
    }
}