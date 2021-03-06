﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DependencyMap.SourceRepositories;
using NuGet;

namespace DependencyMap.Scanning
{
    public class NuGetPackageConfigScanner : IDependencyFileScanner
    {
        public IEnumerable<ServiceDependency> GetAllServiceDependencies(IEnumerable<DependencyFile> dependencyFiles)
        {
            var configFiles = dependencyFiles?.ToList();
            if (configFiles == null || configFiles.IsEmpty())
            {
                throw new DependencyFilesNotFoundException();
            }

            foreach (var configFile in configFiles)
            {
                var packages = ProcessPackages(configFile.FileContents);

                foreach (var package in packages)
                {
                    SemanticVersion version;
                    SemanticVersion.TryParse(package.Value, out version);
                    yield return
                        new ServiceDependency
                        {
                            ServiceId = configFile.ServiceId,
                            DependencyId = package.Key,
                            DependencyVersion = version,
                            DependencyFilePath = configFile.FilePath,
                        };
                }
            }
        }

        private static Dictionary<string, string> ProcessPackages(string packagesConfig)
        {
            var lines = packagesConfig.Split('\r', '\n');
            var packages = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var matchPackageId = Regex.Match(line, @"id=""(.*?)""");
                var matchVersion = Regex.Match(line, @"version=""(.*?)""");

                if (matchPackageId.Success && matchVersion.Success)
                {
                    var packageId = ExtractValue(matchPackageId.Value);
                    if (!packages.ContainsKey(packageId))
                    {
                        packages.Add(packageId, ExtractValue(matchVersion.Value));
                    }
                    // TODO: log duplicates?
                }
            }

            return packages;
        }

        private static string ExtractValue(string value)
        {
            return Regex.Match(value, @"""(.*?)""").Value.Replace(@"""", "");
        }
    }
}