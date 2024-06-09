namespace KmlHandling;

public interface IKmlSerializer //ToDO: it belongs to the consumer
{
    public void DoSerialization(object? kml, string? fileName);
    public KmlModel.Kml? DoDeserialization(string? fileName);

}