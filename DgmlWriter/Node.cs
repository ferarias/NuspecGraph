using System.Xml.Serialization;

namespace DGML
{
    public struct Node
    {
        [XmlAttribute]
        public string Id;
        [XmlAttribute]
        public string Label;
        [XmlAttribute]
        public string Category;
        [XmlAttribute]
        public string FilePath;
        [XmlAttribute]
        public string Group;
        public Node(string id, string label, string group = null, string category = null, string filePath = null)
        {
            Id = id;
            Label = label;
            Category = category;
            FilePath = filePath;
            Group = group;
        }
    }
}
