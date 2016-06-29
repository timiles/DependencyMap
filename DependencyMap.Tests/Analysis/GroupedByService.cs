using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void InputDependenciesNotOrdered_ShouldReturnOutputOrderedByDependencyId()
        {
            var serviceId = "Service0";
            var version = new SemanticVersion(1, 0, 0, 0);
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = serviceId,
                    DependencyId = "Charlie",
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = serviceId,
                    DependencyId = "Bravo",
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = serviceId,
                    DependencyId = "Alpha",
                    DependencyVersion = version
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByService();

            // Should.Equal also asserts order
            output[serviceId].Select(x => x.DependencyId).Should().Equal(
                input.Select(x => x.DependencyId).OrderBy(x => x));
        }

        [Test]
        public void InputServicesNotOrdered_ShouldReturnOutputOrderedByServiceId()
        {
            var dependencyId = "Dependency0";
            var version = new SemanticVersion(1, 0, 0, 0);
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Charlie",
                    DependencyId = dependencyId,
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = "Bravo",
                    DependencyId = dependencyId,
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = "Alpha",
                    DependencyId = dependencyId,
                    DependencyVersion = version
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByService();

            // Should.Equal also asserts order
            output.Select(x => x.Key).Should().Equal(
                input.Select(x => x.ServiceId).OrderBy(x => x));
        }

        [Test]
        public void HighestVersionIsPreRelease_ShouldNotBeUsedAsLatestKnownVersion()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, "alpha")
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
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
                                LatestKnownVersion = new SemanticVersion(1, 0, 0, 0),
                                Version = new SemanticVersion(2, 0, 0, "alpha"),
                                StalenessRating = 0
                            },
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
        public void OnlyVersionIsPreRelease_ShouldBeUsedAsLatestKnownVersion()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, "alpha")
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
                                LatestKnownVersion = new SemanticVersion(2, 0, 0, "alpha"),
                                Version = new SemanticVersion(2, 0, 0, "alpha"),
                                StalenessRating = 0
                            }
                        }
                    }
                });
        }

    }
}