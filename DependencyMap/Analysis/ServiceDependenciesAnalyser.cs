using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Models;

namespace DependencyMap.Analysis
{
    internal class ServiceDependenciesAnalyser
    {
        private readonly IList<ServiceDependency> _serviceDependencies;

        internal ServiceDependenciesAnalyser(IList<ServiceDependency> serviceDependencies)
        {
            _serviceDependencies = serviceDependencies;
        }

        internal IDictionary<string, DependencyStaleness[]> GroupByService()
        {
            var stalenesses = CalculateDependencyStalenessesAcrossEntireSource().ToList();
            var stalenessLookup = stalenesses.ToDictionary(x => x.DependencyId + ":" + x.Version, x => x.StalenessRating);

            var latestVersionByDependency = stalenesses.GroupBy(x => x.DependencyId).ToDictionary(
                x => x.Key,
                x => x.OrderByDescending(y => y.Version).First().Version);

            var results = new Dictionary<string, DependencyStaleness[]>();
            foreach (var service in _serviceDependencies.GroupBy(x => x.ServiceId))
            {
                var dependencyVersions = service.GroupBy(x => x.DependencyId + ":" + x.DependencyVersion);
                results.Add(service.Key,
                    dependencyVersions.OrderBy(x => stalenessLookup[x.Key]).Select(x => new DependencyStaleness
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

        internal IDictionary<string, IDictionary<Version, string[]>> GroupByDependency()
        {
            var results = new Dictionary<string, IDictionary<Version, string[]>>();
            foreach (var dependency in _serviceDependencies.GroupBy(x => x.DependencyId).OrderBy(x => x.Key))
            {
                results.Add(dependency.Key,
                    dependency.GroupBy(x => x.DependencyVersion)
                        .OrderByDescending(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Select(y => y.ServiceId).Distinct().ToArray())
                    );
            }
            return results;
        }
    }
}