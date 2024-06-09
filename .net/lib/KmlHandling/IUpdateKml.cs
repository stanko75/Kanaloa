namespace KmlHandling;

public interface IUpdateKml : IKmlGenerator
{
    public KmlModel.Style? Style { get; set; }
    public KmlModel.Placemark? Placemark { get; set; }

    public KmlModel.Kml? OldKml { get; set; }
}