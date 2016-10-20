namespace NuspecReader
{
    public class VersionedPackage
    {
        public VersionedPackage(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; set; }

        public string Version { get; set; }

        public string GetLabel() => $"{Id} v{Version}";
    }
}