namespace KmlHandling;

public interface IUpdateKmlIfExistsOrCreateNewIfNot
{
    void Execute(string fileName, string coordinates);
}