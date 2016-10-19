using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuspecFileReader
{
    public class NuspecReader
    {
        public VersionedPackage Package { get; private set; }
        public IEnumerable<VersionedPackage> Dependencies { get; private set; }

        public void Read(string fileName)
        {
            var doc = XDocument.Load(fileName, LoadOptions.None);
            var ns = doc?.Root?.GetDefaultNamespace();

            Package = new VersionedPackage(doc?.Descendants(ns + "id").FirstOrDefault()?.Value, doc?.Descendants(ns + "version").FirstOrDefault()?.Value);
            Dependencies = doc?.Descendants(ns + "dependency")?.Select(x => new VersionedPackage(x.Attribute("id")?.Value, x.Attribute("version")?.Value));
        }
    }
}
