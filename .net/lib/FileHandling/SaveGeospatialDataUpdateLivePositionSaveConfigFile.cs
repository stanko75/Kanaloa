using Common;

namespace FileHandling;

public class SaveGeospatialDataUpdateLivePositionSaveConfigFile(
    ICommandHandlerAsync<UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand> updateGeospatialDataIfExistsOrCreateNewIfNot,
    ICommandHandlerAsync<AddFileWithLastKnownGpsPositionCommand> addFileWithLastKnownGpsPosition,
    ICommandHandlerAsync<WriteConfigurationToJsonFileCommand> writeConfigurationToJsonFile) : ISaveGeospatialDataUpdateLivePositionSaveConfigFile
{
    public async Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand,
        AddFileWithLastKnownGpsPositionCommand addFileWithLastKnownGpsPositionCommand
        , WriteConfigurationToJsonFileCommand writeConfigurationToJsonFileCommand)
    {
        await updateGeospatialDataIfExistsOrCreateNewIfNot.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);
        await writeConfigurationToJsonFile.Execute(writeConfigurationToJsonFileCommand);
        await addFileWithLastKnownGpsPosition.Execute(addFileWithLastKnownGpsPositionCommand);
    }
}