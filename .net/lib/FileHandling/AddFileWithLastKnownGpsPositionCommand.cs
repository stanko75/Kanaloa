namespace FileHandling;

public class AddFileWithLastKnownGpsPositionCommand: CommandBase
{
    public GpsCommand? GpsCommand { get; set; }
}