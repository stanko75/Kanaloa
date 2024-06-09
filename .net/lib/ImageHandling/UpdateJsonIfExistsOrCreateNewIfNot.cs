using Common;
using ImageHandling;
using System.Text.Json;

namespace PreparePicturesAndHtmlAndUploadToWebsite;

public class UpdateJsonIfExistsOrCreateNewIfNot : ICommandHandler<UpdateJsonIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(string jsonFileName, LatLngModel latLngFileNameModel)
    {
        string latLngFileName;

        List<LatLngModel>? latLngFileNameModels = new List<LatLngModel>();

        if (File.Exists(jsonFileName))
        {
            string jsonString = File.ReadAllText(jsonFileName);
            try
            {
                latLngFileNameModels =
                    JsonSerializer.Deserialize<List<LatLngModel>>(jsonString);
                latLngFileNameModels?.Add(latLngFileNameModel);
                latLngFileName = JsonSerializer.Serialize(latLngFileNameModels);
            }
            catch
            {
                LatLngModel? latLngFileNameModelToSave =
                    JsonSerializer.Deserialize<LatLngModel>(jsonString);
                if (latLngFileNameModelToSave is not null) latLngFileNameModels?.Add(latLngFileNameModelToSave);
                latLngFileNameModels?.Add(latLngFileNameModel);
                latLngFileName = JsonSerializer.Serialize(latLngFileNameModels);
            }
        }
        else
        {
            latLngFileNameModels?.Add(latLngFileNameModel);
            latLngFileName = JsonSerializer.Serialize(latLngFileNameModels);
        }

        if (!string.IsNullOrEmpty(latLngFileName))
            File.WriteAllText(jsonFileName, latLngFileName);
    }

    public void Execute(UpdateJsonIfExistsOrCreateNewIfNotCommand command)
    {
        throw new NotImplementedException();
    }
}