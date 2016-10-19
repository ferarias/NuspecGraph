using System.Xml.Serialization;

namespace DgmlWriter
{
    public struct Link
    {
        [XmlAttribute]
        public string Source;
        [XmlAttribute]
        public string Target;
        [XmlAttribute]
        public string Label;
        [XmlAttribute]
        public string Category;
        [XmlAttribute]
        public string FetchingParent;
        public Link(string source, string target, string category = null, string label = null)
        {
            Source = source;
            Target = target;
            Label = label;
            Category = category;
            if (category != null)
            {
                FetchingParent = source;
            }
            else
            {
                FetchingParent = null;
            }
        }
    }
}
