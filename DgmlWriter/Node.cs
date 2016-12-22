using System.Xml.Serialization;

namespace Toolfactory.Dgml
{
    public struct Node
    {
        public static Node EmptyNode = new Node(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

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

        public bool IsEmpty => string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Label) && string.IsNullOrEmpty(Category);

        public override string ToString()
        {
            return $"{Id} - {Label}";
        }
    }

}
