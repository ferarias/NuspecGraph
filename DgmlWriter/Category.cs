using System.Xml.Serialization;

namespace DGML
{
    public struct Category
    {
        [XmlAttribute]
        public string Id;
        [XmlAttribute]
        public string Label;
        [XmlAttribute]
        public string BasedOn;
        [XmlAttribute]
        public string CanBeDataDriven;
        [XmlAttribute]
        public string CanLinkedNodesBeDataDriven;
        [XmlAttribute]
        public string DefaultAction;
        [XmlAttribute]
        public string Icon;
        [XmlAttribute]
        public string NavigationActionLabel;
        [XmlAttribute]
        public string IncomingActionLabel;
        [XmlAttribute]
        public string OutgoingActionLabel;
        [XmlAttribute]
        public string Description;
        [XmlAttribute]
        public string IsContainment;

        public Category(string id, string label, string navigationActionLabel = null, string basedOn = null,
            string canBeDataDriven = null, string canLinkedNodesBeDataDriven = null, string defaultAction = null,
            string icon = null, string incomingActionLabel = null, string outgoingActionLabel = null,
            string description = null, string isContainment = null)
        {
            Id = id;
            Label = label;
            NavigationActionLabel = navigationActionLabel;
            BasedOn = basedOn;
            CanBeDataDriven = canBeDataDriven;
            CanLinkedNodesBeDataDriven = canLinkedNodesBeDataDriven;
            DefaultAction = defaultAction;
            Icon = icon;
            IncomingActionLabel = incomingActionLabel;
            OutgoingActionLabel = outgoingActionLabel;
            Description = description;
            IsContainment = isContainment;
        }
    }
}
