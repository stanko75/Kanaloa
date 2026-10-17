using Common;
using Microsoft.Extensions.DependencyInjection;

namespace GeospatialData;

public class UpdateGeospatialDataIfExistsOrCreateNewIfNot(
    [FromKeyedServices("UpdateGeospatialData")]
    ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> updateGeospatialData,

    [FromKeyedServices("CreateGeospatialData")]
    ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> createGeospatialData)
    : ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public async Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Coordinates))
            throw new Exception("Coordinates cannot be empty!");

        if (File.Exists(command.GeospatialDataFileName))
        {
            await updateGeospatialData.Execute(command);
        }
        else
        {
            await createGeospatialData.Execute(command);
        }
    }
}