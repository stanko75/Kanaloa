using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace HtmlHandling;

public class PrepareTemplates //: IPrepareTemplates
{
    public ConcurrentBag<string>? ReplaceKeysInTemplateFilesWithProperValues(string listOfFilesToReplaceJson
        , string listOfKeyValuesToReplaceInFilesJson
        , string templatesFolder
        , string saveToPath)
    {
        ConcurrentBag<string> listOfSavedFiles = new ConcurrentBag<string>();
        listOfSavedFiles.Add($"Loading from: {Path.GetFullPath(listOfFilesToReplaceJson)}");
        List<string>? listOfFilesToReplace = LoadJsonFileAndConvertToList(listOfFilesToReplaceJson);
        JObject listOfKeyValuesToReplaceInFilesObject =
            LoadJsonFileAndConvertToObject(listOfKeyValuesToReplaceInFilesJson);

        if (listOfFilesToReplace is null) return null;
        Parallel.ForEach(listOfFilesToReplace, fileToReplace =>
        {
            string fileToReplaceInFolder = Path.Join(templatesFolder, fileToReplace);
            if (File.Exists(fileToReplaceInFolder))
            {
                string fileContent = File.ReadAllText(fileToReplaceInFolder);
                foreach (KeyValuePair<string, JToken?> keyValuesToReplaceInFiles in
                         listOfKeyValuesToReplaceInFilesObject)
                {
                    if (keyValuesToReplaceInFiles.Value is not null)
                    {
                        fileContent = fileContent.Replace(keyValuesToReplaceInFiles.Key,
                            keyValuesToReplaceInFiles.Value.Value<string>());

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
                listOfSavedFiles.Add(Path.GetFullPath(saveToFileNameWithPath));
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