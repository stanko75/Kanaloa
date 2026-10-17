using Common;

namespace FileHandling;

public interface ISaveGeospatialDataUpdateLivePositionSaveConfigFile
{
    public Task Execute(UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand
        , AddFileWithLastKnownGpsPositionCommand addFileWithLastKnownGpsPositionCommand
        , WriteConfigurationToJsonFileCommand writeConfigurationToJsonFileCommand);
}