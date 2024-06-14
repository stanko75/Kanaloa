using Common;
using FileHandling;
using KmlHandling;
using Microsoft.Extensions.FileProviders;
using System.Runtime;
using Kanaloa;
using ImageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().AddNewtonsoftJson();

builder.Services.Configure<KanaloaSettings>(builder.Configuration.GetSection("KanaloaSettings"));

builder.Services.AddSingleton<IUpdateKml, UpdateKml>();
builder.Services.AddSingleton<IKmlSerializer>(_ => new KmlSerializerTextWriter(typeof(KmlModel.Kml)));
builder.Services.AddSingleton<ICreateKml>(_ => new CreateKml("test", "test"));
builder.Services.AddSingleton<ICommandHandler<UpdateKmlIfExistsOrCreateNewIfNotCommand>, UpdateKmlIfExistsOrCreateNewIfNot>();

builder.Services.AddSingleton<ICommandHandlerAsync<AddFileWithLastKnownGpsPositionCommand>, AddFileWithLastKnownGpsPosition>();
builder.Services.AddSingleton<ICommandHandlerAsync<WriteConfigurationToJsonFileCommand>, WriteConfigurationToJsonFile>();
builder.Services.AddSingleton<ISaveKmlUpdateLivePositionSaveConfigFile, SaveKmlUpdateLivePositionSaveConfigFile>();

builder.Services.AddSingleton<ICommandHandler<ResizeImageCommand>, ResizeImage>();
builder.Services.AddSingleton<ICommandHandler<ExtractGpsInfoFromImageCommand>, ExtractGpsInfoFromImage>();


builder.Services.AddSingleton<ICommandHandler<UpdateOrCreateJsonFileWithListOfImagesForThumbsCommand>, UpdateOrCreateJsonFileWithListOfImagesForThumbs>();
//builder.Services.AddSingleton<ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand>, UpdateJsonIfExistsOrCreateNewIfNot>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory())),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
