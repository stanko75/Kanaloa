using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace HtmlHandling;

public class ReplaceInFilesObjectCommand
{
    public string ListOfFilesToReplaceJson { get; set; }
    public string ListOfKeyValuesToReplaceInFilesJson { get; set; }
    public string TemplatesFolder { get; set; }
    public string SaveToPath { get; set; }
    public string FileToReplaceInFolder { get; set; }
    public JObject ListOfKeyValuesToReplaceInFilesObject { get; set; }
    public string FileToReplace { get; set; }
    public string SavedFile { get; set; }
}