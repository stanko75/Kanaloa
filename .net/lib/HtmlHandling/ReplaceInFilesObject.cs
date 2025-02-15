using System.Collections.Concurrent;
using Common;
using Newtonsoft.Json.Linq;

namespace HtmlHandling;

public class ReplaceInFilesObject: ICommandHandler<ReplaceInFilesObjectCommand>
{
    public void Execute(ReplaceInFilesObjectCommand command)
    {
        command.SavedFile = ReplaceKeysInTemplateFilesWithProperValues(command.ListOfFilesToReplaceJson
            , command.ListOfKeyValuesToReplaceInFilesJson
            , command.TemplatesFolder
            , command.SaveToPath
            , command.FileToReplaceInFolder
            , command.ListOfKeyValuesToReplaceInFilesObject
            , command.FileToReplace);
    }

    public string ReplaceKeysInTemplateFilesWithProperValues(string listOfFilesToReplaceJson
        , string listOfKeyValuesToReplaceInFilesJson
        , string templatesFolder
        , string saveToPath
        , string fileToReplaceInFolder
        , JObject listOfKeyValuesToReplaceInFilesObject
        , string fileToReplace)
    {
        string fileContent = File.ReadAllText(fileToReplaceInFolder);
        foreach (KeyValuePair<string, JToken?> keyValuesToReplaceInFiles in
                 listOfKeyValuesToReplaceInFilesObject)
        {
            if (keyValuesToReplaceInFiles.Value is not null)
            {
                fileContent = fileContent.Replace(keyValuesToReplaceInFiles.Key,
                    keyValuesToReplaceInFiles.Value.Value<string>());

                if (keyValuesToReplaceInFiles.Key == "/*galleryName*/")
                {
                    saveToPath = Path.Join(saveToPath, keyValuesToReplaceInFiles.Value.Value<string>());
                    saveToPath = Path.Join(saveToPath, "www");
                }

                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }

                string fileToReplaceDir = Path.GetDirectoryName(fileToReplace) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(fileToReplaceDir))
                {
                    fileToReplaceDir = Path.Join(saveToPath, fileToReplaceDir);
                    if (!Directory.Exists(fileToReplaceDir))
                    {
                        Directory.CreateDirectory(fileToReplaceDir);
                    }
                }
            }
        }

        string saveToFileNameWithPath = Path.Join(saveToPath, fileToReplace);
        File.WriteAllText(saveToFileNameWithPath, fileContent);
        return Path.GetFullPath(saveToFileNameWithPath);
    }
}