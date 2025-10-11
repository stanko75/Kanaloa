using System.Data;
using HtmlHandling;

namespace Pics2gMaps.Test;

[TestClass]
public sealed class AutomaticallyFillMissingValuesInDataTableTests
{
    [TestMethod]
    public void AutomaticallyFillMissingValuesInDataTableCheckIfOk()
    {
        DataTable dt = new DataTable();
        AddColumnsToDt(dt);

        DataRow dataRow = dt.NewRow();
        dataRow[DataTableConfigColumns.GalleryName] = "test";
        dataRow[DataTableConfigColumns.RootGalleryFolder] = "test";
        dataRow[DataTableConfigColumns.OgTitle] = "test";
        dataRow[DataTableConfigColumns.OgImage] = "test";

        string baseUrl = "www.milosev.com";
        string jqueryVersion = "jquery-3.6.4.js";

        AutomaticallyFillMissingValuesInDataTableCommand automaticallyFillMissingValuesCommand =
                new AutomaticallyFillMissingValuesInDataTableCommand
                {
                    DataRow = dataRow,
                    BaseUrl = baseUrl,
                    JqueryVersion = jqueryVersion
                };
        AutomaticallyFillMissingValuesInDataTable automaticallyFillMissingValues = new AutomaticallyFillMissingValuesInDataTable(new PathConverter());
        automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);
        Assert.AreEqual(dataRow[DataTableConfigColumns.WebPath], "http://www.milosev.com/test/");
        Assert.AreEqual(dataRow[DataTableConfigColumns.OgUrl], "http://www.milosev.com/test/test/www/index.html");
        Assert.AreEqual(dataRow[DataTableConfigColumns.PicsJson], "test");
        Assert.AreEqual(dataRow[DataTableConfigColumns.JoomlaThumbsPath], "/test/test/www/testThumbs.json");
        Assert.AreEqual(dataRow[DataTableConfigColumns.JoomlaImgSrcPath], "/test/test/www/");
        Assert.AreEqual(dataRow[DataTableConfigColumns.JqueryVersion], "jquery-3.6.4.js");
        Assert.AreEqual(dataRow[DataTableConfigColumns.OgImageFullPath], "http://www.milosev.com/test/test/test");
    }

    private void AddColumnsToDt(DataTable dt)
    {
        dt.Columns.Add(DataTableConfigColumns.GalleryName);
        dt.Columns.Add(DataTableConfigColumns.RootGalleryFolder, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.WebPath, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.Gapikey, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.OgTitle, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.OgDescription, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.OgImage, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.OgUrl, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.PicsJson, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.Zoom, typeof(int));
        dt.Columns.Add(DataTableConfigColumns.ResizeImages, typeof(bool));
        dt.Columns.Add(DataTableConfigColumns.JoomlaThumbsPath, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.JoomlaImgSrcPath, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.IsMerged, typeof(bool));
        dt.Columns.Add(DataTableConfigColumns.JqueryVersion, typeof(string));
        dt.Columns.Add(DataTableConfigColumns.OgImageFullPath, typeof(string));
    }
}