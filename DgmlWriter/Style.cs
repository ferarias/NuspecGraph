using System.Xml.Serialization;

namespace DGML
{
    public struct Style
    {
        public struct StyleCondition
        {
            [XmlAttribute]
            public string Expression;
            public StyleCondition(string expression)
            {
                Expression = expression;
            }
        }
        public struct StyleProperty
        {
            [XmlAttribute]
            public string Property;
            [XmlAttribute]
            public string Value;
            public StyleProperty(string property, string value)
            {
                Property = property;
                Value = value;
            }
        }
        [XmlAttribute]
        public string Hidden;
        [XmlAttribute]
        public string TargetType;
        [XmlAttribute]
        public string GroupLabel;
        [XmlAttribute]
        public string ValueLabel;
        [XmlElement(ElementName = "Condition")]
        public StyleCondition Condition;
        [XmlElement(ElementName = "Setter")]
        public StyleProperty[] Property;
        public Style(string hidden, string condition, string targetType = null, string groupLabel = null, string valueLabel = null, params StyleProperty[] properties)
        {
            Hidden = hidden;
            TargetType = targetType;
            GroupLabel = groupLabel;
            ValueLabel = valueLabel;
            Condition = new StyleCondition(condition);
            Property = properties;
        }
    }
}
