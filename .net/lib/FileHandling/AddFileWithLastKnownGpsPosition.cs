using System.Text.Json;
using Common;

namespace FileHandling;

public class AddFileWithLastKnownGpsPosition : ICommandHandlerAsync<AddFileWithLastKnownGpsPositionCommand>
{
    public async Task Execute(AddFileWithLastKnownGpsPositionCommand command)
    {
        var objCoordinates = new
        {
            lat = command.GpsCommand?.Latitude
            , lng = command.GpsCommand?.Longitude
        };
        string jsonCoordinates = JsonSerializer.Serialize(objCoordinates);
        await File.WriteAllTextAsync(command.CurrentLocationFileName, jsonCoordinates);
    }
}