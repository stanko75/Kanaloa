using Common;
using FileHandling;
using Kanaloa;
using KmlHandling;

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

builder.Services.Configure<KanaloaSettings>(builder.Configuration.GetSection("KanaloaSettings"));


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();