using Newtonsoft.Json.Linq;

namespace ImageHandling;

public class PrepareToResizeImageDecoratorCommand
{
    public JObject? Data { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
    public string ImageFileNameWithFullPath { get; set; } = string.Empty;
}