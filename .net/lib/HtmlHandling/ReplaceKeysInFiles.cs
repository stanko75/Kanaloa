using Common;

namespace HtmlHandling;

public class ReplaceKeysInFiles: ICommandHandler<ReplaceKeysInFilesCommand>
{
    public void Execute(ReplaceKeysInFilesCommand replaceKeysInFilesCommand)
    {
        foreach (string fileName in replaceKeysInFilesCommand.ListOfFilesToReplace)
        {
            string fullFileName = Path.Join(replaceKeysInFilesCommand.TemplateRootFolder, fileName);

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

            string saveToFullFileName = Path.Join(replaceKeysInFilesCommand.SaveToPath, fileName);
            string saveToPath = Path.GetDirectoryName(saveToFullFileName);
            if (!Directory.Exists(saveToPath))
            {
                Directory.CreateDirectory(saveToPath);
            }
            File.WriteAllText(saveToFullFileName, fileContent);
        }
    }
}