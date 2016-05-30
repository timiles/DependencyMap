using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DependencyMap.JsonGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(@"USAGE: DependencyMap.JsonGenerator.exe <sourceDir> <outputPath>");
                return -1;
            }
            var sourceDir = args[0];
            var outputPath = args[1];

            var client = new Generator("packages.config", sourceDir);
            var dependencies = SerializeJson(client.GetAllDependencies());
            var services = SerializeJson(client.GetAllServices());

            File.WriteAllText(outputPath, $"g_dependencies = {dependencies};\r\ng_services = {services};");

            return 0;
        }

        private static string SerializeJson(object getServicesByDependency)
        {
            return JsonConvert.SerializeObject(getServicesByDependency, Formatting.Indented, new VersionConverter());
        }
    }
}
