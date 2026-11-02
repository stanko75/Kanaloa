using Common;

namespace HtmlHandling;

public class ReplaceKeysInFiles: ICommandHandler<ReplaceKeysInFilesCommand>
{
    public void Execute(ReplaceKeysInFilesCommand replaceKeysInFilesCommand)
    {
        foreach (string fileName in replaceKeysInFilesCommand.ListOfFilesToReplace)
        {
            string replaceFileName = fileName.Replace("\\", "/");

            string fullFileName = string.Join("/", replaceKeysInFilesCommand.TemplateRootFolder, replaceFileName);

            if (!File.Exists(fullFileName))
            {
                throw new FileNotFoundException($"File: {Path.GetFullPath(fullFileName)} not found!");
            }

            string fileContent = File.ReadAllText(fullFileName);

            foreach (KeyValuePair<string, string?> keyValuesToReplaceInFiles in
                     replaceKeysInFilesCommand.ListOfKeyValuesToReplaceInFiles)
            {
                fileContent = fileContent.Replace(keyValuesToReplaceInFiles.Key, keyValuesToReplaceInFiles.Value);
            }

            string saveToFullFileName = string.Join("/", replaceKeysInFilesCommand.SaveToPath, replaceFileName);
            string saveToPath = Path.GetDirectoryName(saveToFullFileName);
            if (!Directory.Exists(saveToPath))
            {
                Directory.CreateDirectory(saveToPath);
            }
            File.WriteAllText(saveToFullFileName, fileContent);
        }
    }
}