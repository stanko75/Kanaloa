using System.Data;
using System.Xml.Linq;
using FastLoadImagesToMemoryAndProcessLater.Log;
using HtmlHandling;
using Newtonsoft.Json;
using Pics2gMaps.Log;

namespace Pics2gMaps;

public partial class Form1 : Form
{
    private readonly DataTable _dtGalleryConfiguration = new();
    private ParallelForEachAndExtractGpsInfoWrapper _parallelForEachAndExtractGpsInfoWrapper;
    private DbHandling _dbHandling;
    private const string BaseUrl = "www.milosev.com";
    private const string JqueryVersion = "jquery-3.6.4.js";

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

        try
        {
            IProgress<int> recordCountProgress = new Progress<int>(UpdateRecordCount);
            //_parallelForEachAndExtractGpsInfoWrapper.OnGpsInfoFromImageExtracted += OnGpsInfoFromImageExtractedAddToMsSqlServer;
            //_dbHandling = new DbHandling();
            //await DbHandling.EnsureTableExists();

            _parallelForEachAndExtractGpsInfoWrapper = new ParallelForEachAndExtractGpsInfoWrapper();
            var extractGpsInfoAndResizeImageWrapper = new ExtractGpsInfoAndResizeImageWrapper(_parallelForEachAndExtractGpsInfoWrapper);
            var automaticallyFillMissingValuesInDataTable = new AutomaticallyFillMissingValuesInDataTable(new PathConverter());
            var extractGpsInfoAndCreateWebPageDataTable = new ExtractGpsInfoAndCreateWebPageDataTable(automaticallyFillMissingValuesInDataTable, extractGpsInfoAndResizeImageWrapper);
            var extractGpsInfoAndCreateWebPageDataTableCommand = new ExtractGpsInfoAndCreateWebPageDataTableCommand();
            extractGpsInfoAndCreateWebPageDataTableCommand.RecordCountProgress = recordCountProgress;

            foreach (DataRow dataRow in rows)
            {
                extractGpsInfoAndCreateWebPageDataTableCommand.DataRow = dataRow;
                extractGpsInfoAndCreateWebPageDataTableCommand.BaseUrl = BaseUrl;
                extractGpsInfoAndCreateWebPageDataTableCommand.Columns = _dtGalleryConfiguration.Columns;
                extractGpsInfoAndCreateWebPageDataTableCommand.JqueryVersion = JqueryVersion;
                extractGpsInfoAndCreateWebPageDataTableCommand.TemplateRootFolder = tbTemplateRootFolder.Text;

                await extractGpsInfoAndCreateWebPageDataTable.Execute(extractGpsInfoAndCreateWebPageDataTableCommand);
            }
        }
        catch (AggregateException ae)
        {
            ILogger logger = new TextBoxLogger(tbLog);
            logger.Log(ae);
        }
        catch (Exception ex)
        {
            ILogger logger = new TextBoxLogger(tbLog);
            logger.Log(ex);
        }
        finally
        {
            _cts?.Cancel();
            MessageBox.Show("Done!");
        }
    }

    private async void OnGpsInfoFromImageExtractedAddToMsSqlServer(object? sender, GpsInfoFromImageExtractedEventArgs e)
    {
        var dbHandlingCommand = new DbHandlingCommand
        {
            LatLngFileNameModel = e.LatLngFileName
        };

        await _dbHandling.Execute(dbHandlingCommand);
    }

    private void UpdateRecordCount(int recordCount)
    {
        if (IsDisposed) return;

        if (InvokeRequired)
        {
            BeginInvoke(() => UpdateStatus(recordCount));
        }
        else
        {
            UpdateStatus(recordCount);
        }
    }

    private void UpdateStatus(int processedFiles)
    {
        tsslRecordCount.Text = $"Processed files: {processedFiles}";
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
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.JqueryVersion, typeof(string));
        _dtGalleryConfiguration.Columns.Add(DataTableConfigColumns.OgImageFullPath, typeof(string));
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
                { "/*isMerged*/", string.IsNullOrWhiteSpace(row[DataTableConfigColumns.IsMerged].ToString()) ? "false" : row[DataTableConfigColumns.IsMerged].ToString() },
                { "/*jqueryVersion*/", row[DataTableConfigColumns.JqueryVersion].ToString() },
                { "/*ogImageFullPath*/", row[DataTableConfigColumns.OgImageFullPath].ToString() }
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
            drGalleryConfiguration[DataTableConfigColumns.Zoom] = setting.TryGetValue("/*zoom*/", out string? zoomValue) ? zoomValue : 13;
            drGalleryConfiguration[DataTableConfigColumns.ResizeImages] = setting.TryGetValue("/*resizeImages*/", out string? resizeImagesValue) ? resizeImagesValue : false;
            drGalleryConfiguration[DataTableConfigColumns.JoomlaThumbsPath] = setting["/*joomlaThumbsPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.JoomlaImgSrcPath] = setting["/*joomlaImgSrcPath*/"];
            drGalleryConfiguration[DataTableConfigColumns.IsMerged] = setting.TryGetValue("/*isMerged*/", out string? value) ? value : false;
            drGalleryConfiguration[DataTableConfigColumns.JqueryVersion] = setting.GetValueOrDefault("/*jqueryVersion*/", JqueryVersion);
            drGalleryConfiguration[DataTableConfigColumns.OgImageFullPath] = setting.GetValueOrDefault("/*ogImageFullPath*/", string.Empty);
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

        if (_dtGalleryConfiguration.Columns.Count == 0)
        {
            AddColumnsToDt();
        }

        dgvGalleryConfiguration.DataSource = _dtGalleryConfiguration;
    }
}