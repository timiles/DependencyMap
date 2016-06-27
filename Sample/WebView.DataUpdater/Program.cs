using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;
using Newtonsoft.Json;
using WebView.DataUpdater.DataModels;

namespace WebView.DataUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            const string webViewDir = "../../../WebView/";
            const string outputPath = webViewDir + "data.js";

            var repository = new GitHubSourceRepository("packages.config", "Particular", true);
            var packageConfigs = repository.GetDependencyFiles();

            var scanner = new NuGetPackageConfigScanner();
            var serviceDependencies = scanner.GetAllServiceDependencies(packageConfigs);

            // filter our dependencies if we like
            var nonTestServiceDependencies = serviceDependencies.Where(x => !x.DependencyFilePath.Contains(".Tests"));

            var analyser = new ServiceDependenciesAnalyser(nonTestServiceDependencies.ToList());


            var data = new RootObject
            {
                LastRunDateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"),
                NuGetPackages = new Anaylsis
                {
                    Dependencies = analyser.GetAllDependencies(),
                    Services = analyser.GetAllServices()
                }
            };
            File.WriteAllText(outputPath, $"g_data = {SerializeObjectToJson(data)};");

            Process.Start("chrome.exe", webViewDir + "index.html");
        }

        private static string SerializeObjectToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, new SemanticVersionConverter());
        }
    }
}
