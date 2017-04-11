using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NuspecReader;
using Toolfactory.Dgml;

namespace NuspecGraph
{
    public class Grapher
    {
        private static readonly Style.StyleProperty[] PackageStyles = {
                new Style.StyleProperty("Background", "#FF0E619A"),
                new Style.StyleProperty("Foreground", "#FFFFFF"),
                new Style.StyleProperty("Stroke", "#FF0E619A"),
                new Style.StyleProperty("Icon", "CodeSchema_Method")
            };
        private static readonly Style.StyleProperty[] ContainerStyles = {
                new Style.StyleProperty("Background", "#FFDEBA83"),
                new Style.StyleProperty("Foreground", "#cccccc"),
                new Style.StyleProperty("Icon", "CodeSchema_Namespace")
            };

        private static readonly Style.StyleProperty[] LinkStyles = { new Style.StyleProperty("Foreground", "#FFDEBA83") };

        private static readonly Style.StyleProperty[] DependencyStyles = {
                new Style.StyleProperty("Stroke", "#9A9A9A"),
                new Style.StyleProperty("DrawArrow", "true")
            };

        private static readonly Style.StyleProperty[] RepositoryDependencyStyles = {
                new Style.StyleProperty("Stroke", "#000000"),
                new Style.StyleProperty("DrawArrow", "true")
            };

        private static readonly Style.StyleProperty[] ExternalDependencyStyles = {
                new Style.StyleProperty("Stroke", "#9A9A9A"),
                new Style.StyleProperty("StrokeDashArray", "4 4"),
                new Style.StyleProperty("DrawArrow", "true"),
                new Style.StyleProperty("TargetDecorator", "ClosedArrow")
            };

        private const string PackageCategory = "NuGetPackage";
        private const string ExternalPackageCategory = "ExternalNuGetPackage";
        private const string ContainerCategory = "Repository";
        private const string PackageCategoryCondition = "HasCategory('" + PackageCategory + "')";
        private const string ContainerCategoryCondition = "HasCategory('" + ContainerCategory + "')";
        private const string ContainsLinkCategoryCondition = "HasCategory('" + DgmlWriter.ContainsLinkCategory + "')";
        private const string DependsOnNuGetPackageCondition = "HasCategory('" + DgmlWriter.DependsOnLinkCategory + "') And Source.HasCategory('" + PackageCategory + "')";
        private const string DependsOnRepositoryCondition = "HasCategory('" + DgmlWriter.DependsOnLinkCategory + "') And Source.HasCategory('" + ContainerCategory + "')";
        private const string DependsOnExternalNuGetPackageCondition = "HasCategory('" + DgmlWriter.DependsOnLinkCategory + "') And Source.HasCategory('" + ExternalPackageCategory + "')";

        public string InputFolder { get; set; }
        public string GroupFolder { get; set; }
        public bool UsePrefixes { get; set; }
        public bool Verbose { get; set; }

        private readonly DgmlWriter _dgmlWriter;
        public Grapher(AppSettings settings)
        {
            InputFolder = settings.InputFolder;
            GroupFolder = settings.GroupFolder;
            UsePrefixes = settings.UsePrefixes;
            Verbose = settings.Verbose;
            
            _dgmlWriter = new DgmlWriter();
            CustomizeGraph();
        }

