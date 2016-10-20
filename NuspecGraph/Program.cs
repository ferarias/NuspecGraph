using System;
using System.IO;
using System.Text;
using System.Xml;
using DgmlWriter;
using Fclp;

namespace NuspecGraph
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Parse arguments into settings
            var settings = ParseArguments(args);
            if (settings == null) return;

            // Obtain nuspec files to process
            var nuspecFiles = GetNuspecFiles(settings.InputFolder);
            if (nuspecFiles == null) return;

            // Process files
            var dgmlWriter = new DgmlWriter.DgmlWriter();
            
            CustomizeGraph(dgmlWriter);
            foreach (var nuspecFile in nuspecFiles)
            {
                Console.Write($"Processing {nuspecFile}...");

                // Parse .nuspec file
                var reader = new NuspecReader.NuspecReader();
                reader.Read(nuspecFile);
                dgmlWriter.AddNode(reader.Package.Id.ToLower(), reader.Package.GetLabel(), "NuGetPackage", nuspecFile);

                // Obtain and set container (repository)
                var container = GetNodeContainer(nuspecFile, settings.GroupFolder);
                if (!container.HasValue) continue;
                if (!dgmlWriter.ExistsContainer(container.Value.Id))
                    dgmlWriter.AddContainer(container.Value.Id, container.Value.Label, "Repository", container.Value.FilePath);
                dgmlWriter.AddNodeToContainer(container.Value.Id, reader.Package.Id.ToLower());
                Console.WriteLine($"in repo {container.Value.Label}.");
            }

            dgmlWriter.AddCollapsedContainer("ext", "External", "ExternalRepository", string.Empty);
            foreach (var nuspecFile in nuspecFiles)
            {
                var reader = new NuspecReader.NuspecReader();
                reader.Read(nuspecFile);
                foreach (var dependency in reader.Dependencies)
                {
                    var dependencyId = dependency.Id.ToLower();
                    if (!dgmlWriter.ExistsNode(dependencyId))
                    {
                        dgmlWriter.AddNode(dependencyId, dependency.GetLabel(), "ExternalNuGetPackage");
                        dgmlWriter.SetParent("ext", dependencyId);
                    }
                    dgmlWriter.AddLink(dependencyId, reader.Package.Id.ToLower(), dependency.Version, "DependsOn");
                }
            }

            using (var ms = new FileStream(settings.OutputFile, FileMode.Create))
            {
                XmlWriter io = new XmlTextWriter(ms, Encoding.Unicode);
                dgmlWriter.Serialize(io);
            }

        }

        private static Node? GetNodeContainer(string nuspecFile, string groupFolder)
        {
            var fi = new FileInfo(nuspecFile);
            var di = fi.Directory;
            if (di?.Name == groupFolder)
            {
                return new Node
                {
                    Id = di?.Parent?.Name.ToLower(),
                    Label = di?.Parent?.Name,
                    FilePath = di?.Parent?.FullName
                };
            }
            return null;
        }

        private static string[] GetNuspecFiles(string inputFolder)
        {
            Console.WriteLine($"Reading .nuspec files from {inputFolder}");
            var nuspecFiles = Directory.GetFiles(inputFolder, "*.nuspec", SearchOption.AllDirectories);
            if (nuspecFiles.Length != 0) return nuspecFiles;
            Console.WriteLine("No .nuspec files found");
            return null;
        }

        private static AppSettings ParseArguments(string[] args)
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            // create a generic parser for the ApplicationArguments type
            var p = new FluentCommandLineParser<AppSettings>();

            // specify which property the value will be assigned too.
            p.Setup(arg => arg.InputFolder)
                .As('i', "folder") // define the short and long option name
                .SetDefault(folder);

            p.Setup(arg => arg.OutputFile)
                .As('o', "out")
                .SetDefault(Path.Combine(folder, "out.dgml"));

            p.Setup(arg => arg.GroupFolder)
                .As('g', "groupfolder")
                .SetDefault("NuGetPackages");


            var result = p.Parse(args);
            if (!result.HasErrors) return p.Object;

            Console.WriteLine(result.ErrorText);
            return null;
        }

        private static void CustomizeGraph(DgmlWriter.DgmlWriter writer)
        {
            writer.Background = "#ffffff";
            writer.Stroke = "#000000";
            writer.Styles.Add(new Style(null, "HasCategory('NuGetPackage')", "Node", "NuGet Package", "NuGet Package", new Style.StyleProperty("Background", "#FFBCFFBE"), new Style.StyleProperty("Background", "#FF0E619A"), new Style.StyleProperty("Stroke", "#FF0E619A"), new Style.StyleProperty("Icon", "CodeSchema_Method")));
            writer.Styles.Add(new Style(null, "HasCategory('Repository')", "Node", "Repository", "Repository", new Style.StyleProperty("Background", "#FFDEBA83"), new Style.StyleProperty("Foreground", "#cccccc"), new Style.StyleProperty("Icon", "CodeSchema_Namespace")));
            writer.Styles.Add(new Style("Hidden", "HasCategory('Contains')", "Link", "Contains", "Contains", new Style.StyleProperty("Background", "#9A9A9A")));
            writer.Styles.Add(new Style("Hidden", "HasCategory('DependsOn') And Target.HasCategory('NuGetPackage')", "Link", "Depends On", "Dependency", new Style.StyleProperty("Stroke", "#9A9A9A"), new Style.StyleProperty("StrokeDashArray", "2 2"), new Style.StyleProperty("DrawArrow", "true")));
            writer.Styles.Add(new Style("Hidden", "HasCategory('DependsOn') And Target.HasCategory('ExternalNuGetPackage')", "Link", "Depends On", "True", new Style.StyleProperty("TargetDecorator", "OpenArrow")));
        }
    }
}
