using System.Collections.Generic;
using DependencyMap.Analysis;
using DependencyMap.Scanning;
using FluentAssertions;
using NuGet;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class GroupedByService
    {
        [Test]
        public void SimpleDependency_ShouldReturnServicesAsExpected()
        {
            var input = new ServiceDependency
            {
                ServiceId = "Service0",
                DependencyId = "Dependency0",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] { input });
            var output = analyser.GroupByService();

            output.ShouldBeEquivalentTo(
                new Dictionary<string, DependencyStaleness[]>
                {
                    {
                        "Service0", new[]
                        {
                            new DependencyStaleness
                            {
                                DependencyId = "Dependency0",
                                LatestKnownVersion = new SemanticVersion(1, 0, 0, 0),
                                Version = new SemanticVersion(1, 0, 0, 0),
                                StalenessRating = 0
                            }
                        }
                    }
                });
        }

        [Test]
        public void DifferentVersions_ShouldReturnDifferentStalenesses()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 1, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service2",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, 0)
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByService();

            output.ShouldBeEquivalentTo(
                new Dictionary<string, DependencyStaleness[]>
                {
                    {
                        "Service0", new[]
                        {
                            new DependencyStaleness
                            {
                                DependencyId = "Dependency0",
                                LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                Version = new SemanticVersion(1, 0, 0, 0),
                                StalenessRating = 2
                            }
                        }
                    },
                    {
                        "Service1",
                        new[]
                        {
                            new DependencyStaleness
                            {
                                DependencyId = "Dependency0",
                                LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                Version = new SemanticVersion(1, 0, 0, 0),
                                StalenessRating = 2
                            },
                            new DependencyStaleness
                            {
                                DependencyId = "Dependency0",
                                LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                Version = new SemanticVersion(1, 1, 0, 0),
                                StalenessRating = 1
                            }
                        }
                    },
                    {
                        "Service2", new[]
                        {
                            new DependencyStaleness
                            {
                                DependencyId = "Dependency0",
                                LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                Version = new SemanticVersion(2, 0, 0, 0),
                                StalenessRating = 0
                            }
                        }
                    }
                });
        }
    }
}