using Common;
using System.Xml.Serialization;

namespace GeospatialData;

public class UpdateGeospatialDataIfExistsOrCreateNewIfNot(
    ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> updateGeospatialData
    , ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> createGeospatialData)
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