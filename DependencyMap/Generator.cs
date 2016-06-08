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
        private readonly NuGetPackageConfigScanner _configScanner;

        public Generator(IFileSystemSourceRepositoryConfig config) : this(new FileSystemSourceRepository(config))
        {
        }

        public Generator(IGitHubSourceRepositoryConfig config) : this(new GitHubSourceRepository(config))
        {
        }

        private Generator(ISourceRepository sourceRepository)
        {
            _configScanner = new NuGetPackageConfigScanner(sourceRepository);
        }

        public IEnumerable<Dependency> GetAllDependencies()
        {
            var serviceDependencies = _configScanner.GetAllServiceDependencies().ToList();
            var dependencies = new ServiceDependenciesAnalyser(serviceDependencies).GroupByDependency();

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
            var serviceDependencies = _configScanner.GetAllServiceDependencies().ToList();
            var services = new ServiceDependenciesAnalyser(serviceDependencies).GroupByService();

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
