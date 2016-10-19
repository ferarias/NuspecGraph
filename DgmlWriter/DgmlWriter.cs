using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using static DGML.Style;

namespace DGML
{
    public class DgmlWriter
    {
        public List<Node> Nodes { get; private set; }
        public List<Link> Links { get; private set; }
        public List<Style> Styles { get; private set; }
        public List<Category> Categories { get; private set; }
        public List<Property> Properties { get; private set; }
        public DgmlWriter()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            Styles = new List<Style>();
            Categories = new List<Category>();
            AddDefaultStyles();
        }

        private void AddDefaultStyles()
        {
            AddStyle(new Style(null, "HasCategory('QueryResult')", "Node", "Results", "True", new StyleProperty("Background", "#FFBCFFBE")));
            AddStyle(new Style(null, "HasCategory('CodeMap_TestProject')", "Node", "Test Project", "Test Project", new StyleProperty("Icon", "CodeMap_TestProject"), new StyleProperty("Background", "#FF307A69")));
            AddStyle(new Style(null, "HasCategory('CodeMap_WebProject')", "Node", "Web Project", "Web Project", new StyleProperty("Icon", "CodeMap_WebProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_WindowsStoreProject')", "Node", "Windows Store Project", "Windows Store Project", new StyleProperty("Icon", "CodeMap_WindowsStoreProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_PhoneProject')", "Node", "Phone Project", "Phone Project", new StyleProperty("Icon", "CodeMap_PhoneProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_PortableLibraryProject')", "Node", "Portable Library", "Portable Library", new StyleProperty("Icon", "CodeMap_PortableLibraryProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_WpfProject')", "Node", "WPF Project", "WPF Project", new StyleProperty("Icon", "CodeMap_WpfProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_VsixProject')", "Node", "VSIX Project", "VSIX Project", new StyleProperty("Icon", "CodeMap_VsixProject")));
            AddStyle(new Style(null, "HasCategory('CodeMap_ModelingProject')", "Node", "Modeling Project", "Modeling Project", new StyleProperty("Icon", "CodeMap_ModelingProject")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Assembly')", "Node", "Assembly", "Has category", new StyleProperty("Background", "#FF094167"), new StyleProperty("Stroke", "#FF094167"), new StyleProperty("Icon", "CodeSchema_Assembly")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Namespace')", "Node", "Namespace", "Has category", new StyleProperty("Background", "#FF0E619A"), new StyleProperty("Stroke", "#FF0E619A"), new StyleProperty("Icon", "CodeSchema_Namespace")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Interface')", "Node", "Interface", "Has category", new StyleProperty("Background", "#FF1382CE"), new StyleProperty("Stroke", "#FF1382CE"), new StyleProperty("Icon", "CodeSchema_Interface")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Struct')", "Node", "Struct", "Has category", new StyleProperty("Background", "#FF1382CE"), new StyleProperty("Stroke", "#FF1382CE"), new StyleProperty("Icon", "CodeSchema_Struct")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Enum')", "Node", "Enumeration", "Has category", new StyleProperty("Background", "#FF1382CE"), new StyleProperty("Stroke", "#FF1382CE"), new StyleProperty("Icon", "CodeSchema_Enum"), new StyleProperty("LayoutSettings", "List")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Delegate')", "Node", "Delegate", "Has category", new StyleProperty("Background", "#FF1382CE"), new StyleProperty("Stroke", "#FF1382CE"), new StyleProperty("Icon", "CodeSchema_Delegate")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Type')", "Node", "Class", "Has category", new StyleProperty("Background", "#FF1382CE"), new StyleProperty("Stroke", "#FF1382CE"), new StyleProperty("Icon", "CodeSchema_Class")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Property')", "Node", "Property", "Has category", new StyleProperty("Background", "#FFE0E0E0"), new StyleProperty("Stroke", "#FFE0E0E0"), new StyleProperty("Icon", "CodeSchema_Property")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Method') Or HasCategory('CodeSchema_CallStackUnresolvedMethod')", "Node", "Method", "Has category", new StyleProperty("Background", "#FFE0E0E0"), new StyleProperty("Stroke", "#FFE0E0E0"), new StyleProperty("Icon", "CodeSchema_Method"), new StyleProperty("LayoutSettings", "List")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Event')", "Node", "Event", "Has category", new StyleProperty("Background", "#FFE0E0E0"), new StyleProperty("Stroke", "#FFE0E0E0"), new StyleProperty("Icon", "CodeSchema_Event")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Field')", "Node", "Field", "Has category", new StyleProperty("Background", "#FFE0E0E0"), new StyleProperty("Stroke", "#FFE0E0E0"), new StyleProperty("Icon", "CodeSchema_Field")));
            AddStyle(new Style(null, "CodeSchemaProperty_IsOut = 'True'", "Node", "Out Parameter", "Has category", new StyleProperty("Icon", "CodeSchema_OutParameter")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Parameter')", "Node", "Parameter", "Has category", new StyleProperty("Icon", "CodeSchema_Parameter")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_LocalExpression')", "Node", "Local Variable", "Has category", new StyleProperty("Icon", "CodeSchema_LocalExpression")));
            AddStyle(new Style(null, "HasCategory('Externals')", "Node", "Externals", "Has category", new StyleProperty("Background", "#FF424242"), new StyleProperty("Stroke", "#FF424242")));
            AddStyle(new Style(null, "HasCategory('InheritsFrom')", "Link", "Inherits From", "True", new StyleProperty("Stroke", "#FF00A600"), new StyleProperty("StrokeDashArray", "2 0"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('Implements')", "Link", "Implements", "True", new StyleProperty("Stroke", "#8000A600"), new StyleProperty("StrokeDashArray", "2 2"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_Calls')", "Link", "Calls", "True", new StyleProperty("Stroke", "#FFFF00FF"), new StyleProperty("StrokeDashArray", "2 0"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_FunctionPointer')", "Link", "Function Pointer", "True", new StyleProperty("Stroke", "#FFFF00FF"), new StyleProperty("StrokeDashArray", "2 2"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_FieldRead')", "Link", "Field Read", "True", new StyleProperty("Stroke", "#FF00AEEF"), new StyleProperty("StrokeDashArray", "2 2"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('CodeSchema_FieldWrite')", "Link", "Field Write", "True", new StyleProperty("Stroke", "#FF00AEEF"), new StyleProperty("DrawArrow", "true"), new StyleProperty("IsHidden", "false")));
            AddStyle(new Style(null, "HasCategory('CodeMap_ProjectReference')", "Link", "Project Reference", "Project Reference", new StyleProperty("Stroke", "#9A9A9A"), new StyleProperty("StrokeDashArray", "2 2"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style(null, "HasCategory('CodeMap_ExternalReference')", "Link", "External Reference", "External Reference", new StyleProperty("Stroke", "#9A9A9A"), new StyleProperty("StrokeDashArray", "2 2"), new StyleProperty("DrawArrow", "true")));
            AddStyle(new Style("Hidden", "HasCategory('InheritsFrom') And Target.HasCategory('CodeSchema_Class')", "Link", "Inherits From", "True", new StyleProperty("TargetDecorator", "OpenArrow")));
            AddStyle(new Style("Hidden", "HasCategory('Implements') And Target.HasCategory('CodeSchema_Interface')", "Link", "Implements", "True", new StyleProperty("TargetDecorator", "OpenArrow")));
            AddStyle(new Style("Hidden", "Source.HasCategory('Comment')", "Link", "Comment Link", "True", new StyleProperty("Stroke", "#FFE5C365")));
            AddStyle(new Style("Hidden", "IsCursorLocation", "Node", "Cursor Location Changed", "True", new StyleProperty("IndicatorWest", "WestIndicator")));
            AddStyle(new Style("Hidden", "DisabledBreakpointCount", "Node", "Disabled Breakpoint Location Changed", "True", new StyleProperty("IndicatorWest", "WestIndicator")));
            AddStyle(new Style("Hidden", "EnabledBreakpointCount", "Node", "Enabled Breakpoint Location Changed", "True", new StyleProperty("IndicatorWest", "WestIndicator")));
            AddStyle(new Style("Hidden", "IsInstructionPointerLocation", "Node", "Instruction Pointer Location Changed", "True", new StyleProperty("IndicatorWest", "WestIndicator")));
            AddStyle(new Style("Hidden", "IsCurrentCallstackFrame", "Node", "Current Callstack Changed", "True", new StyleProperty("IndicatorWest", "WestIndicator")));
            AddStyle(new Style("Hidden", "HasCategory('CodeSchema_ReturnTypeLink')", "Link", "Return", "True"));
            AddStyle(new Style("Hidden", "HasCategory('References')", "Link", "References", "True"));
            AddStyle(new Style("Hidden", "HasCategory('CodeSchema_AttributeUse')", "Link", "Uses Attribute", "True"));
            AddStyle(new Style("Hidden", "HasCategory('CodeMap_SolutionFolder')", "Node", "Solution Folder", "True", new StyleProperty("Background", "#FFDEBA83")));
        }
        private void AddNode(Node node)
        {
            Nodes.Add(node);
        }
        /// <summary>
        /// Adds a link between two nodes on the graph. Usually should not be called directly; use the SetParent or SetLinked methods.
        /// </summary>
        /// <param name="link">The link to be added</param>

        private void AddLink(Link link)
        {
            Links.Add(link);
        }

        private void AddStyle(Style style)
        {
            Styles.Add(style);
        }

        private void AddCategory(Category category)
        {
            Categories.Add(category);
        }

        private void AddProperty(Property property)
        {
            Properties.Add(property);
        }
        
        public void Serialize(XmlWriter io)
        {
            Graph graph = new Graph();
            graph.Nodes = Nodes.ToArray();
            graph.Links = Links.ToArray();
            graph.Categories = Categories.ToArray();
            graph.Styles = Styles.ToArray();

            XmlRootAttribute root = new XmlRootAttribute("DirectedGraph");
            root.Namespace = "http://schemas.microsoft.com/vs/2009/dgml";
            XmlSerializer serializer = new XmlSerializer(typeof(Graph), root);
            serializer.Serialize(io, graph);
        }
        /// <summary>
        /// Adds an assembly to the graph. Examples of an assembly would be "my_typescript_file.ts".
        /// </summary>
        /// <param name="id">The unique id for the assembly</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddAssembly(string id, string label)
        {
            AddNode(new Node(id, label, group: "Expanded", category: "CodeSchema_Assembly"));
        }
        /// <summary>
        /// Adds a namespace to the graph.
        /// </summary>
        /// <param name="id">The unique id for the namespace</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddNamespace(string id, string label)
        {
            AddNode(new Node(id, label, group: "Expanded", category: "CodeSchema_Namespace"));
        }
        /// <summary>
        /// Adds a class to the graph.
        /// </summary>
        /// <param name="id">The unique id for the class</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddClass(string id, string label)
        {
            AddNode(new Node(id, label, group: "Expanded", category: "CodeSchema_Class"));
        }
        /// <summary>
        /// Adds an interface to the graph.
        /// </summary>
        /// <param name="id">The unique id for the interface</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddInterface(string id, string label)
        {
            AddNode(new Node(id, label, group: "Expanded", category: "CodeSchema_Interface"));
        }
        /// <summary>
        /// Adds an enum to the graph.
        /// </summary>
        /// <param name="id">The unique id for the enum</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddEnum(string id, string label)
        {
            AddNode(new Node(id, label, group: "Expanded", category: "CodeSchema_Enum"));
        }
        /// <summary>
        /// Adds a method to the graph.
        /// </summary>
        /// <param name="id">The unique id for the method</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddMethod(string id, string label)
        {
            AddNode(new Node(id, label, category: "CodeSchema_Method"));
        }
        /// <summary>
        /// Adds a field to the graph.
        /// </summary>
        /// <param name="id">The unique id for the field</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddField(string id, string label)
        {
            AddNode(new Node(id, label, category: "CodeSchema_Field"));
        }
        /// <summary>
        /// Indicates that the node with the given parentId should contain the node with the given childId
        /// </summary>
        /// <param name="parentId">The id of the parent node</param>
        /// <param name="childId">The id of the child node</param>
        public void SetParent(string parentId, string childId)
        {
            AddLink(new Link(parentId, childId, category: "Contains"));
        }
        /// <summary>
        /// Indicates that the node with the given fromId should be linked to the node with the given toId
        /// </summary>
        /// <param name="firstId">The id of the first node to be linked</param>
        /// <param name="to">The id of the second node to be linked</param>
        /// <param name="style">The style to draw the link in; solid by default</param>
        public void SetLinked(string fromId, string toId)
        {
            AddLink(new Link(fromId, toId));
        }
    }
}
