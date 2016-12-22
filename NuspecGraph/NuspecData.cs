using System.Collections.Generic;
using Toolfactory.Dgml;

namespace NuspecReader
{
    public class NuspecData
    {
        public string FilePath { get; set; }
        public string PackageId { get; set; }
        public string Label { get; set; }
        public Node Container { get; set; }
        public IEnumerable<VersionedPackage> Dependencies { get; set; }
    }
}
