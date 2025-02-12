namespace HtmlHandling;

public interface IPrepareTemplates
{
    public List<string>? ReplaceKeysInTemplateFilesWithProperValues(string listOfFilesToReplaceJson
        , string listOfKeyValuesToReplaceInFilesJson
        , string templatesFolder
        , string saveToPath);
}