namespace HtmlHandling;

public class ReplaceKeysInFilesCommand
{
    public IEnumerable<string> ListOfFilesToReplace { get; set; }
    public Dictionary<string, string> ListOfKeyValuesToReplaceInFiles { get; set; }
    public string TemplateRootFolder { get; set; }
    public string SaveToPath { get; set; }
}