using Common;
using ImageHandling;
using System.Text.Json;

namespace PreparePicturesAndHtmlAndUploadToWebsite;

public class UpdateJsonIfExistsOrCreateNewIfNot : ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(UpdateJsonIfExistsOrCreateNewIfNotCommand command)
    {
        if (command.LatLngModel is null) throw new NullReferenceException("LatLngModel is null!");
        if (command.JsonFileName is null) throw new NullReferenceException("JsonFileName is null!");

        string latLngFileName;

        List<LatLngModel>? latLngModels = new List<LatLngModel>();

        if (File.Exists(command.JsonFileName))
        {
            string jsonString = File.ReadAllText(command.JsonFileName);
            try
            {
                latLngModels =
                    JsonSerializer.Deserialize<List<LatLngModel>>(jsonString);
                latLngModels?.Add(command.LatLngModel);
                latLngFileName = JsonSerializer.Serialize(latLngModels);
            }
            catch
            {
                LatLngModel? latLngFileNameModelToSave =
                    JsonSerializer.Deserialize<LatLngModel>(jsonString);
                if (latLngFileNameModelToSave is not null) latLngModels?.Add(latLngFileNameModelToSave);
                latLngModels?.Add(command.LatLngModel);
                latLngFileName = JsonSerializer.Serialize(latLngModels);
            }
        }
        else
        {
            latLngModels?.Add(command.LatLngModel);
            latLngFileName = JsonSerializer.Serialize(latLngModels);
        }

        if (!string.IsNullOrEmpty(latLngFileName))
            File.WriteAllText(command.JsonFileName, latLngFileName);
    }
}