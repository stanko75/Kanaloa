﻿using System.Data;

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
                    Columns = dt.Columns,
                    BaseUrl = baseUrl,
                    JqueryVersion = jqueryVersion
                };
        AutomaticallyFillMissingValuesInDataTable automaticallyFillMissingValues = new AutomaticallyFillMissingValuesInDataTable();
        automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);
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