using HtmlHandling;

namespace Pics2gMaps;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        string templateRootFolder = @"..\..\..\..\..\..\..\html\templateForBlog";
        string saveToPath = @"html\blog\www";

        string listOfFilesToReplaceJson = Path.Join(templateRootFolder, "listOfFilesToReplaceAndCopy.json");
        string listOfKeyValuesToReplaceInFilesJson = Path.Join(templateRootFolder, "listOfKeyValuesToReplaceInFiles.json");

        try
        {
            ReplaceInFilesObject replaceInFilesObject = new ReplaceInFilesObject();

            PrepareTemplatesCommand prepareTemplatesCommand = new PrepareTemplatesCommand();


            PrepareTemplates prepareTemplates = new PrepareTemplates(replaceInFilesObject);
            prepareTemplatesCommand.ListOfFilesToReplaceJson = listOfFilesToReplaceJson;
            prepareTemplatesCommand.ListOfKeyValuesToReplaceInFilesJson = listOfKeyValuesToReplaceInFilesJson;
            prepareTemplatesCommand.SaveToPath = saveToPath;
            prepareTemplatesCommand.TemplatesFolder = templateRootFolder;

            prepareTemplates.Execute(prepareTemplatesCommand);

            prepareTemplates.Execute(prepareTemplatesCommand);

            if (prepareTemplatesCommand.ListOfSavedFiles is not null)
            {
                foreach (string savedFile in prepareTemplatesCommand.ListOfSavedFiles)
                {
                    tbLog.AppendText(savedFile);
                    tbLog.AppendText(Environment.NewLine);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }
}