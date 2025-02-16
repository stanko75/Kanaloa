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
        foreach (DataRow dataRow in _dtGalleryConfiguration.Rows)
        {
            string galleryName = dataRow[DataTableConfigColumns.GalleryName].ToString();

            string folderName = Path.Join(dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
            string jsonThumbsFileName = Path.Join(dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}Thumbs.json");
            string jsonPicsFileName = Path.Join(dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), $@"{galleryName}\www\{galleryName}.json");

            if (File.Exists(jsonThumbsFileName))
            {
                File.Delete(jsonThumbsFileName);
            }

            if (File.Exists(jsonPicsFileName))
            {
                File.Delete(jsonPicsFileName);
            }

            string picsFolder = Path.Join(folderName, "pics");
            if (Directory.Exists(picsFolder))
            {
                foreach (string imageFileName in Directory.GetFiles(picsFolder))
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

                        var updateOrCreateJsonFileWithListOfImagesCommand =
                            new UpdateOrCreateJsonFileWithListOfImagesCommand
                            {
                                FolderName = string.Empty,
                                LatLngModel = extractGpsInfoFromImageCommand.LatLngModel,
                                ImageFileName = Path.GetFileName(imageFileName),
                                JsonThumbsFileName = jsonThumbsFileName,
                                JsonPicsFileName = jsonPicsFileName
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
            }
            else
            {
                tbLog.AppendText($"Folder {picsFolder} does not exist!");
                tbLog.AppendText(Environment.NewLine);
            }

            PrepareHtmlFolder(tbTemplateRootFolder.Text, dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), tbJsonFile.Text);

        }
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
        if (_dtGalleryConfiguration.Columns.Count == 0)
        {
            AddColumnsToDt();
        }

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
                drGalleryConfiguration[DataTableConfigColumns.WebPath] = string.IsNullOrWhiteSpace(setting.WebPath)
                    ? "http://www.milosev.com/gallery/allWithPics/travelBuddies/"
                    : setting.WebPath;
                //drGalleryConfiguration["gapikey"] = setting.
                drGalleryConfiguration[DataTableConfigColumns.OgTitle] = setting.OgTitle;
                drGalleryConfiguration[DataTableConfigColumns.OgDescription] = setting.OgDescription;
                drGalleryConfiguration[DataTableConfigColumns.OgImage] = setting.OgImage;
                drGalleryConfiguration[DataTableConfigColumns.OgUrl] =
                    $"{setting.WebPath}{setting.GalleryName}/www/index.html";
                drGalleryConfiguration[DataTableConfigColumns.Zoom] = setting.Zoom;
                drGalleryConfiguration[DataTableConfigColumns.ResizeImages] = setting.ResizeImages;
                drGalleryConfiguration[DataTableConfigColumns.JoomlaThumbsPath] = setting.JoomlaThumbsPath;
                drGalleryConfiguration[DataTableConfigColumns.JoomlaImgSrcPath] = setting.JoomlaImgSrcPath;
                _dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
            }
        }
    }

    private void AddColumnsToDt()
    {
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.GalleryName);
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.RootGalleryFolder, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.WebPath, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.Gapikey, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgTitle, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgDescription, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgImage, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgUrl, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.PicsJson, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.Zoom, typeof(int));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.ResizeImages, typeof(bool));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.JoomlaThumbsPath, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.JoomlaImgSrcPath, typeof(string));
    }

    private void btnSaveConfig_Click(object sender, EventArgs e)
    {
        List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();
        foreach (DataRow row in _dtGalleryConfiguration.Rows)
        {
            var jsonObj = new Dictionary<string, string>
            {
                { "/*galleryName*/", row[DataTableConfigColumns.GalleryName].ToString() },
                { "/*rootGalleryFolder*/", row[DataTableConfigColumns.RootGalleryFolder].ToString() },
                { "/*webPath*/", row[DataTableConfigColumns.WebPath].ToString() },
                { "/*gapikey*/", row[DataTableConfigColumns.Gapikey].ToString() },
                { "/*ogTitle*/", row[DataTableConfigColumns.OgTitle].ToString() },
                { "/*ogDescription*/", row[DataTableConfigColumns.OgDescription].ToString() },
                { "/*ogImage*/", row[DataTableConfigColumns.OgImage].ToString() },
                { "/*ogUrl*/", row[DataTableConfigColumns.OgUrl].ToString() },
                { "/*picsJson*/", row[DataTableConfigColumns.PicsJson].ToString() },
                { "/*zoom*/", row[DataTableConfigColumns.Zoom].ToString() },
                { "/*resizeImages*/", row[DataTableConfigColumns.ResizeImages].ToString() },
                { "/*joomlaThumbsPath*/", row[DataTableConfigColumns.JoomlaThumbsPath].ToString() },
                { "/*joomlaImgSrcPath*/", row[DataTableConfigColumns.JoomlaImgSrcPath].ToString() }
            };

            jsonList.Add(jsonObj);
        }

        string jsonString = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
        File.WriteAllText(tbJsonFile.Text, jsonString);
    }

    private void btnLoadNew_Click(object sender, EventArgs e)
    {
        if (_dtGalleryConfiguration.Columns.Count == 0)
        {
            AddColumnsToDt();
        }
        dgvGalleryConfiguration.DataSource = _dtGalleryConfiguration;

        var galleries = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(File.ReadAllText(tbJsonFile.Text));
        foreach (Dictionary<string, string> setting in galleries)
        {
            DataRow drGalleryConfiguration = _dtGalleryConfiguration.NewRow();
            drGalleryConfiguration[DataTableConfigColumns.GalleryName] = setting["/*galleryName*/"];
            drGalleryConfiguration[DataTableConfigColumns.RootGalleryFolder] = setting["/*rootGalleryFolder*/"];
            drGalleryConfiguration[DataTableConfigColumns.WebPath] = setting["/*webPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.Gapikey] = setting["/*gapikey*/"];
            drGalleryConfiguration[DataTableConfigColumns.OgTitle] = setting["/*ogTitle*/"];
            drGalleryConfiguration[DataTableConfigColumns.OgDescription] = setting["/*ogDescription*/"];
            drGalleryConfiguration[DataTableConfigColumns.OgImage] = setting["/*ogImage*/"];
            drGalleryConfiguration[DataTableConfigColumns.OgUrl] = setting["/*ogUrl*/"];
            drGalleryConfiguration[DataTableConfigColumns.PicsJson] = setting["/*picsJson*/"];
            drGalleryConfiguration[DataTableConfigColumns.Zoom] = setting["/*zoom*/"];
            drGalleryConfiguration[DataTableConfigColumns.ResizeImages] = setting["/*resizeImages*/"];
            drGalleryConfiguration[DataTableConfigColumns.JoomlaThumbsPath] = setting["/*joomlaThumbsPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.JoomlaImgSrcPath] = setting["/*joomlaImgSrcPath*/"];
            _dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
        }
    }

    private void tbJsonFile_Leave(object sender, EventArgs e)
    {
        Settings.Default.JsonFile = tbJsonFile.Text;
        Settings.Default.Save();
        Settings.Default.Reload();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        tbJsonFile.Text = Settings.Default.JsonFile;
    }
}