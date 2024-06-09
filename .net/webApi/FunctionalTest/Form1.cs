using FunctionalTest.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FunctionalTest;

public partial class Form1 : Form
{
    private static readonly HttpClient HttpClientPost = new();
    private readonly CancellationDecorator<PostGpsPositionsFromFilesWithFileNameCommand> _cancellationDecoratorPostGpsPositionsFromFiles;
    private readonly CancellationDecorator<UploadToBlogCommand> _cancellationDecoratorUploadToBlog;
    private readonly CancellationDecorator<UploadImageCommand> _cancellationDecoratorUploadImage;

    private const string JsonConfigForTests = "jsconfigForTests.json";

    public Form1()
    {
        InitializeComponent();

        _cancellationDecoratorPostGpsPositionsFromFiles = new CancellationDecorator<PostGpsPositionsFromFilesWithFileNameCommand>(new PostGpsPositionsFromFilesWithFileNameHandler(new TextBoxLogger(log)), new TextBoxLogger(log));
        _cancellationDecoratorUploadToBlog = new CancellationDecorator<UploadToBlogCommand>(new UploadToBlogHandler(new TextBoxLogger(log)), new TextBoxLogger(log));
        _cancellationDecoratorUploadImage = new CancellationDecorator<UploadImageCommand>(new UploadImageHandler(new TextBoxLogger(log)), new TextBoxLogger(log));

        if (File.Exists(JsonConfigForTests))
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(JsonConfigForTests)
                .Build();

            string? addressValue = configuration.GetSection("address").Value;
            if (address is not null) address.Text = addressValue;

            string? gpsLocationsPath = configuration.GetSection("gpsLocationsPath").Value;
            if (gpsLocationsPath is not null) tbGpsLocationsPath.Text = gpsLocationsPath;

            string? ftpUser = configuration.GetSection("ftpUser").Value;
            if (ftpUser is not null) tbFtpUser.Text = ftpUser;

            string? ftpHost = configuration.GetSection("ftpHost").Value;
            if (ftpHost is not null) tbFtpHost.Text = ftpHost;

            string? ftpPass = configuration.GetSection("ftpPass").Value;
            if (ftpPass is not null) tbFtpPass.Text = ftpPass;

            string? strImagesPath = configuration.GetSection("imagesPath").Value;
            if (strImagesPath is not null) imagesPath.Text = strImagesPath;

            string? strKmlFileName = configuration.GetSection("kmlFileName").Value;
            if (strKmlFileName is not null) kmlFileName.Text = strKmlFileName;

            string? strFolderName = configuration.GetSection("folderName").Value;
            if (strFolderName is not null) folderName.Text = strFolderName;
        }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        var jsonConfig = new JObject
        {
            ["address"] = address.Text,
            ["gpsLocationsPath"] = tbGpsLocationsPath.Text,
            ["ftpUser"] = tbFtpUser.Text,
            ["ftpHost"] = tbFtpHost.Text,
            ["ftpPass"] = tbFtpPass.Text,
            ["folderName"] = folderName.Text,
            ["kmlFileName"] = kmlFileName.Text,
            ["imagesPath"] = imagesPath.Text
        };

        string json = jsonConfig.ToString(Formatting.Indented);
        File.WriteAllText(@$"..\..\..\{JsonConfigForTests}", json);
    }

    private async void PostGpsPositionsFromFilesWithFileName_Click(object sender, EventArgs e)
    {
        var command = new PostGpsPositionsFromFilesWithFileNameCommand
        {
            AddressText = address.Text,
            KmlFileName = kmlFileName.Text,
            FolderName = folderName.Text,
            HttpClientPost = HttpClientPost,
            GpsLocationsPath = tbGpsLocationsPath.Text
        };

        await _cancellationDecoratorPostGpsPositionsFromFiles.Execute(command);
    }

    private async void UploadImage_Click(object sender, EventArgs e)
    {
        var command = new UploadImageCommand
        {
            AddressText = address.Text,
            KmlFileName = kmlFileName.Text,
            FolderName = folderName.Text,
            HttpClientPost = HttpClientPost,
            ImagesPath = imagesPath.Text
        };

        await _cancellationDecoratorUploadImage.Execute(command);
    }

    private async void UploadToBlog_Click(object sender, EventArgs e)
    {
        var command = new UploadToBlogCommand
        {
            AddressText = address.Text,
            KmlFileName = kmlFileName.Text,
            FolderName = folderName.Text,
            HttpClientPost = HttpClientPost,
            FtpHost = tbFtpHost.Text,
            FtpPass = tbFtpPass.Text,
            FtpUser = tbFtpUser.Text
        };

        await _cancellationDecoratorUploadToBlog.Execute(command);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _cancellationDecoratorUploadImage.CancelOperation();
        _cancellationDecoratorPostGpsPositionsFromFiles.CancelOperation();
        _cancellationDecoratorUploadToBlog.CancelOperation();
    }
}