using Common;
using System.Data;
using HtmlHandling;

namespace Pics2gMaps;

public class AutomaticallyFillMissingValuesInDataTable(IPathConverter pathConverter) : ICommandHandler<AutomaticallyFillMissingValuesInDataTableCommand>
{
    public void Execute(AutomaticallyFillMissingValuesInDataTableCommand command)
    {
        DoAutomaticallyFillMissingValues(command.DataRow
            , command.BaseUrl
            , command.JqueryVersion);
    }

    private void DoAutomaticallyFillMissingValues(
        DataRow dataRow
        , string baseUrl
        , string jqueryVersion)
    {
        string[] columnsWhichCantBeNull =
        [
            DataTableConfigColumns.GalleryName
            , DataTableConfigColumns.RootGalleryFolder
            , DataTableConfigColumns.OgTitle
            , DataTableConfigColumns.OgImage
        ];

        if (columnsWhichCantBeNull.Any(column =>
                dataRow.IsNull(column) || string.IsNullOrWhiteSpace(dataRow[column].ToString())))
        {
            IEnumerable<string> emptyColumns = columnsWhichCantBeNull.Where(column =>
                dataRow.IsNull(column) || string.IsNullOrWhiteSpace(dataRow[column].ToString()));
            throw new Exception($"Columns: {string.Join(", ", emptyColumns)} are not allowed to be empty!");
        }

        var automaticallyFillMissingValuesCommand =
            new AutomaticallyFillMissingValuesCommand
            {
                GalleryName = dataRow[DataTableConfigColumns.GalleryName].ToString()
                , RootGalleryFolder = dataRow[DataTableConfigColumns.RootGalleryFolder].ToString()
                , WebPath = dataRow[DataTableConfigColumns.WebPath].ToString()
                , Gapikey = dataRow[DataTableConfigColumns.Gapikey].ToString()
                , OgTitle = dataRow[DataTableConfigColumns.OgTitle].ToString()
                , OgImage = dataRow[DataTableConfigColumns.OgImage].ToString()
                , OgUrl = dataRow[DataTableConfigColumns.OgUrl].ToString()
                , PicsJson = dataRow[DataTableConfigColumns.PicsJson].ToString()
                , Zoom = (int?)(dataRow.IsNull(DataTableConfigColumns.Zoom) ? 13 : dataRow[DataTableConfigColumns.Zoom])
                , ResizeImages = (bool?)(dataRow.IsNull(DataTableConfigColumns.ResizeImages) ? false : dataRow[DataTableConfigColumns.ResizeImages])
                , JoomlaThumbsPath = dataRow[DataTableConfigColumns.JoomlaThumbsPath].ToString()
                , JoomlaImgSrcPath = dataRow[DataTableConfigColumns.JoomlaImgSrcPath].ToString()
                , IsMerged = (bool?)(dataRow.IsNull(DataTableConfigColumns.ResizeImages) ? false : dataRow[DataTableConfigColumns.ResizeImages])
                , JqueryVersion = jqueryVersion
                , OgImageFullPath = dataRow[DataTableConfigColumns.OgImageFullPath].ToString()
                , BaseUrl = baseUrl
            };

        var automaticallyFillMissingValues = new AutomaticallyFillMissingValues(pathConverter);
        automaticallyFillMissingValues.Execute(automaticallyFillMissingValuesCommand);

        dataRow[DataTableConfigColumns.GalleryName] = automaticallyFillMissingValuesCommand.GalleryName;
        dataRow[DataTableConfigColumns.RootGalleryFolder] = automaticallyFillMissingValuesCommand.RootGalleryFolder;
        dataRow[DataTableConfigColumns.WebPath] = automaticallyFillMissingValuesCommand.WebPath;
        dataRow[DataTableConfigColumns.Gapikey] = automaticallyFillMissingValuesCommand.Gapikey;
        dataRow[DataTableConfigColumns.OgTitle] = automaticallyFillMissingValuesCommand.OgTitle;
        dataRow[DataTableConfigColumns.OgImage] = automaticallyFillMissingValuesCommand.OgImage;
        dataRow[DataTableConfigColumns.OgUrl] = automaticallyFillMissingValuesCommand.OgUrl;
        dataRow[DataTableConfigColumns.PicsJson] = automaticallyFillMissingValuesCommand.PicsJson;
        dataRow[DataTableConfigColumns.Zoom] = automaticallyFillMissingValuesCommand.Zoom;
        dataRow[DataTableConfigColumns.ResizeImages] = automaticallyFillMissingValuesCommand.ResizeImages;
        dataRow[DataTableConfigColumns.JoomlaThumbsPath] = automaticallyFillMissingValuesCommand.JoomlaThumbsPath;
        dataRow[DataTableConfigColumns.JoomlaImgSrcPath] = automaticallyFillMissingValuesCommand.JoomlaImgSrcPath;
        dataRow[DataTableConfigColumns.ResizeImages] = automaticallyFillMissingValuesCommand.IsMerged;
        dataRow[DataTableConfigColumns.JqueryVersion] = automaticallyFillMissingValuesCommand.JqueryVersion;
        dataRow[DataTableConfigColumns.OgImageFullPath] = automaticallyFillMissingValuesCommand.OgImageFullPath;
    }
}