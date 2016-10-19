using System.Xml.Serialization;

namespace DGML
{
    public struct Property
    {
        [XmlAttribute]
        public string Id;
        [XmlAttribute]
        public string DataType;
        [XmlAttribute]
        public string Label;
        [XmlAttribute]
        public string Description;
        public Property(string id, string dataType = null, string label = null, string description = null)
        {
            Id = id;
            DataType = dataType;
            Label = label;
            Description = description;
        }
    }
}
