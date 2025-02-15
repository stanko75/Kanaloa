using System.Data;
using System.Xml.Linq;
using Common;
using HtmlHandling;
using ImageHandling;

namespace Pics2gMaps;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        string folderName = @"C:\projects\KanaloaGalleryTest\mariaLaach\";
        foreach (string imageFileName in Directory.GetFiles(Path.Join(folderName, "pics")))
        {
            try
            {
                var resizeImageCommand = new ResizeImageCommand
                {
                    CanvasHeight = 200,
                    CanvasWidth = 200,
                    OriginalFileName = Path.GetFileName(imageFileName),
                    //SaveTo = Path.Join(@"C:\projects\KanaloaGalleryTest\mariaLaach\thumbs", imageFileName)
                    SaveTo = Path.GetFileName(imageFileName)
                };
                resizeImageCommand.CreateDirectories(folderName);

                ResizeImage resizeImage = new ResizeImage();
                resizeImage.Execute(resizeImageCommand);

                ExtractGpsInfoFromImage extractGpsInfoFromImage = new ExtractGpsInfoFromImage();
                var extractGpsInfoFromImageCommand = new ExtractGpsInfoFromImageCommand
                {
                    ImageFileNameToReadGpsFrom = imageFileName
                };
                extractGpsInfoFromImage.Execute(extractGpsInfoFromImageCommand);

                var updateOrCreateJsonFileWithListOfImagesCommand = new UpdateOrCreateJsonFileWithListOfImagesCommand
                {
                    FolderName = folderName,
                    LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                    ImageFileName = imageFileName,
                    JsonFileName = @"C:\projects\KanaloaGalleryTest\mariaLaach\www\mariaLaachThumbs.json"
                };
                UpdateOrCreateJsonFileWithListOfImages updateOrCreateJsonFileWithListOfImages =
                    new UpdateOrCreateJsonFileWithListOfImages(new UpdateJsonIfExistsOrCreateNewIfNot());
                updateOrCreateJsonFileWithListOfImages.Execute(updateOrCreateJsonFileWithListOfImagesCommand);
            }
            catch (Exception ex)
            {
                tbLog.AppendText(ex.Message);
            }
        }

        PrepareHtmlFolder(tbTemplateRootFolder.Text, tbSaveToPath.Text, tbJsonFile.Text);
    }

    private void PrepareHtmlFolder(string templateRootFolder, string saveToPath,
        string listOfKeyValuesToReplaceInFilesJson)
    {
        string listOfFilesToReplaceJson = Path.Join(templateRootFolder, "listOfFilesToReplaceAndCopy.json");

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
            tbLog.AppendText(ex.Message);
            tbLog.AppendText(Environment.NewLine);
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