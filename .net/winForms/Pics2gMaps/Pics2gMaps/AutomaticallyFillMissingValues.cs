using Common;
using System.Buffers.Text;
using System.Data;

namespace Pics2gMaps;

public class AutomaticallyFillMissingValues : ICommandHandler<AutomaticallyFillMissingValuesCommand>
{
    public void Execute(AutomaticallyFillMissingValuesCommand command)
    {
        DoAutomaticallyFillMissingValues(command.DataRow
            , command.Columns
            , command.BaseUrl
            , command.JqueryVersion);
    }

    private void DoAutomaticallyFillMissingValues(
        DataRow dataRow
        , DataColumnCollection columns
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

        string galleryFullWebPath = string.Empty;
        string relativeWebPath = string.Empty;

        foreach (DataColumn dataColumn in columns)
        {
            if (dataRow.IsNull(dataColumn) || string.IsNullOrWhiteSpace(dataRow[dataColumn].ToString()))
            {
                if (dataColumn.ColumnName == DataTableConfigColumns.WebPath)
                {
                    dataRow[dataColumn] = ConvertToUrl(dataRow[DataTableConfigColumns.RootGalleryFolder].ToString(), baseUrl);
                    galleryFullWebPath = dataRow[DataTableConfigColumns.WebPath] +
                                         dataRow[DataTableConfigColumns.GalleryName].ToString();
                }

                if (dataColumn.ColumnName == DataTableConfigColumns.OgUrl)
                {
                    dataRow[dataColumn] = galleryFullWebPath + "/www/index.html";
                }

                if (dataColumn.ColumnName == DataTableConfigColumns.PicsJson)
                {
                    dataRow[dataColumn] = dataRow[DataTableConfigColumns.GalleryName];
                }

                relativeWebPath = ConvertWindowsPathToWebPath(
                    dataRow[DataTableConfigColumns.RootGalleryFolder].ToString());
                string joomlaImgSrcPath = relativeWebPath + dataRow[DataTableConfigColumns.GalleryName] + "/www/";
                if (dataColumn.ColumnName == DataTableConfigColumns.JoomlaImgSrcPath)
                {
                    dataRow[dataColumn] = joomlaImgSrcPath;
                }

                if (dataColumn.ColumnName == DataTableConfigColumns.JoomlaThumbsPath)
                {
                    dataRow[dataColumn] = joomlaImgSrcPath + dataRow[DataTableConfigColumns.GalleryName] + "Thumbs.json";
                }

                if (dataColumn.ColumnName == DataTableConfigColumns.JqueryVersion)
                {
                    dataRow[dataColumn] = jqueryVersion;
                }
            }
        }
    }

    static string ConvertToUrl(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("URL is not allowed to be empty.");

        baseUrl = AddHttp(baseUrl);

        return $"{baseUrl.TrimEnd('/')}{ConvertWindowsPathToWebPath(path)}";
    }

    private static string AddHttp(string baseUrl)
    {
        if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = "http://" + baseUrl;
        }

        return baseUrl;
    }

    static string ConvertWindowsPathToWebPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is not allowed to be empty.");

        string webPath = path.Replace("\\", "/");

        int index = webPath.IndexOf("/", StringComparison.Ordinal);
        if (index != -1)
        {
            webPath = webPath.Substring(index);
        }

        return $"{webPath.TrimEnd('/')}/";
    }
}