using NuspecFileReader;
using System;
using System.IO;
using System.Text;
using System.Xml;
using DGML;

namespace NuDgm
{
    class Program
    {
        static void Main(string[] args)
        {

            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var name = Path.Combine(folder, "test.nuspec");
            var reader = new NuspecReader();
            reader.Read(name);

            var dgmlWriter = new DgmlWriter();
            dgmlWriter.AddClass(reader.Package.GetId(), reader.Package.GetLabel());
            foreach (var dependency in reader.Dependencies)
            {
                dgmlWriter.AddClass(dependency.GetId(), dependency.GetLabel());
                dgmlWriter.SetLinked(reader.Package.GetId(), dependency.GetId());
            }


            using (var ms = new FileStream("C:\\Users\\fernando.arias\\Desktop\\test.dgml", FileMode.Create))
            {
                XmlWriter io = new XmlTextWriter(ms, Encoding.Unicode);
                dgmlWriter.Serialize(io);
            }

        }
    }
}
