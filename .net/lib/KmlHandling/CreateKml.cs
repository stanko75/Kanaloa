namespace KmlHandling;

public class CreateKml: ICreateKml
{
    public CreateKml(string name, string description)
    {
        Name = name; 
        Description = description;
    }

    public KmlModel.Kml GenerateKml()
    {
        KmlModel.Kml kmlModel = new()
        {
            Document = new KmlModel.Document
            {
                Name = Name
                , Description = Description
                , Style = Style
                , Placemarks = PlaceMarks
            }
        };

        return kmlModel;
    }

    private string? Description { get; }
    private string? Name { get; }

    public KmlModel.Style? Style { get; set; }
    public KmlModel.Placemark[]? PlaceMarks { get; set; }
}