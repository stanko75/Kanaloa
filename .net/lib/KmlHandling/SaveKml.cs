namespace KmlHandling;

public class SaveKml
{
    private readonly IKmlGenerator _kmlGenerator;
    private readonly IKmlSerializer _kmlSerializer;

    public SaveKml(IKmlGenerator kmlGenerator
                   , IKmlSerializer kmlSerializer
                  )
    {
        _kmlGenerator = kmlGenerator ?? throw new ArgumentNullException(nameof(kmlGenerator));
        _kmlSerializer = kmlSerializer ?? throw new ArgumentNullException(nameof(kmlSerializer));
    }

    public void Execute(string? fileName)
    {
        _kmlSerializer.DoSerialization(_kmlGenerator.GenerateKml(), fileName);
    }
}