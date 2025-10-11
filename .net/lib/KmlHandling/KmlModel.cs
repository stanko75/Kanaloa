using System.Xml;
using System.Xml.Serialization;

namespace KmlHandling
{
    public record KmlModel
    {
        [XmlRoot("kml", Namespace = "http://www.opengis.net/kml/2.2")]
        public class Kml
        {
            public Document? Document { get; set; }
        }

        public class Document
        {
            [XmlElement("name")]
            public string? Name { get; set; }

            [XmlElement("description")]
            public string? Description { get; set; }

            public Style? Style { get; set; }

            [XmlElement("Placemark")]
            public Placemark[]? Placemarks { get; set; }

        }

        public class Style
        {
            [XmlAttribute("Id")]
            public string? Id { get; set; }

            public LineStyle? LineStyle { get; set; }
            public PolyStyle? PolyStyle { get; set; }
        }

        public class LineStyle
        {
            [XmlElement("color")]
            public string? Color { get; set; }
            [XmlElement("width")]
            public string? Width { get; set; }
        }

        public class PolyStyle
        {
            [XmlElement("color")]
            public string? Color { get; set; }
        }

        public class Placemark
        {
            [XmlElement("name")]
            public string? Name { get; set; }
            public string? Snippet { get; set; }
            [XmlElement("description")]
            public XmlCDataSection? Description { get; set; }
            [XmlElement("styleUrl")]
            public string? StyleUrl { get; set; }
            public Point? Point { get; set; }
            public LineString? LineString { get; set; }
        }

        public class LineString
        {
            [XmlElement("extrude")]
            public string? Extrude { get; set; }
            [XmlElement("tessellate")]
            public string? Tessellate { get; set; }
            [XmlElement("altitudeMode")]
            public string? AltitudeMode { get; set; }
            [XmlElement("coordinates")]
            public string? Coordinates { get; set; }
        }

        public class Point
        {
            public string? Coordinates { get; set; }
        }
    }
}