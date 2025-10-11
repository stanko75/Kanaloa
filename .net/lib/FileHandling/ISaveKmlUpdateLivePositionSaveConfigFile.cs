using KmlHandling;

namespace FileHandling;

public interface ISaveKmlUpdateLivePositionSaveConfigFile
{
    public Task Execute(UpdateKmlIfExistsOrCreateNewIfNotCommand updateKmlIfExistsOrCreateNewIfNotCommand
        , AddFileWithLastKnownGpsPositionCommand addFileWithLastKnownGpsPositionCommand
        , WriteConfigurationToJsonFileCommand writeConfigurationToJsonFileCommand);
}