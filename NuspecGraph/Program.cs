using System;
using System.IO;
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

            // Initialize a new grapher
            var grapher = new Grapher(settings);

            // Run graph process
            var count = grapher.Do();

            // Write graph to file
            grapher.WriteToFile(settings.OutputFile);
            Console.WriteLine($"Finished processing {count} files");
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
                .As('g', "groupfolder");

            p.Setup(arg => arg.UsePrefixes)
                .As('p', "prefixes");

            p.Setup(arg => arg.Verbose)
                .As('v', "verbose")
                .SetDefault(false);

            p.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

            var result = p.Parse(args);
            if (!result.HasErrors)
            {
                var appSettings = p.Object;
                Console.WriteLine("NuSpecGraph - A NuSpec file parser and DGML dependency generator");
                Console.WriteLine($"Input folder: {appSettings.InputFolder}");
                Console.WriteLine($"Output file: {appSettings.OutputFile}");
                if(!string.IsNullOrEmpty(appSettings.GroupFolder))
                    Console.WriteLine($"Groups folder name: {appSettings.GroupFolder}");
                if (appSettings.UsePrefixes)
                    Console.WriteLine("Using prefixes to group packages");
                return appSettings;
            }

            Console.WriteLine(result.ErrorText);
            return null;
        }
    }
}
