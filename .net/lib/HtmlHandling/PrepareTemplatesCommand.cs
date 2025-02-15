using System.Collections.Concurrent;

namespace HtmlHandling;

public class PrepareTemplatesCommand
{
    public string TemplatesFolder { get; set; }
    public string ListOfFilesToReplaceJson { get; set; }
    public string ListOfKeyValuesToReplaceInFilesJson { get; set; }
    public string SaveToPath { get; set; }
    public ConcurrentBag<string>? ListOfSavedFiles { get; set; }
}