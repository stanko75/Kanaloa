using FunctionalTest.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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

        linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://kanaloa.azurewebsites.net/html/live/index.html");

        _cancellationDecoratorPostGpsPositionsFromFiles = new CancellationDecorator<PostGpsPositionsFromFilesWithFileNameCommand>(new PostGpsPositionsFromFilesWithFileNameHandler(new TextBoxLogger(log)), new TextBoxLogger(log));
        _cancellationDecoratorUploadToBlog = new CancellationDecorator<UploadToBlogCommand>(new UploadToBlogHandler(new TextBoxLogger(log)), new TextBoxLogger(log));
        _cancellationDecoratorUploadImage = new CancellationDecorator<UploadImageCommand>(new UploadImageHandler(new TextBoxLogger(log)), new TextBoxLogger(log));

        if (File.Exists(JsonConfigForTests))
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(JsonConfigForTests)
                .Build();

            foreach (TextBox tb in GetAllTextBoxes(this))
            {
                string? value = configuration[tb.Name];

                if (value is not null)
                {
                    tb.Text = value;
                }
            }
        }
    }

    private IEnumerable<TextBox> GetAllTextBoxes(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is TextBox textBox && control.Name != "log")
                yield return textBox;

            foreach (var childTextBox in GetAllTextBoxes(control))
                yield return childTextBox;
        }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        var jsonConfig = new JObject();

        foreach (TextBox tb in GetAllTextBoxes(this))
        {
            jsonConfig[tb.Name] = tb.Text;
        }

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
            FtpUser = tbFtpUser.Text,

            OgTitle = tbOgTitle.Text,
            OgImage = tbOgImage.Text,
            BaseUrl = tbBaseUrl.Text,
            ExpectedUrl = tbExpectedUrl.Text,
            PrepareForUpload = tbPrepareForUploadUrl.Text,

            DeleteFirstKmlPoints = tbDeleteFirstKmlPoints.Text,
            DeleteLastKmlPoints = tbDeleteLastKmlPoints.Text,

            JoomlaCategoryId = tbJoomlaCategoryId.Text,
            JoomlaLoginUrl = tbJoomlaLoginUrl.Text,
            JoomlaPostUrl = tbJoomlaPostUrl.Text,
            JoomlaUserName = tbJoomlaUserName.Text,
            JoomlaPass = tbJoomlaPass.Text,

            PhpUserName = tbPhpUserName.Text,
            PhpPass = tbPhpPass.Text,
            PhpUrl = tbPhpUrl.Text

        };

        await _cancellationDecoratorUploadToBlog.Execute(command);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _cancellationDecoratorUploadImage.CancelOperation();
        _cancellationDecoratorPostGpsPositionsFromFiles.CancelOperation();
        _cancellationDecoratorUploadToBlog.CancelOperation();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        string url = e.Link.LinkData?.ToString() ?? string.Empty;

        try
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void btnDefaultExpectedUrl_Click(object sender, EventArgs e)
    {
        tbExpectedUrl.Text = "http://www.milosev.com/gallery/allWithPics/travelBuddies/";
    }

    private void button1_Click(object sender, EventArgs e)
    {
        tbPrepareForUploadUrl.Text = "https://kanaloa.azurewebsites.net/prepareForUpload";
    }

    private void btnLocalAddress_Click(object sender, EventArgs e)
    {
        address.Text = "https://localhost:7001/";
    }

    private void btnAzureAddress_Click(object sender, EventArgs e)
    {
        address.Text = "https://kanaloa.azurewebsites.net/";
    }
}