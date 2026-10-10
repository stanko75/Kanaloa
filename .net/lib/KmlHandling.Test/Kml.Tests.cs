using Common;
using System.Xml.Linq;

namespace KmlHandling.Test;

[TestClass]
public class KmlTests
{
    private readonly string _testFolder = Path.Combine(Path.GetTempPath(), "GeospatialData");
    private readonly string _geospatialDataFileName = "test";

    [TestMethod]
    public void CreateKml()
    {
        UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand =
            new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
            {
                Coordinates = "6.641421,43.1869746,2357",
                GeospatialDataFileName = _geospatialDataFileName,
                GeospatialDataFileNameExtension = "kml",
                FolderName = _testFolder
            };

        string geospatialDataFullFileName = Path.ChangeExtension(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName, updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileNameExtension);
        geospatialDataFullFileName = Path.Join(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.FolderName, geospatialDataFullFileName);

        if (File.Exists(geospatialDataFullFileName))
        {
            File.Delete(geospatialDataFullFileName);
        }

        CreateKml createKml = new CreateKml();
        createKml.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);
        Assert.IsTrue(IsExpectedKml(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName));
    }

    [TestMethod]
    public void UpdateKml()
    {
        UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand updateGeospatialDataIfExistsOrCreateNewIfNotCommand =
            new UpdateGeospatialDataIfExistsOrCreateNewIfNotCommand
            {
                Coordinates = "6.641421,43.1869746,2357",
                GeospatialDataFileName = _geospatialDataFileName,
                GeospatialDataFileNameExtension = "kml",
                FolderName = _testFolder
            };

        string geospatialDataFullFileName = Path.ChangeExtension(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName, updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileNameExtension);
        geospatialDataFullFileName = Path.Join(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.FolderName, geospatialDataFullFileName);

        if (File.Exists(geospatialDataFullFileName))
        {
            File.Delete(geospatialDataFullFileName);
        }

        CreateKml createKml = new CreateKml();
        createKml.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);

        updateGeospatialDataIfExistsOrCreateNewIfNotCommand.Coordinates = "6.982319,50.2130308,501.0483676407712";
        UpdateKml updateKml = new UpdateKml();
        updateKml.Execute(updateGeospatialDataIfExistsOrCreateNewIfNotCommand);

        Assert.IsTrue(IsExpectedKml(updateGeospatialDataIfExistsOrCreateNewIfNotCommand.GeospatialDataFileName, "6.641421,43.1869746,2357,6.982319,50.2130308,501.0483676407712"));
    }

    public static bool IsExpectedKml(string kmlFileName, string expectedCoordinates = "6.641421,43.1869746,2357")
    {
        try
        {
            if (!File.Exists(kmlFileName))
                return false;

            string kml = File.ReadAllText(kmlFileName);
            if (!kml.Contains("<![CDATA[test]]>"))
                return false;

            XDocument document = XDocument.Parse(kml);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            XElement? kmlElement = document.Element(ns + "kml");
            XElement? documentElement = kmlElement?.Element(ns + "Document");
            XElement? placemarkElement = documentElement?.Element(ns + "Placemark");
            XElement? lineStringElement = placemarkElement?.Element(ns + "LineString");

            return documentElement?.Element(ns + "name")?.Value == "test"
                   && documentElement.Element(ns + "description")?.Value == "test"
                   && placemarkElement?.Element(ns + "name")?.Value == "test"
                   && placemarkElement.Element(ns + "description")?.Value == "test"
                   && placemarkElement.Element(ns + "styleUrl")?.Value == "styleUrl test"
                   && lineStringElement?.Element(ns + "extrude")?.Value == "1"
                   && lineStringElement.Element(ns + "tessellate")?.Value == "1"
                   && lineStringElement.Element(ns + "altitudeMode")?.Value == "absolute"
                   && lineStringElement.Element(ns + "coordinates")?.Value == expectedCoordinates;
        }
        catch
        {
            return false;
        }
    }
}