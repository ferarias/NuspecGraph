using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DgmlWriter
{
    public class DgmlWriter
    {
        public DgmlWriter()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            Styles = new List<Style>();
            Categories = new List<Category>();
        }

        public string Background { get; set; }
        public string Stroke { get; set; }

        public List<Node> Nodes { get; }
        public List<Link> Links { get; }
        public List<Style> Styles { get; }
        public List<Category> Categories { get; }
        public List<Property> Properties { get; private set; }

        public void AddContainer(string id, string label, string path)
        {
            Nodes.Add(new Node(id, label, "Expanded", filePath: path));
        }

        public void AddContainer(string id, string label, string category, string path)
        {
            Nodes.Add(new Node(id, label, "Expanded", category:category, filePath: path));
        }
        public void AddCollapsedContainer(string id, string label, string category, string path)
        {
            Nodes.Add(new Node(id, label, "Collapsed", category:category, filePath: path));
        }

        public bool ExistsContainer(string id)
        {
            return Nodes.Exists(n => (n.Id == id) && (n.Group == "Expanded"));
        }

        public void AddNodeToContainer(Node containerNode, string targetId)
        {
            if (!ExistsContainer(containerNode.Id))
                AddContainer(containerNode.Id, containerNode.Label, containerNode.Category, containerNode.FilePath);
            Links.Add(new Link { Category = "Contains", Source = containerNode.Id, Target = targetId });
        }

        public void AddNode(string id, string label)
        {
            Nodes.Add(new Node(id, label));
        }

        public void AddNode(string id, string label, string path)
        {
            Nodes.Add(new Node(id, label, filePath: path));
        }

        public void AddNode(string id, string label, string category, string path)
        {
            Nodes.Add(new Node(id, label, category:category, filePath: path));
        }

        public void AddNode(string id, string label, string category, string path, Node container)
        {
            Nodes.Add(new Node(id, label, category: category, filePath: path));
            if (!container.IsEmpty)
            {
                AddNodeToContainer(container, id);
            }
        }

        public bool ExistsNode(string id)
        {
            return Nodes.Exists(n => n.Id == id);
        }

        public void AddLink(string sourceId, string targetId, string label)
        {
            Links.Add(new Link {Source = sourceId, Target = targetId, Label = label});
        }
        public void AddLink(string sourceId, string targetId, string label, string category)
        {
            Links.Add(new Link { Source = sourceId, Target = targetId, Label = label, Category = category});
        }

        public void Serialize(XmlWriter io)
        {
            var graph = new Graph
            {
                Background = Background,
                Stroke = Stroke,
                Nodes = Nodes.ToArray(),
                Links = Links.ToArray(),
                Categories = Categories.ToArray(),
                Styles = Styles.ToArray()
            };

            var root = new XmlRootAttribute("DirectedGraph") {Namespace = "http://schemas.microsoft.com/vs/2009/dgml"};
            
            var serializer = new XmlSerializer(typeof(Graph), root);
            serializer.Serialize(io, graph);
        }

        /// <summary>
        ///     Adds an assembly to the graph. Examples of an assembly would be "my_typescript_file.ts".
        /// </summary>
        /// <param name="id">The unique id for the assembly</param>
        /// <param name="label">The label which is displayed on the graph</param>
        public void AddRepository(string id, string label)
        {
            Nodes.Add(new Node(id, label, "Expanded", "Repository"));
        }

        /// <summary>
        ///     Indicates that the node with the given parentId should contain the node with the given childId
        /// </summary>
        /// <param name="parentId">The id of the parent node</param>
        /// <param name="childId">The id of the child node</param>
        public void SetParent(string parentId, string childId)
        {
            Links.Add(new Link(parentId, childId, "Contains"));
        }

        /// <summary>
        ///     Indicates that the node with the given fromId should be linked to the node with the given toId
        /// </summary>
        /// <param name="fromId">The id of the first node to be linked</param>
        /// <param name="toId">The id of the second node to be linked</param>
        public void SetLinked(string fromId, string toId)
        {
            Links.Add(new Link(fromId, toId));
        }
    }
}