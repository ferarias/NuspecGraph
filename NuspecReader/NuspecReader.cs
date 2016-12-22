using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuspecReader
{
    public class NuspecReader
    {
        public VersionedPackage Package { get; private set; }
        public IEnumerable<VersionedPackage> Dependencies { get; private set; }

        public void Read(string fileName)
        {
            using (var sr = new StreamReader(fileName, true))
            {
                var doc = XDocument.Load(sr);
                var ns = doc.Root?.GetDefaultNamespace();

                Package = new VersionedPackage(doc.Descendants(ns + "id").FirstOrDefault()?.Value, doc?.Descendants(ns + "version").FirstOrDefault()?.Value);
                Dependencies = doc?.Descendants(ns + "dependency")?.Select(x => new VersionedPackage(x.Attribute("id")?.Value.ToLower(), x.Attribute("version")?.Value));
            }

           
            
        }
    }
}
