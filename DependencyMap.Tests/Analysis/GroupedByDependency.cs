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
    public class GroupedByDependency
    {
        [Test]
        public void SimpleDependency_ShouldReturnDependenciesAsExpected()
        {
            var input = new ServiceDependency
            {
                ServiceId = "Service0",
                DependencyId = "Dependency0",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] {input});
            var output = analyser.GroupByDependency();

            output.ShouldBeEquivalentTo(
                new Dictionary<string, Dictionary<SemanticVersion, string[]>>
                {
                    {
                        "Dependency0",
                        new Dictionary<SemanticVersion, string[]>
                        {
                            {
                                new SemanticVersion(1, 0, 0, 0),
                                new[] {"Service0"}
                            }
                        }
                    }
                });
        }

        [Test]
        public void DifferentVersions_ShouldReturnServicesGroupedByVersion()
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
            var output = analyser.GroupByDependency();

            output.ShouldBeEquivalentTo(
                new Dictionary<string, Dictionary<SemanticVersion, string[]>>
                {
                    {
                        "Dependency0",
                        new Dictionary<SemanticVersion, string[]>
                        {
                            {
                                new SemanticVersion(1, 0, 0, 0),
                                new[] {"Service0", "Service1"}
                            },
                            {
                                new SemanticVersion(1, 1, 0, 0),
                                new[] {"Service1"}
                            },
                            {
                                new SemanticVersion(2, 0, 0, 0),
                                new[] {"Service2"}
                            }
                        }
                    }
                });
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
            var output = analyser.GroupByDependency();

            // Should.Equal also asserts order
            output[dependencyId][version].Should().Equal(
                input.Select(x => x.ServiceId).OrderBy(x => x));
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
            var output = analyser.GroupByDependency();

            // Should.Equal also asserts order
            output.Select(x => x.Key).Should().Equal(
                input.Select(x => x.DependencyId).OrderBy(x => x));
        }
    }
}