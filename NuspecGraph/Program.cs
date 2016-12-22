using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using DgmlWriter;
using Fclp;
using NuspecReader;

namespace NuspecGraph
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Parse arguments into settings
            var settings = ParseArguments(args);
            if (settings == null) return;

            Console.WriteLine("NuSpecGraph - A NuSpec file parser and DGML dependency generator");
            Console.WriteLine($"Input folder: {settings.InputFolder}");
            Console.WriteLine($"Output file: {settings.OutputFile}");
            Console.WriteLine($"Groups folder name: {settings.GroupFolder}");

            // Initialize a new graph
            var dgmlWriter = new DgmlWriter.DgmlWriter();
            CustomizeGraph(dgmlWriter);

            // Look for nuspec files to process and read their contents
            var nuspecDataList = ReadNuGetFiles(settings.InputFolder, settings.GroupFolder, settings.Verbose);
            if (nuspecDataList == null) return;

            // Add nodes to graph
            foreach (var nuspecData in nuspecDataList)
            {
                dgmlWriter.AddNode(nuspecData.PackageId, nuspecData.Label, "NuGetPackage", nuspecData.FilePath, nuspecData.Container);
            }
            
            // Add dependencies to graph
            foreach (var nuspecData in nuspecDataList)
            {
                foreach (var dependency in nuspecData.Dependencies)
                {
                    if (!dgmlWriter.ExistsNode(dependency.Id))
                    {
                        dgmlWriter.AddNode(dependency.Id, dependency.GetLabel(), "ExternalNuGetPackage", "");
                        dgmlWriter.SetParent("ext", dependency.Id);
                    }
                    dgmlWriter.AddLink(dependency.Id, nuspecData.PackageId, dependency.Version, "DependsOn");
                }
            }

            // Write graph to file
            using (var ms = new FileStream(settings.OutputFile, FileMode.Create))
            {
                XmlWriter io = new XmlTextWriter(ms, Encoding.Unicode);
                dgmlWriter.Serialize(io);
            }
            Console.WriteLine($"Finished processing {nuspecDataList.Count} files");
        }

        private static List<NuspecData> ReadNuGetFiles(string inputFolder, string groupsFolder, bool verbose = false)
        {
            var nuspecFiles = FindAllNuspecFilesInFolder(inputFolder);
            if (nuspecFiles == null) return null;

            var nuspecData = new List<NuspecData>();
            foreach (var nuspecFile in nuspecFiles)
            {
                if (verbose) Console.WriteLine($"Reading {nuspecFile}...");

                // Parse .nuspec file
                var reader = new NuspecReader.NuspecReader();
                reader.Read(nuspecFile);

                var item = new NuspecData
                {
                    FilePath = nuspecFile,
                    PackageId = reader.Package.Id.ToLower(),
                    Label = reader.Package.GetLabel(),
                    Container = GetNodeContainer(nuspecFile, groupsFolder),
                    Dependencies = reader.Dependencies
                };
                
                nuspecData.Add(item);
            }
            return nuspecData;
        }

        public class NuspecData
        {
            public string FilePath { get; set; }
            public string PackageId { get; set; }
            public string Label { get; set; }
            public Node Container { get; set; }

            public IEnumerable<VersionedPackage> Dependencies { get; set; }
        }

        private static Node GetNodeContainer(string nuspecFile, string groupFolder)
        {
            var fi = new FileInfo(nuspecFile);
            var di = fi.Directory;
            if (di?.Name == groupFolder)
            {
                return new Node
                {
                    Id = di?.Parent?.Name.ToLower(),
                    Label = di?.Parent?.Name,
                    Category = "Repository",
                    FilePath = di?.Parent?.FullName
                };
            }
            return Node.EmptyNode;
        }

        private static string[] FindAllNuspecFilesInFolder(string inputFolder)
        {
            if (string.IsNullOrEmpty(inputFolder))
            {
                Console.WriteLine("inputFolder is empty");
                return null;
            }
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

            p.Setup(arg => arg.Verbose)
                .As('v', "verbose")
                .SetDefault(false);

            p.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

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
            writer.AddCollapsedContainer("ext", "External", "ExternalRepository", string.Empty);
        }
    }
}
