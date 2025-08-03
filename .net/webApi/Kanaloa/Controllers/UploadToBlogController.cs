using Common;
using FileHandling;
using FtpHandling;
using HtmlHandling;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using Kanaloa.Controllers.uploadToBlog;

namespace Kanaloa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadToBlogController : ControllerBase
{
    [HttpPost]
    [Route("UploadToBlog")]
    public IActionResult UploadToBlog([FromBody] JObject data)
    {
        try
        {
            string folder = CommonStaticMethods.GetValue(data, "folderName");
            folder = string.IsNullOrWhiteSpace(folder) ? "default" : folder;

            string kmlFileName = CommonStaticMethods.GetValue(data, "kmlFileName");
            kmlFileName = string.IsNullOrWhiteSpace(kmlFileName) ? "default" : kmlFileName;

            string host = CommonStaticMethods.GetValue(data, "host");
            string user = CommonStaticMethods.GetValue(data, "user");
            string pass = CommonStaticMethods.GetValue(data, "pass");

            string remoteRootFolder = "/allWithPics/travelBuddies";

            CopyHtmlFilesCommand copyHtmlFilesCommand = new CopyHtmlFilesCommand();
            copyHtmlFilesCommand.HtmlTemplateFolderWithRelativePath = @"html\templateForBlog";
            copyHtmlFilesCommand.KmlFileName = Path.Join(folder, kmlFileName);
            copyHtmlFilesCommand.NameOfAlbum = folder;
            copyHtmlFilesCommand.PrepareForUploadFolder = "prepareForUpload";

            CopyHtmlFiles copyHtmlFiles = new CopyHtmlFiles();
            copyHtmlFiles.Execute(copyHtmlFilesCommand);

            ReplaceKeysInFilesCommand replaceKeysInFilesCommand = new ReplaceKeysInFilesCommand();

            string listOfFilesToReplaceAndCopyFileName =
                Path.Join(copyHtmlFilesCommand.HtmlTemplateFolderWithRelativePath, "listOfFilesToReplaceAndCopy.json");
            replaceKeysInFilesCommand.ListOfFilesToReplace =
                JsonConvert.DeserializeObject<IEnumerable<string>>(
                    System.IO.File.ReadAllText(listOfFilesToReplaceAndCopyFileName)) ??
                throw new InvalidOperationException();

            var listOfKeyValuesToReplaceInFiles = new Dictionary<string, string>();
            FillListOfKeyValuesToReplaceInFiles(listOfKeyValuesToReplaceInFiles, data);
            listOfKeyValuesToReplaceInFiles["galleryName"] = folder;
            listOfKeyValuesToReplaceInFiles["rootGalleryFolder"] = copyHtmlFilesCommand.PrepareForUploadFolder;

            AutomaticallyFillMissingValuesCommand automaticallyFillMissingValuesCommand =
                new AutomaticallyFillMissingValuesCommand();
            FillProperties(automaticallyFillMissingValuesCommand, listOfKeyValuesToReplaceInFiles);
            AutomaticallyFillMissingValues automaticallyFillMissingValues = new AutomaticallyFillMissingValues();
            automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);

            FillDictionary(automaticallyFillMissingValuesCommand, listOfKeyValuesToReplaceInFiles);

            var keys = listOfKeyValuesToReplaceInFiles.Keys.ToList();

            foreach (var oldKey in keys)
            {
                var value = listOfKeyValuesToReplaceInFiles[oldKey];
                listOfKeyValuesToReplaceInFiles.Remove(oldKey);
                listOfKeyValuesToReplaceInFiles[$"/*{oldKey}*/"] = value;
            }

            replaceKeysInFilesCommand.ListOfKeyValuesToReplaceInFiles = listOfKeyValuesToReplaceInFiles;

            replaceKeysInFilesCommand.TemplateRootFolder = Path.Join(copyHtmlFilesCommand.PrepareForUploadFolder, folder);
            replaceKeysInFilesCommand.TemplateRootFolder = Path.Join(replaceKeysInFilesCommand.TemplateRootFolder, "www");
            replaceKeysInFilesCommand.SaveToPath = replaceKeysInFilesCommand.TemplateRootFolder;

            ReplaceKeysInFiles replaceKeysInFiles = new ReplaceKeysInFiles();
            replaceKeysInFiles.Execute(replaceKeysInFilesCommand);

            return Ok(@$"Uploaded: {remoteRootFolder}/{folder}/{kmlFileName}");
        }
        catch (Exception e)
        {
            return BadRequest($"Exception message: {e.Message}, inner exception: {e.InnerException}");
        }
    }

    private void FillDictionary(AutomaticallyFillMissingValuesCommand automaticallyFillMissingValuesCommand, Dictionary<string, string> listOfKeyValuesToReplaceInFiles)
    {
        foreach (var prop in automaticallyFillMissingValuesCommand.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanRead)
            {
                var value = prop.GetValue(automaticallyFillMissingValuesCommand);

                string key = FirstCharToLower(prop.Name);
                if (listOfKeyValuesToReplaceInFiles.ContainsKey(key))
                {
                    listOfKeyValuesToReplaceInFiles[key] = value?.ToString() ?? string.Empty;
                }
                else
                {
                    throw new InvalidOperationException($"Property {key} not found in the dictionary.");
                }
            }
        }

        //return dict;
    }

    public static T? GetValue<T>(object source, string columnName)
    {
        return source switch
        {
            DataRow row => (T?)row[columnName],
            JObject jObject => jObject[columnName] is not null
                ? jObject[columnName]!.ToObject<T>()
                : default,
            _ => throw new ArgumentException("Unsupported object type.")
        };
    }

    private void FillListOfKeyValuesToReplaceInFiles(Dictionary<string, string> listOfKeyValuesToReplaceInFiles,
        JObject data)
    {
        FieldInfo[] fields = typeof(DataTableConfigColumns)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo field in fields)
        {
            string columnName = field.GetValue(null)?.ToString() ?? throw new InvalidOperationException();
            listOfKeyValuesToReplaceInFiles.Add($"{FirstCharToLower(columnName)}", GetValue<string>(data, columnName) ?? string.Empty);
        }
    }

    private List<string> GetListOfFields_old()
    {
        FieldInfo[] fields = typeof(DataTableConfigColumns)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        List<string> columnNames = new List<string>();
        foreach (FieldInfo field in fields)
        {
            string columnName = field.GetValue(null)?.ToString() ?? throw new InvalidOperationException();
            columnNames.Add(columnName);
        }

        return columnNames;
    }

    private IEnumerable<string> GetListOfFields()
    {
        FieldInfo[] fields = typeof(DataTableConfigColumns)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo field in fields)
        {
            yield return field.GetValue(null)?.ToString() ?? throw new InvalidOperationException();
        }
    }

    private static void FillProperties(object target, Dictionary<string, string> listOfKeyValuesToReplaceInFiles)
    {
        IEnumerable<PropertyInfo> properties = target.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        foreach (PropertyInfo prop in properties)
        {
            //string field = prop.Name.Substring(0, 1).ToLower() + prop.Name.Substring(1, prop.Name.Length - 1);
            string field = FirstCharToLower(prop.Name);
            Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (listOfKeyValuesToReplaceInFiles.ContainsKey(field))
            {
                object? value = type switch
                {
                    { } t when t == typeof(string) => listOfKeyValuesToReplaceInFiles[field],
                    { } t when t == typeof(int) => string.IsNullOrWhiteSpace(listOfKeyValuesToReplaceInFiles[field])
                        ? 0
                        : int.Parse(listOfKeyValuesToReplaceInFiles[field]),
                    { } t when t == typeof(bool) =>
                        !string.IsNullOrWhiteSpace(listOfKeyValuesToReplaceInFiles[field]) &&
                        bool.Parse(listOfKeyValuesToReplaceInFiles[field]),
                    _ => null
                };

                prop.SetValue(target, value);
            }
        }
    }

    private static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

}