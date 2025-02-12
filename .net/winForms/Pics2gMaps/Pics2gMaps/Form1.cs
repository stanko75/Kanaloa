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

        PrepareTemplates prepareTemplates = new PrepareTemplates();
        try
        {
            foreach (string savedFile in prepareTemplates.ReplaceKeysInTemplateFilesWithProperValues(listOfFilesToReplaceJson
                         , listOfKeyValuesToReplaceInFilesJson
                         , templateRootFolder
                         , saveToPath))
            {
                tbLog.AppendText(savedFile);
                tbLog.AppendText(Environment.NewLine);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }
}