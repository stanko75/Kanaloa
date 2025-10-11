using Common;
using FileHandling;
using ImageHandling;
using Kanaloa;
using KmlHandling;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddMvc().AddNewtonsoftJson();

builder.Services.AddSingleton<IUpdateKml, UpdateKml>();
builder.Services.AddSingleton<IKmlSerializer>(_ => new KmlSerializerTextWriter(typeof(KmlModel.Kml)));
builder.Services.AddSingleton<ICreateKml>(_ => new CreateKml("test", "test"));
builder.Services.AddSingleton<ICommandHandler<UpdateKmlIfExistsOrCreateNewIfNotCommand>, UpdateKmlIfExistsOrCreateNewIfNot>();

builder.Services.AddSingleton<ICommandHandlerAsync<AddFileWithLastKnownGpsPositionCommand>, AddFileWithLastKnownGpsPosition>();
builder.Services.AddSingleton<ICommandHandlerAsync<WriteConfigurationToJsonFileCommand>, WriteConfigurationToJsonFile>();
builder.Services.AddSingleton<ISaveKmlUpdateLivePositionSaveConfigFile, SaveKmlUpdateLivePositionSaveConfigFile>();
builder.Services.AddScoped<ICommandHandlerAsync<PrepareToResizeImageDecoratorCommand>, PrepareToResizeImageDecorator>();
builder.Services.AddScoped<ICommandHandler<ResizeImageCommand>, ResizeImage>();
builder.Services.AddScoped<ICommandHandler<ExtractGpsInfoFromImageCommand>, ExtractGpsInfoFromImage>();
builder.Services.AddScoped<ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand>, UpdateOrCreateJsonFileWithListOfImagesForThumbs>();
builder.Services.AddScoped<ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand>, UpdateJsonIfExistsOrCreateNewIfNot>();
builder.Services.AddScoped<ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesCommand>, UpdateOrCreateJsonFileWithListOfImages>();

builder.Services.Configure<KanaloaSettings>(builder.Configuration.GetSection("KanaloaSettings"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory())),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

string currentDir = Directory.GetCurrentDirectory();
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(currentDir),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();