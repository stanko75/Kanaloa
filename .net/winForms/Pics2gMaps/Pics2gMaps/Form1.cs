using System.Data;
using System.Xml.Linq;
using Common;
using HtmlHandling;
using ImageHandling;
using Newtonsoft.Json;

namespace Pics2gMaps;

public partial class Form1 : Form
{
    private readonly DataTable _dtGalleryConfiguration = new();

    public Form1()
    {
        InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        string folderName = @"C:\projects\KanaloaGalleryTest\mariaLaach\";
        string jsonThumbsFileName = @"C:\projects\KanaloaGalleryTest\mariaLaach\www\mariaLaachThumbs.json";
        string jsonPicsFileName = @"C:\projects\KanaloaGalleryTest\mariaLaach\www\mariaLaach.json";

        if (File.Exists(jsonThumbsFileName))
        {
            File.Delete(jsonThumbsFileName);
        }

        if (File.Exists(jsonPicsFileName))
        {
            File.Delete(jsonPicsFileName);
        }

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
                    FolderName = string.Empty,
                    LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                    ImageFileName = Path.GetFileName(imageFileName),
                    JsonThumbsFileName = @"C:\projects\KanaloaGalleryTest\mariaLaach\www\mariaLaachThumbs.json",
                    JsonPicsFileName = @"C:\projects\KanaloaGalleryTest\mariaLaach\www\mariaLaach.json"
                };

                UpdateOrCreateJsonFileWithListOfImages updateOrCreateJsonFileWithListOfImages =
                    new UpdateOrCreateJsonFileWithListOfImages(new UpdateJsonIfExistsOrCreateNewIfNot());
                updateOrCreateJsonFileWithListOfImages.Execute(updateOrCreateJsonFileWithListOfImagesCommand);
            }
            catch (Exception ex)
            {
                tbLog.AppendText(ex.Message);
                tbLog.AppendText(Environment.NewLine);
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
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.GalleryName);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.RootGalleryFolder);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.WebPath);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.Gapikey);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgTitle);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgDescription);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgImage);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgUrl);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.PicsJson);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.Zoom);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.ResizeImages);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.JoomlaThumbsPath);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.JoomlaImgSrcPath);

        dgvGalleryConfiguration.DataSource = _dtGalleryConfiguration;

        XDocument xmlDoc = XDocument.Load(@"C:\projects\Kanaloa\.net\winForms\Pics2gMaps\Pics2gMaps\app.config");
        var galleries = xmlDoc.Descendants("galleries")
            .Descendants("settings")
            .Descendants("add")
            .Select(gallery => new
            {
                GalleryName = (string)gallery.Attribute(DataTableConfigColumns.GalleryName),
                RootGalleryFolder = (string)gallery.Attribute(DataTableConfigColumns.RootGalleryFolder),
                WebPath = (string)gallery.Attribute(DataTableConfigColumns.WebPath),
                OgTitle = (string)gallery.Attribute(DataTableConfigColumns.OgTitle),
                OgDescription = (string)gallery.Attribute(DataTableConfigColumns.OgDescription),
                OgImage = (string)gallery.Attribute(DataTableConfigColumns.OgImage),
                Zoom = (int?)gallery.Attribute(DataTableConfigColumns.Zoom) ?? 0,
                ResizeImages = (bool?)gallery.Attribute(DataTableConfigColumns.ResizeImages) ?? false,
                JoomlaThumbsPath = (string)gallery.Attribute(DataTableConfigColumns.JoomlaThumbsPath),
                JoomlaImgSrcPath = (string)gallery.Attribute(DataTableConfigColumns.JoomlaImgSrcPath)
            })
            .ToList();

        if (!(galleries is null))
        {
            foreach (var setting in galleries)
            {
                DataRow drGalleryConfiguration = _dtGalleryConfiguration.NewRow();
                drGalleryConfiguration[DataTableConfigColumns.GalleryName] = setting.GalleryName;
                drGalleryConfiguration[DataTableConfigColumns.RootGalleryFolder] = setting.RootGalleryFolder;
                drGalleryConfiguration[DataTableConfigColumns.WebPath] = string.IsNullOrWhiteSpace(setting.WebPath) ? "http://www.milosev.com/gallery/allWithPics/travelBuddies/" : setting.WebPath;
                //drGalleryConfiguration["gapikey"] = setting.
                drGalleryConfiguration[DataTableConfigColumns.OgTitle] = setting.OgTitle;
                drGalleryConfiguration[DataTableConfigColumns.OgDescription] = setting.OgDescription;
                drGalleryConfiguration[DataTableConfigColumns.OgImage] = setting.OgImage;
                drGalleryConfiguration[DataTableConfigColumns.OgUrl] =
                    $"{setting.WebPath}{setting.GalleryName}/www/index.html";
                //drGalleryConfiguration["picsJson"] = setting.P;
                drGalleryConfiguration[DataTableConfigColumns.Zoom] = setting.Zoom;
                drGalleryConfiguration[DataTableConfigColumns.ResizeImages] = setting.ResizeImages;
                drGalleryConfiguration[DataTableConfigColumns.JoomlaThumbsPath] = setting.JoomlaThumbsPath;
                drGalleryConfiguration[DataTableConfigColumns.JoomlaImgSrcPath] = setting.JoomlaImgSrcPath;
                _dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
            }
        }
    }

    private void btnSaveConfig_Click(object sender, EventArgs e)
    {
        List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();
        foreach (DataRow row in _dtGalleryConfiguration.Rows)
        {
            var jsonObj = new Dictionary<string, string>
            {
                { "/*galleryName*/", row["GalleryName"].ToString() },
                { "/*gapikey*/", row["Gapikey"].ToString() },
                { "/*ogTitle*/", row["OgTitle"].ToString() },
                { "/*ogDescription*/", row["OgDescription"].ToString() },
                { "/*ogImage*/", row["OgImage"].ToString() },
                { "/*ogUrl*/", row["OgUrl"].ToString() },
                { "/*picsJson*/", row["PicsJson"].ToString() },
                { "/*zoom*/", row["Zoom"].ToString() },
                { "/*joomlaThumbsPath*/", row["JoomlaThumbsPath"].ToString() },
                { "/*joomlaImgSrcPath*/", row["JoomlaImgSrcPath"].ToString() }
            };

            jsonList.Add(jsonObj);
        }
        string jsonString = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
        File.WriteAllText("gallery_config.json", jsonString);
    }
}