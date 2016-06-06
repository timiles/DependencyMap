using System;
using System.IO;
using Newtonsoft.Json;

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

            var client = new Generator(new FileSystemSourceRepositoryConfig(sourceDir));
            var dependencies = SerializeObjectToJson(client.GetAllDependencies());
            var services = SerializeObjectToJson(client.GetAllServices());

            File.WriteAllText(outputPath, $"g_dependencies = {dependencies};\r\ng_services = {services};");

            return 0;
        }

        private static string SerializeObjectToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, new SemanticVersionConverter());
        }
    }
}
