using System.Xml;
using Common;

namespace KmlHandling;

public class UpdateKmlIfExistsOrCreateNewIfNot(IUpdateKml updateKml, ICreateKml createKml, IKmlSerializer kmlSerializer)
    : ICommandHandler<UpdateKmlIfExistsOrCreateNewIfNotCommand>
{
    public void Execute(UpdateKmlIfExistsOrCreateNewIfNotCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Coordinates))
            throw new Exception("Coordinates cannot be empty!");

        SaveKml saveKml;

        if (File.Exists(command.KmlFileName))
        {
            KmlModel.Kml? kmlToUpdate = kmlSerializer.DoDeserialization(command.KmlFileName);

            updateKml.OldKml = kmlToUpdate;
            updateKml.Placemark = new KmlModel.Placemark
            {
                LineString = new KmlModel.LineString
                {
                    Coordinates = command.Coordinates
                }
            };
            saveKml = new SaveKml(updateKml, kmlSerializer);
        }
        else
        {
            createKml.PlaceMarks =
            [
                new KmlModel.Placemark
                {
                    Name = "test"
                    , Description = new XmlDocument().CreateCDataSection("test")
                    , StyleUrl= "styleUrl test"
                    , LineString = new KmlModel.LineString
                    {
                        Extrude = "1"
                        , Tessellate = "1"
                        , AltitudeMode = "absolute"
                        , Coordinates = command.Coordinates
                    }
                }
            ];
            saveKml = new SaveKml(createKml, kmlSerializer);
        }

        saveKml.Execute(command.KmlFileName);
    }
}