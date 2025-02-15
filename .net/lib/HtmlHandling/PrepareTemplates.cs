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
        /*
        JObject listOfKeyValuesToReplaceInFilesObject =
            LoadJsonFileAndConvertToObject(listOfKeyValuesToReplaceInFilesJson);
        */

        if (listOfFilesToReplace is null) return null;
        Parallel.ForEach(listOfFilesToReplace, fileToReplace =>
        {
            string fileToReplaceInFolder = Path.Join(templatesFolder, fileToReplace);
            if (File.Exists(fileToReplaceInFolder))
            {
                listOfSavedFiles.Add(ReplaceInFiles(listOfKeyValuesToReplaceInFilesJson
                    , listOfFilesToReplaceJson
                    , templatesFolder
                    , saveToPath
                    , fileToReplaceInFolder
                    , fileToReplace));
            }
        });

        return listOfSavedFiles;
    }

    private string ReplaceInFiles(string fileName
        , string listOfFilesToReplaceJson
        , string templatesFolder
        , string saveToPath
        , string fileToReplaceInFolder
        , string fileToReplace)
    {
        string jsonFileContent = File.ReadAllText(fileName);
        JToken jToken = JToken.Parse(jsonFileContent);

        if (jToken.Type == JTokenType.Array)
        {
            Console.WriteLine("Array"); //Further Manipulations
            return null;
        }

        JObject? listOfKeyValuesToReplaceInFilesObject = jToken.ToObject<JObject>();
        return ReplaceInFilesObjectCommand(listOfFilesToReplaceJson
            , listOfKeyValuesToReplaceInFilesObject
            , templatesFolder
            , saveToPath
            , fileToReplaceInFolder
            , fileToReplace);
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

    private string ReplaceInFilesObjectCommand(string listOfFilesToReplaceJson
        , JObject? listOfKeyValuesToReplaceInFilesObject
        , string templatesFolder
        , string saveToPath
        , string fileToReplaceInFolder
        , string fileToReplace)
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
        return command.SavedFile;
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