        private void CustomizeGraph()
        {
            _dgmlWriter.AddCollapsedContainer("ext", "External", "ExternalNuGetPackage", string.Empty);
            _dgmlWriter.Background = "#ffffff";
            _dgmlWriter.Stroke = "#000000";
            _dgmlWriter.Styles.Add(new Style(null, PackageCategoryCondition, DgmlWriter.NodeTargetType, PackageCategory, PackageCategory, PackageStyles));
            _dgmlWriter.Styles.Add(new Style(null, ContainerCategoryCondition, DgmlWriter.NodeTargetType, ContainerCategory, ContainerCategory, ContainerStyles));
            _dgmlWriter.Styles.Add(new Style("Hidden", ContainsLinkCategoryCondition, DgmlWriter.LinkTargetType, DgmlWriter.ContainsLinkCategory + PackageCategory, DgmlWriter.ContainsLinkCategory + PackageCategory, LinkStyles));
            _dgmlWriter.Styles.Add(new Style("Hidden", DependsOnNuGetPackageCondition, DgmlWriter.LinkTargetType, DgmlWriter.DependsOnLinkCategory + PackageCategory, DgmlWriter.DependsOnLinkCategory + PackageCategory, DependencyStyles));
            _dgmlWriter.Styles.Add(new Style("Hidden", DependsOnRepositoryCondition, DgmlWriter.LinkTargetType, DgmlWriter.DependsOnLinkCategory + ContainerCategory, DgmlWriter.DependsOnLinkCategory + ContainerCategory, RepositoryDependencyStyles));
            _dgmlWriter.Styles.Add(new Style("Hidden", DependsOnExternalNuGetPackageCondition, DgmlWriter.LinkTargetType, DgmlWriter.DependsOnLinkCategory + ExternalPackageCategory, DgmlWriter.DependsOnLinkCategory + ExternalPackageCategory, ExternalDependencyStyles));
        }

        public int Do()
        {
            // Look for nuspec files to process and read their contents
            var nuspecDataList = ReadNuGetFiles(InputFolder, GroupFolder, UsePrefixes, Verbose);
            if (nuspecDataList == null) return 0;


            // Add nodes to graph
            foreach (var nuspecData in nuspecDataList)
            {
                _dgmlWriter.AddNode(nuspecData.PackageId, nuspecData.Label, PackageCategory, nuspecData.FilePath, nuspecData.Container);
            }

            // Add dependencies to graph
            foreach (var nuspecData in nuspecDataList)
            {
                foreach (var dependency in nuspecData.Dependencies)
                {
                    if (!_dgmlWriter.ExistsNode(dependency.Id))
                    {
                        _dgmlWriter.AddNode(dependency.Id, dependency.GetLabel(), ExternalPackageCategory, "");
                        _dgmlWriter.SetParent("ext", dependency.Id);
                    }
                    _dgmlWriter.AddLink(dependency.Id, nuspecData.PackageId, dependency.Version, "DependsOn");
                }
            }
            return nuspecDataList.Count;
        }


        private static List<NuspecData> ReadNuGetFiles(string inputFolder, string groupsFolder, bool usePrefixes, bool verbose = false)
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

                var packageId = reader.Package.Id.ToLower();
                var item = new NuspecData
                {
                    FilePath = nuspecFile,
                    PackageId = packageId,
                    Label = reader.Package.GetLabel(),
                    Container = GetNodeContainer(nuspecFile, packageId, groupsFolder, usePrefixes),
                    Dependencies = reader.Dependencies
                };

                nuspecData.Add(item);
            }
            return nuspecData;
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


        private static Node GetNodeContainer(string nuspecFile, string packageId, string groupFolder, bool usePrefixes)
        {
            if (!string.IsNullOrEmpty(groupFolder))
            {
                var fi = new FileInfo(nuspecFile);
                var di = fi.Directory;
                if (di != null)
                {
                    if (di.Name == groupFolder)
                    {
                        return new Node
                        {
                            Id = di.Parent?.Name.ToLower(),
                            Label = di.Parent?.Name,
                            Category = ContainerCategory,
                            FilePath = di.Parent?.FullName
                        };
                    }
                    if (usePrefixes)
                    {
                        var idx = di.Name.IndexOf('-');
                        if (idx < 1) return Node.EmptyNode;

                        var name = di.Name.Substring(0, idx);

                        return new Node
                        {
                            Id = name.ToLower(),
                            Label = name,
                            Category = ContainerCategory
                        };
                    }
                }
            }

            

            
            return Node.EmptyNode;
        }

        public void WriteToFile(string file)
        {
            using (var ms = new FileStream(file, FileMode.Create))
            {
                XmlWriter io = new XmlTextWriter(ms, Encoding.Unicode);
                _dgmlWriter.Serialize(io);
            }
        }
    }
}
