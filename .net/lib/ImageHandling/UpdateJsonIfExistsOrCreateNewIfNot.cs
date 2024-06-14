using System.Text.Json;
using Common;

namespace ImageHandling;

public class UpdateJsonIfExistsOrCreateNewIfNot : ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(UpdateJsonIfExistsOrCreateNewIfNotCommand command)
    {
        if (command.LatLngModel is null) throw new NullReferenceException("LatLngModel is null!");
        if (command.JsonFileName is null) throw new NullReferenceException("JsonFileName is null!");

        var latLngFileModel = new LatLngFileNameModel
        {
            Latitude = command.LatLngModel.Latitude,
            Longitude = command.LatLngModel.Longitude,
            FileName = command.ImageFileName
        };

        string latLngFileName;

        List<LatLngFileNameModel>? latLngFileModels = new List<LatLngFileNameModel>();

        if (File.Exists(command.JsonFileName))
        {
            string jsonString = File.ReadAllText(command.JsonFileName);
            try
            {
                latLngFileModels =
                    JsonSerializer.Deserialize<List<LatLngFileNameModel>>(jsonString);
                latLngFileModels?.Add(latLngFileModel);
                latLngFileName = JsonSerializer.Serialize(latLngFileModels);
            }
            catch
            {
                LatLngFileNameModel? latLngFileNameModelToSave =
                    JsonSerializer.Deserialize<LatLngFileNameModel>(jsonString);
                if (latLngFileNameModelToSave is not null) latLngFileModels?.Add(latLngFileNameModelToSave);
                latLngFileModels?.Add(latLngFileModel);
                latLngFileName = JsonSerializer.Serialize(latLngFileModels);
            }
        }
        else
        {
            latLngFileModels.Add(latLngFileModel);
            latLngFileName = JsonSerializer.Serialize(latLngFileModels);
        }

        if (!string.IsNullOrEmpty(latLngFileName))
            File.WriteAllText(command.JsonFileName, latLngFileName);
    }
}