using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Scanning;
using NuGet;

namespace DependencyMap.Analysis
{
    public class ServiceDependenciesAnalyser
    {
        private readonly IList<ServiceDependency> _serviceDependencies;

        public ServiceDependenciesAnalyser(IList<ServiceDependency> serviceDependencies)
        {
            if (serviceDependencies == null)
            {
                throw new ArgumentNullException(nameof(serviceDependencies));
            }
            _serviceDependencies = serviceDependencies;
        }

        public IEnumerable<Service> GetAllServices()
        {
            // Normalise to a value between 1 and 5 where 1 is best.
            Func<IEnumerable<DependencyStaleness>, int> calculateScore = dependencyStalenesses =>
            {
                var average = dependencyStalenesses.Average(x => x.StalenessRating);
                var score = (int)Math.Floor(average) + 1;
                return Math.Min(score, 5);
            };

            foreach (var service in GroupByService())
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

        internal IDictionary<string, DependencyStaleness[]> GroupByService()
        {
            var stalenesses = CalculateDependencyStalenessesAcrossEntireSource().ToList();
            var stalenessLookup = stalenesses.ToDictionary(x => new DependencyIdVersion(x.DependencyId, x.Version), x => x.StalenessRating);

            var latestVersionByDependency = stalenesses.GroupBy(x => x.DependencyId).ToDictionary(
                x => x.Key,
                x => x.OrderByDescending(y => y.Version).First().Version);

            var results = new Dictionary<string, DependencyStaleness[]>();
            foreach (var service in _serviceDependencies.GroupBy(x => x.ServiceId).OrderBy(x => x.Key))
            {
                var dependencyVersions = service.GroupBy(x => new DependencyIdVersion(x.DependencyId, x.DependencyVersion));

                results.Add(service.Key,
                    dependencyVersions.OrderBy(x => stalenessLookup[x.Key]).ThenBy(x => x.Key.DependencyId)
                        .Select(x => new DependencyStaleness
                        {
                            DependencyId = x.First().DependencyId,
                            Version = x.First().DependencyVersion,
                            LatestKnownVersion = latestVersionByDependency[x.First().DependencyId],
                            StalenessRating = stalenessLookup[x.Key]
                        }).ToArray());
            }
            return results;
        }

        private IEnumerable<DependencyStaleness> CalculateDependencyStalenessesAcrossEntireSource()
        {
            foreach (var dependency in _serviceDependencies.GroupBy(x => x.DependencyId))
            {
                var stalenessRating = 0;
                foreach (var version in dependency.GroupBy(x => x.DependencyVersion).OrderByDescending(x => x.Key))
                {
                    yield return new DependencyStaleness
                    {
                        DependencyId = dependency.Key,
                        Version = version.Key,
                        StalenessRating = stalenessRating++
                    };
                }
            }
        }



        public IEnumerable<Dependency> GetAllDependencies()
        {
            foreach (var dependency in GroupByDependency())
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

        internal IDictionary<string, IDictionary<SemanticVersion, string[]>> GroupByDependency()
        {
            var results = new Dictionary<string, IDictionary<SemanticVersion, string[]>>();
            foreach (var dependency in _serviceDependencies.GroupBy(x => x.DependencyId).OrderBy(x => x.Key))
            {
                results.Add(dependency.Key,
                    dependency.GroupBy(x => x.DependencyVersion)
                        .OrderByDescending(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Select(y => y.ServiceId).Distinct().OrderBy(z => z).ToArray())
                    );
            }
            return results;
        }
    }
}