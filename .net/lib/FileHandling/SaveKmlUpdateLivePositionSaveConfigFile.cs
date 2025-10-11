using Common;
using KmlHandling;

namespace FileHandling;

public class SaveKmlUpdateLivePositionSaveConfigFile(
    ICommandHandler<UpdateKmlIfExistsOrCreateNewIfNotCommand> updateKmlIfExistsOrCreateNewIfNot,
    ICommandHandlerAsync<AddFileWithLastKnownGpsPositionCommand> addFileWithLastKnownGpsPosition,
    ICommandHandlerAsync<WriteConfigurationToJsonFileCommand> writeConfigurationToJsonFile) : ISaveKmlUpdateLivePositionSaveConfigFile
{
    public async Task Execute(UpdateKmlIfExistsOrCreateNewIfNotCommand updateKmlIfExistsOrCreateNewIfNotCommand,
        AddFileWithLastKnownGpsPositionCommand addFileWithLastKnownGpsPositionCommand
        , WriteConfigurationToJsonFileCommand writeConfigurationToJsonFileCommand)
    {
        updateKmlIfExistsOrCreateNewIfNot.Execute(updateKmlIfExistsOrCreateNewIfNotCommand);
        await writeConfigurationToJsonFile.Execute(writeConfigurationToJsonFileCommand);
        await addFileWithLastKnownGpsPosition.Execute(addFileWithLastKnownGpsPositionCommand);
    }
}