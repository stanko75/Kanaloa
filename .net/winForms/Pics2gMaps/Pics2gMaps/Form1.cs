using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Common;
using FastLoadImagesToMemoryAndProcessLater.Log;
using HtmlHandling;
using ImageHandling;
using Newtonsoft.Json;
using Pics2gMaps.Log;
using Timer = System.Windows.Forms.Timer;

namespace Pics2gMaps;

public partial class Form1 : Form
{
    private readonly DataTable _dtGalleryConfiguration = new();

    public Form1()
    {
        InitializeComponent();
    }

    private CancellationTokenSource _cts;
    private TimeSpan _elapsedTime;
    private DateTime _startTime;
    private async void btnStart_Click(object sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();
        _startTime = DateTime.Now;

        _ = Task.Run(() =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                _elapsedTime = DateTime.Now - _startTime;

                // UI sicher aktualisieren
                Invoke((MethodInvoker)(() => tsslElapsedTime.Text = $"Elapsed time: {_elapsedTime.ToString(@"hh\:mm\:ss")}"));

                try
                {
                    Task.Delay(1000, _cts.Token); // 1 Sekunde warten
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            return Task.FromResult(Task.CompletedTask);
        }, _cts.Token);

        IEnumerable<DataRow> rows;

        if (dgvGalleryConfiguration.SelectedRows.Count == 0)
        {
            rows = _dtGalleryConfiguration.AsEnumerable();
        }
        else
        {
            rows = dgvGalleryConfiguration.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => (row.DataBoundItem as DataRowView)?.Row)
                .Where(row => row != null)!;
        }

        foreach (DataRow dataRow in rows)
        {
            PrepareHtmlFolderCommand prepareHtmlFolderCommand = new PrepareHtmlFolderCommand
            {
                DataRow = dataRow,
                TemplateRootFolder = tbTemplateRootFolder.Text,
                Columns = _dtGalleryConfiguration.Columns
            };
            PrepareHtmlFolder prepareHtmlFolder = new PrepareHtmlFolder();
            prepareHtmlFolder.Execute(prepareHtmlFolderCommand);

            var fileQueue = new BlockingCollection<string>();
            ResizeImageDesktopCommand resizeImageDesktopCommand = new ResizeImageDesktopCommand
            {
                DataRow = dataRow,
                FileQueue = fileQueue
            };
            ResizeImageDesktop resizeImageDesktop = new ResizeImageDesktop(new UiLogger());

            UpdateUi updateUi = new UpdateUi
            {
                Form = this,
                TextBox = tbLog,
                Name = "resizeImageDesktop"
            };
            resizeImageDesktop.UpdateUi = updateUi;

            resizeImageDesktop.RecordCountChanged += (senderRecordCountChanged, count) =>
            {
                tsslRecordCount.Text = $"Files processed: {count}";
                tsslRecordCount.GetCurrentParent().Refresh();

            };

            bool isMerged = (bool)dataRow[DataTableConfigColumns.IsMerged];
            string galleryName = dataRow[DataTableConfigColumns.GalleryName].ToString();
            string folderName = Path.Join(dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), galleryName);
            string picsFolder = isMerged ? folderName : Path.Join(folderName, "pics");

            SearchForAllFilesAndPutThemInQueueCommand searchForAllFilesAndPutThemInQueueCommand =
                new SearchForAllFilesAndPutThemInQueueCommand
                {
                    FileQueue = fileQueue,
                    Path = picsFolder
                };
            SearchForAllFilesAndPutThemInQueue searchForAllFilesAndPutThemInQueue =
                new SearchForAllFilesAndPutThemInQueue();

            await Task.WhenAll(searchForAllFilesAndPutThemInQueue.Execute(searchForAllFilesAndPutThemInQueueCommand),
                resizeImageDesktop.Execute(resizeImageDesktopCommand));
        }

        _cts?.Cancel();
        MessageBox.Show("Done!");
    }

    private void btnLoadOld_Click(object sender, EventArgs e)
    {
        _dtGalleryConfiguration.Clear();

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
                drGalleryConfiguration[DataTableConfigColumns.IsMerged] = false;
                _dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
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
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.IsMerged, typeof(bool));
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
                { "/*resizeImages*/", string.IsNullOrWhiteSpace(row[DataTableConfigColumns.ResizeImages].ToString()) ? "false" : row[DataTableConfigColumns.ResizeImages].ToString() },
                { "/*joomlaThumbsPath*/", row[DataTableConfigColumns.JoomlaThumbsPath].ToString() },
                { "/*joomlaImgSrcPath*/", row[DataTableConfigColumns.JoomlaImgSrcPath].ToString() },
                { "/*isMerged*/", string.IsNullOrWhiteSpace(row[DataTableConfigColumns.IsMerged].ToString()) ? "false" : row[DataTableConfigColumns.IsMerged].ToString()}
            };

            jsonList.Add(jsonObj);
        }

        string jsonString = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
        File.WriteAllText(tbJsonFile.Text, jsonString);
    }

    private void btnLoadNew_Click(object sender, EventArgs e)
    {
        _dtGalleryConfiguration.Clear();
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
            drGalleryConfiguration[DataTableConfigColumns.ResizeImages] = setting.TryGetValue("/*resizeImages*/", out string? resizeImagesValue) ? resizeImagesValue : false;
            drGalleryConfiguration[DataTableConfigColumns.JoomlaThumbsPath] = setting["/*joomlaThumbsPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.JoomlaImgSrcPath] = setting["/*joomlaImgSrcPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.IsMerged] = setting.TryGetValue("/*isMerged*/", out string? value) ? value : false;
            _dtGalleryConfiguration.Rows.Add(drGalleryConfiguration);
        }
    }

    private void tbTemplateRootFolder_Leave(object sender, EventArgs e)
    {
        Settings.Default.TemplateRootFolder = tbTemplateRootFolder.Text;
        Settings.Default.Save();
        Settings.Default.Reload();
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
        tbTemplateRootFolder.Text = string.IsNullOrWhiteSpace(Settings.Default.TemplateRootFolder) ? @"..\..\..\..\..\..\..\html\templateForBlog" : Settings.Default.TemplateRootFolder;

        if (_dtGalleryConfiguration.Columns.Count == 0)
        {
            AddColumnsToDt();
        }

        dgvGalleryConfiguration.DataSource = _dtGalleryConfiguration;
    }
}