using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Models;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;

namespace DependencyMap
{
    public class Generator
    {
        private readonly Lazy<List<ServiceDependency>> _serviceDependencies;
        
        public Generator(string configFileName, params string[] rootFolders)
        {
            _serviceDependencies = new Lazy<List<ServiceDependency>>(() => GetServiceDependencies(configFileName, rootFolders));
        }

        private static List<ServiceDependency> GetServiceDependencies(string configFileName, params string[] rootFolders)
        {
            var sourceRepository = new FileSystemSourceRepository(configFileName, rootFolders);
            var configScanner = new NugetPackageConfigScanner(sourceRepository);
            return configScanner.GetAllServiceDependencies().ToList();
        }

        public IEnumerable<Dependency> GetAllDependencies()
        {
            var dependencies = new ServiceDependenciesAnalyser(_serviceDependencies.Value).GroupByDependency();

            foreach (var dependency in dependencies)
            {
                var score = Math.Min(dependency.Value.Count, 5);
                yield return new Dependency
                {
                    DependencyId = dependency.Key,
                    Score = score,
                    ServiceUsageByVersion = dependency.Value
                };
            }
        }

        public IEnumerable<Service> GetAllServices()
        {
            var services = new ServiceDependenciesAnalyser(_serviceDependencies.Value).GroupByService();

            // Normalise to a value between 1 and 5 where 1 is best.
            Func<IEnumerable<DependencyStaleness>, int> calculateScore = dependencyStalenesses =>
            {
                var average = dependencyStalenesses.Average(x => x.StalenessRating);
                var score = (int)Math.Floor(average) + 1;
                return Math.Min(score, 5);
            };

            foreach (var service in services)
            {
                var score = calculateScore(service.Value);

                yield return new Service
                {
                    ServiceId = service.Key,
                    Score = score,
                    Dependencies = service.Value.Select(
                        x => new Service.Dependency
                        {
                            DependencyId = x.DependencyId,
                            Version = x.Version,
                            LatestKnownVersion = x.LatestKnownVersion,
                            IsStale = x.Version != x.LatestKnownVersion
                        })
                };
            }
        }
    }
}
