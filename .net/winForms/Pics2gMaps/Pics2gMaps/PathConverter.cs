namespace HtmlHandling;

public class PathConverter : IPathConverter
{
    public string? Execute(string webPath)
    {
        int index = webPath.IndexOf("/", StringComparison.Ordinal);
        if (index != -1)
        {
            return webPath.Substring(index);
        }

        return null;
    }
}