using System.Diagnostics;
using System.IO;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;
using Newtonsoft.Json;

namespace WebView.DataUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceDir = "../../../DummySourceRepository/";
            var webViewDir = "../../../WebView/";
            var outputPath = webViewDir + "data.js";

            var repository = new FileSystemSourceRepository("packages.config", new [] { sourceDir });
            var packageConfigs = repository.GetDependencyFiles();

            var scanner = new NuGetPackageConfigScanner();
            var serviceDependencies = scanner.GetAllServiceDependencies(packageConfigs);

            // filter our dependencies if we like
            var nonTestServiceDependencies = serviceDependencies.Where(x => !x.DependencyFilePath.Contains(".Tests"));

            var analyser = new ServiceDependenciesAnalyser(nonTestServiceDependencies.ToList());

            var dependencies = SerializeObjectToJson(analyser.GetAllDependencies());
            var services = SerializeObjectToJson(analyser.GetAllServices());

            File.WriteAllText(outputPath, $"g_dependencies = {dependencies};\r\ng_services = {services};");

            Process.Start("chrome", webViewDir + "dependencies.html");
            Process.Start("chrome", webViewDir + "services.html");
        }

        private static string SerializeObjectToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, new SemanticVersionConverter());
        }
    }
}
