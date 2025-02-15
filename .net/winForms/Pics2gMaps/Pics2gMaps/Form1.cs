using System.Data;
using System.Xml.Linq;
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
        string templateRootFolder = tbTemplateRootFolder.Text;
        string saveToPath = tbSaveToPath.Text;

        string listOfFilesToReplaceJson = Path.Join(templateRootFolder, "listOfFilesToReplaceAndCopy.json");
        string listOfKeyValuesToReplaceInFilesJson = Path.Join(templateRootFolder, tbJsonFile.Text);

        try
        {
            ReplaceInFilesObject replaceInFilesObject = new ReplaceInFilesObject();
            PrepareTemplatesCommand prepareTemplatesCommand = new PrepareTemplatesCommand
            {
                ListOfFilesToReplaceJson = listOfFilesToReplaceJson,
                ListOfKeyValuesToReplaceInFilesJson = listOfKeyValuesToReplaceInFilesJson,
                SaveToPath = saveToPath,
                TemplatesFolder = templateRootFolder
            };
            PrepareTemplates prepareTemplates = new PrepareTemplates(replaceInFilesObject);
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

    private void btnLoadOld_Click(object sender, EventArgs e)
    {
        DataTable dtGalleryConfiguration = new DataTable();

        dtGalleryConfiguration.Columns.Add("galleryName");
        dtGalleryConfiguration.Columns.Add("rootGalleryFolder");
        dtGalleryConfiguration.Columns.Add("webPath");
        dtGalleryConfiguration.Columns.Add("gapikey");
        dtGalleryConfiguration.Columns.Add("ogTitle");
        dtGalleryConfiguration.Columns.Add("ogDescription");
        dtGalleryConfiguration.Columns.Add("ogImage");
        dtGalleryConfiguration.Columns.Add("ogUrl");
        dtGalleryConfiguration.Columns.Add("picsJson");
        dtGalleryConfiguration.Columns.Add("zoom");
        dtGalleryConfiguration.Columns.Add("resizeImages");
        dtGalleryConfiguration.Columns.Add("joomlaThumbsPath");
        dtGalleryConfiguration.Columns.Add("joomlaImgSrcPath");

        dgvGalleryConfiguration.DataSource = dtGalleryConfiguration;

        XDocument xmlDoc = XDocument.Load(@"C:\projects\Kanaloa\.net\winForms\Pics2gMaps\Pics2gMaps\app.config");
        var galleries = xmlDoc.Descendants("galleries")
            .Descendants("settings")
            .Descendants("add")
            .Select(gallery => new
            {
                GalleryName = (string)gallery.Attribute("galleryName"),
                RootGalleryFolder = (string)gallery.Attribute("rootGalleryFolder"),
                WebPath = (string)gallery.Attribute("webPath"),
                OgTitle = (string)gallery.Attribute("ogTitle"),
                OgDescription = (string)gallery.Attribute("ogDescription"),
                OgImage = (string)gallery.Attribute("ogImage"),
                Zoom = (int?)gallery.Attribute("zoom") ?? 0,
                ResizeImages = (bool?)gallery.Attribute("resizeImages") ?? false,
                JoomlaThumbsPath = (string)gallery.Attribute("joomlaThumbsPath"),
                JoomlaImgSrcPath = (string)gallery.Attribute("joomlaImgSrcPath")
            })
            .ToList();

        if (!(galleries is null))
        {
            foreach (var setting in galleries)
            {
                DataRow drGalleryConfiguration = dtGalleryConfiguration.NewRow();
                drGalleryConfiguration["galleryName"] = setting.GalleryName;
                drGalleryConfiguration["rootGalleryFolder"] = setting.RootGalleryFolder;
                drGalleryConfiguration["webPath"] = setting.WebPath;
                //drGalleryConfiguration["gapikey"] = setting.
                drGalleryConfiguration["ogTitle"] = setting.OgTitle;
                drGalleryConfiguration["ogDescription"] = setting.OgDescription;
                drGalleryConfiguration["ogImage"] = setting.OgImage;
                drGalleryConfiguration["ogUrl"] = $"{setting.WebPath}{setting.GalleryName}/www/index.html";
                //drGalleryConfiguration["picsJson"] = setting.P;
                drGalleryConfiguration["zoom"] = setting.Zoom;
                drGalleryConfiguration["resizeImages"] = setting.ResizeImages;
                drGalleryConfiguration["joomlaThumbsPath"] = setting.JoomlaThumbsPath;
                drGalleryConfiguration["joomlaImgSrcPath"] = setting.JoomlaImgSrcPath;
                dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
            }
        }
    }
}