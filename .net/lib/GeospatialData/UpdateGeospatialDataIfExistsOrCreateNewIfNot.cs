using Common;
using System.Xml.Serialization;

namespace GeospatialData;

public class UpdateGeospatialDataIfExistsOrCreateNewIfNot(
    ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> updateGeospatialData
    , ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> createGeospatialData)
    : ICommandHandler<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Coordinates))
            throw new Exception("Coordinates cannot be empty!");

        if (File.Exists(command.GeospatialDataFileName))
        {
            updateGeospatialData.Execute(command);
        }
        else
        {
            createGeospatialData.Execute(command);
        }
    }
}