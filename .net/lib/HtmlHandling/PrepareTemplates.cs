using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace HtmlHandling;

public class PrepareTemplates(ReplaceInFilesObject replaceInFilesObject) : ICommandHandler<PrepareTemplatesCommand>
{
    public void Execute(PrepareTemplatesCommand command)
    {
        command.ListOfSavedFiles = ReplaceKeysInTemplateFilesWithProperValues(command.ListOfFilesToReplaceJson
            , command.ListOfKeyValuesToReplaceInFilesJson
            , command.TemplatesFolder
            , command.SaveToPath);
    }

    private ConcurrentBag<string>? ReplaceKeysInTemplateFilesWithProperValues(string listOfFilesToReplaceJson
        , string listOfKeyValuesToReplaceInFilesJson
        , string templatesFolder
        , string saveToPath)
    {
        ConcurrentBag<string> listOfSavedFiles =
        [
            $"Loading from: {Path.GetFullPath(listOfFilesToReplaceJson)}"
        ];
        List<string>? listOfFilesToReplace = LoadJsonFileAndConvertToList(listOfFilesToReplaceJson);
        JObject listOfKeyValuesToReplaceInFilesObject =
            LoadJsonFileAndConvertToObject(listOfKeyValuesToReplaceInFilesJson);

        if (listOfFilesToReplace is null) return null;
        Parallel.ForEach(listOfFilesToReplace, fileToReplace =>
        {
            string fileToReplaceInFolder = Path.Join(templatesFolder, fileToReplace);
            if (File.Exists(fileToReplaceInFolder))
            {
                var command = new ReplaceInFilesObjectCommand
                {
                    ListOfFilesToReplaceJson = listOfFilesToReplaceJson,
                    ListOfKeyValuesToReplaceInFilesObject = listOfKeyValuesToReplaceInFilesObject,
                    TemplatesFolder = templatesFolder,
                    SaveToPath = saveToPath,
                    FileToReplaceInFolder = fileToReplaceInFolder,
                    FileToReplace = fileToReplace
                };
                replaceInFilesObject.Execute(command);
                listOfSavedFiles.Add(command.SavedFile);
            }
        });

        return listOfSavedFiles;
    }

    private static List<string>? LoadJsonFileAndConvertToList(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new Exception($"File: {Path.GetFullPath(fileName)} does not exist!");
        }

        string jsonFileContent = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<string>>(jsonFileContent);
    }

    private static JObject LoadJsonFileAndConvertToObject(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new Exception($"File: {Path.GetFullPath(fileName)} does not exist!");
        }

        string jsonFileContent = File.ReadAllText(fileName);
        return JObject.Parse(jsonFileContent);
    }
}