using System.Xml.Serialization;

namespace DgmlWriter
{
    public struct Graph
    {
        [XmlAttribute("Background")]
        public string Background;

        [XmlAttribute("Stroke")]
        public string Stroke;
        public Node[] Nodes;
        public Link[] Links;
        public Category[] Categories;
        public Property[] Properties;
        public Style[] Styles;
    }
}
